using Microsoft.EntityFrameworkCore;
using Applications.Services;
using ApiHrm.Infrastructures.Mapping;
using Applications.Interfaces;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Infrastructures.Persistence.Seeders;
using AutoMapper;
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

builder.Services.AddAutoMapper(typeof(MappingProfile));
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
        policy.WithOrigins("http://localhost:3000", "http://localhost:3002")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Database connection string from Configuration or Environment variables
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    connectionString =
        $"Host={Environment.GetEnvironmentVariable("POSTGRES_DB_HOST") ?? "localhost"};" +
        $"Port={Environment.GetEnvironmentVariable("POSTGRES_DB_PORT") ?? "5432"};" +
        $"Database=hrm_db;" +
        $"Username={Environment.GetEnvironmentVariable("POSTGRES_USERNAME") ?? "postgres"};" +
        $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres"}";
}

builder.Services.AddDbContext<hrmAppDbContext>(options =>
    options.UseNpgsql(connectionString));
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
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration/seeding on startup.");
    }
}

app.Run();
