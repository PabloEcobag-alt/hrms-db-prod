using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Applications.Services;
using ApiHrm.Infrastructures.Mapping; // Temporarily re-enabled for migration
using ApiHrm.Infrastructures.BackgroundServices;
using Applications.Interfaces;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Infrastructures.Persistence.Seeders;
using AutoMapper; // Temporarily re-enabled for migration
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.OpenApi;
using Api.Contracts;

// Npgsql 6+ requires DateTimeKind.Utc for timestamptz columns.
// This switch restores legacy behavior so DateTime values from JSON
// (Kind=Unspecified) are accepted without explicit UTC conversion.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// QuestPDF License Declaration
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(MappingProfile)); // Temporarily re-enabled for migration
builder.Services.AddHttpContextAccessor();

// Configure CompanySettings
builder.Services.Configure<CompanySettings>(builder.Configuration.GetSection("CompanySettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];
var cookieName = jwtSettings["CookieName"] ?? "sso_token";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authorization = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authorization))
                {
                    return Task.CompletedTask;
                }

                if (context.Request.Cookies.TryGetValue(cookieName, out var token) && !string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendUI", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3002", "http://localhost:3004", "http://localhost:3006")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Database connection string from Environment variables (Docker deployment)
var connectionString =
    $"Host={Environment.GetEnvironmentVariable("POSTGRES_DB_HOST") ?? "localhost"};" +
    $"Port={Environment.GetEnvironmentVariable("POSTGRES_DB_PORT") ?? "5432"};" +
    $"Database=hrm_db;" +
    $"Username={Environment.GetEnvironmentVariable("POSTGRES_USERNAME") ?? "postgres"};" +
    $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres"}";

builder.Services.AddDbContext<hrmAppDbContext>(options =>
    options.UseNpgsql(connectionString)
        .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// Redis Cache Configuration
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost:6379";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "HRMS_";
});

// SQLite Analytics Database Configuration
var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");
Directory.CreateDirectory(dataDirectory);
var sqliteConnectionString = $"Data Source={Path.Combine(dataDirectory, "recruitment_analytics.db")}";
builder.Services.AddDbContext<AnalyticsDbContext>(options =>
    options.UseSqlite(sqliteConnectionString));

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IApplicantService, ApplicantService>();
builder.Services.AddScoped<IChecklistService, ChecklistService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IExitService, ExitService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<ICashAdvanceService, CashAdvanceService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<IPayrollComputationService, PayrollComputationService>();
builder.Services.AddScoped<IPayslipGeneratorService, PayslipGeneratorService>();
builder.Services.AddScoped<IDigital201Service, Digital201Service>();

// ===================================================================
// AI Applicant Scoring Pipeline (Deliverable 2)
// Event-driven: ecommerce submit -> in-memory Channel<int> queue ->
// ScoringBackgroundService -> OpenAI (Semantic Kernel) -> SQLite Predictions.
// ===================================================================

// Lightweight in-memory queue of ApplicantIds awaiting scoring.
builder.Services.AddSingleton(Channel.CreateUnbounded<int>(new UnboundedChannelOptions
{
    SingleReader = true,
    SingleWriter = false
}));

// SECURITY: API key is read ONLY from the environment. Never hardcoded.
var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (!string.IsNullOrWhiteSpace(openAiApiKey))
{
    // Registers IChatCompletionService backed by OpenAI (gpt-4o-mini).
    builder.Services.AddOpenAIChatCompletion(OpenAIScoringService.ModelId, openAiApiKey);
    builder.Services.AddScoped<IScoringService, OpenAIScoringService>();
    builder.Services.AddHostedService<ScoringBackgroundService>();
}
else
{
    Console.WriteLine("[WARN] OPENAI_API_KEY is not set. AI applicant scoring is DISABLED; " +
                      "queued applicants will not be scored until the key is provided.");
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}



app.UseHttpsRedirection();

app.UseCors("FrontendUI");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Auto-Migration on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<hrmAppDbContext>();
        await dbContext.Database.MigrateAsync();

        // Seed Statutory Data
        await StatutoryDataSeeder.SeedStatutoryDataAsync(dbContext);

        // Seed Roles
        await RoleSeeder.SeedRolesAsync(dbContext);

        // Seed sample employees + applicants for local dev (idempotent).
        await TestDataSeeder.SeedTestDataAsync(dbContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration/seeding on startup.");
    }

    // Initialize SQLite Analytics Database (separate from PostgreSQL)
    try
    {
        var analyticsDbContext = services.GetRequiredService<AnalyticsDbContext>();
        await analyticsDbContext.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during SQLite analytics database initialization on startup.");
    }
}

app.Run();
