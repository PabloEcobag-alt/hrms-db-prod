using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Applications.Services;
using ApiHrm.Infrastructures.Mapping; // Temporarily re-enabled for migration
using ApiHrm.Infrastructures.BackgroundServices;
using Applications.Interfaces;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Infrastructures.Persistence.Seeders;
using ApiHrm.Infrastructures.Persistence.Interceptors;
using AutoMapper; // Temporarily re-enabled for migration
// using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.OpenApi;
using Api.Contracts;
// using Microsoft.AspNetCore.Authorization;
// using api_hrm.Authorization;

// Npgsql 6+ requires DateTimeKind.Utc for timestamptz columns.
// This switch restores legacy behavior so DateTime values from JSON
// (Kind=Unspecified) are accepted without explicit UTC conversion.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// QuestPDF License Declaration
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Load ONLY OpenAI related variables from .env to avoid breaking the local Postgres connection
if (System.IO.File.Exists(".env"))
{
    foreach (var line in System.IO.File.ReadAllLines(".env"))
    {
        if (line.StartsWith("OPENAI_API_KEY=") || line.StartsWith("SCORING_PROVIDER="))
        {
            var parts = line.Split('=', 2);
            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }
}

builder.Services.AddAutoMapper(typeof(MappingProfile)); // Temporarily re-enabled for migration
builder.Services.AddHttpContextAccessor();

// MediatR Configuration for Event-Driven Architecture
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Configure CompanySettings
builder.Services.Configure<CompanySettings>(builder.Configuration.GetSection("CompanySettings"));

/*
var webHrmsUrl = Environment.GetEnvironmentVariable("WEB_HRMS_URL")
    ?? "https://localhost:3001";
var jwtAuthority = Environment.GetEnvironmentVariable("JWT_AUTHORITY")
    ?? "https://localhost:5001";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? "hrms-client";

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(webHrmsUrl)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = jwtAuthority;
        options.Audience = jwtAudience;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        // The current Auth Service does not issue resource audiences.
        // Retain JWT_AUDIENCE for the future, but do not enable this yet.
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddSingleton<IAuthorizationHandler, AppPermissionAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    void AddModulePolicy(string policyName, string moduleName, string action)
    {
        options.AddPolicy(policyName, policy =>
        {
            policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new AppPermissionRequirement("HRMS", moduleName, action));
        });
    }

    // ── canRead policies (all modules) ──
    AddModulePolicy("AdminDigital201CanRead", "Admin Digital 201", "canRead");
    AddModulePolicy("AnalyticsCanRead", "Analytics", "canRead");
    AddModulePolicy("RecruitmentAndHiringCanRead", "Recruitment and Hiring", "canRead");
    AddModulePolicy("AttendanceCanRead", "Attendance", "canRead");
    AddModulePolicy("CashAdvanceCanRead", "Cash Advance", "canRead");
    AddModulePolicy("ChecklistCanRead", "Checklist", "canRead");
    AddModulePolicy("Digital201CanRead", "Digital 201", "canRead");
    AddModulePolicy("DocumentCanRead", "Document", "canRead");
    AddModulePolicy("EmployeeInformationCanRead", "Employee Information", "canRead");
    AddModulePolicy("ExitCanRead", "Exit", "canRead");
    AddModulePolicy("LeaveCanRead", "Leave", "canRead");
    AddModulePolicy("LeaveTypeCanRead", "Leave Type", "canRead");
    AddModulePolicy("PayrollCanRead", "Payroll", "canRead");
    AddModulePolicy("RoleCanRead", "Role", "canRead");

    // ── Employee Information action policies ──
    AddModulePolicy("EmployeeInformationCanWrite", "Employee Information", "canWrite");
    AddModulePolicy("EmployeeInformationCanUpdate", "Employee Information", "canUpdate");

    // ── Attendance action policies ──
    AddModulePolicy("AttendanceCanWrite", "Attendance", "canWrite");

    // ── Cash Advance action policies ──
    AddModulePolicy("CashAdvanceCanApprove", "Cash Advance", "canApprove");

    // ── Document action policies ──
    AddModulePolicy("DocumentCanWrite", "Document", "canWrite");
    AddModulePolicy("DocumentCanDelete", "Document", "canDelete");
    AddModulePolicy("DocumentCanApprove", "Document", "canApprove");

    // ── Leave action policies ──
    AddModulePolicy("LeaveCanApprove", "Leave", "canApprove");

    // ── Payroll action policies ──
    AddModulePolicy("PayrollCanWrite", "Payroll", "canWrite");
    AddModulePolicy("PayrollCanApprove", "Payroll", "canApprove");
    AddModulePolicy("PayrollCanExport", "Payroll", "canExport");
    AddModulePolicy("PayrollCanDelete", "Payroll", "canDelete");
});
*/

var webHrmsUrls = Environment.GetEnvironmentVariable("WEB_HRMS_URL")?.Split(',') ?? new[] { "https://localhost:3001", "https://hrms-three-orpin.vercel.app", "https://deploy-web-hrms.vercel.app" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin => 
                origin == "https://localhost:3001" || 
                origin.EndsWith(".vercel.app"))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

// Database connection string from Environment variables (Docker deployment)
var connectionString =
    $"Host={Environment.GetEnvironmentVariable("POSTGRES_DB_HOST") ?? "localhost"};" +
    $"Port={Environment.GetEnvironmentVariable("POSTGRES_DB_PORT") ?? "5432"};" +
    $"Database=hrm_db;" +
    $"Username={Environment.GetEnvironmentVariable("POSTGRES_USERNAME") ?? "postgres"};" +
    $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres"}";

builder.Services.AddSingleton<AuditInterceptor>();
builder.Services.AddDbContext<hrmAppDbContext>((serviceProvider, options) =>
{
    var interceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
    options.UseNpgsql(connectionString)
        .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
        .AddInterceptors(interceptor);
});

// Redis Cache Configuration
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost:6379";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "HRMS_";
});

builder.Services.AddDbContext<AnalyticsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddHttpClient<IAuthServiceClient, ApiHrm.Infrastructures.Services.AuthServiceClient>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

// ===================================================================
// AI Applicant Scoring Pipeline (ARAE Deliverable 1)
// Event-driven: ecommerce submit -> in-memory Channel<int> queue ->
// ScoringBackgroundService -> IScoringService -> SQLite Predictions.
// Default scorer is the local ML model; OpenAI remains opt-in.
// ===================================================================

// Lightweight in-memory queue of ApplicantIds awaiting scoring.
builder.Services.AddSingleton(Channel.CreateUnbounded<int>(new UnboundedChannelOptions
{
    SingleReader = true,
    SingleWriter = false
}));

// Local ML scorer is always registered so the pipeline is deterministic
// and does not depend on external API keys.
builder.Services.AddScoped<ILocalScoringService, LocalScoringService>();

var scoringProvider = Environment.GetEnvironmentVariable("SCORING_PROVIDER") ?? "Local";

// SECURITY: API key is read ONLY from the environment. Never hardcoded.
var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (scoringProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(openAiApiKey))
{
    // Registers IChatCompletionService backed by OpenAI (gpt-4o-mini).
    builder.Services.AddOpenAIChatCompletion(OpenAIScoringService.ModelId, openAiApiKey);
    builder.Services.AddScoped<IScoringService, OpenAIScoringService>();
    Console.WriteLine("[INFO] SCORING_PROVIDER=OpenAI; using gpt-4o-mini for applicant scoring.");
}
else
{
    if (scoringProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("[WARN] SCORING_PROVIDER=OpenAI requested but OPENAI_API_KEY is missing; falling back to LocalScoringService.");
    }
    else
    {
        Console.WriteLine("[INFO] SCORING_PROVIDER=Local; using local ML model for applicant scoring.");
    }

    builder.Services.AddScoped<IScoringService, LocalScoringService>();
}

builder.Services.AddHostedService<ScoringBackgroundService>();

// ===================================================================
// ARAE Deliverable 2 — Vector Search Integration
// Local ChromaDB-backed semantic search for candidate profiles.
// ===================================================================
builder.Services.AddScoped<IVectorSearchService, LocalVectorSearchService>();

// ===================================================================
// ARAE Deliverable 1 — Analytics Dashboard API
// Exposes AI predictions and aggregated analytics to the Next.js frontend.
// ===================================================================
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/api/hrms/openapi/v1.json");
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/api/hrms/openapi/v1.json";
    });
}



app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/hrms", out var remaining))
    {
        context.Request.Path = "/api" + remaining;
    }
    await next();
});

app.UseRouting();

app.UseHttpsRedirection();

app.UseCors();
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

// Auto-Migration on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = services.GetRequiredService<hrmAppDbContext>();
        await dbContext.Database.MigrateAsync();

        // Seed Statutory Data
        await StatutoryDataSeeder.SeedStatutoryDataAsync(dbContext);

        // Seed Roles
        await RoleSeeder.SeedRolesAsync(dbContext);

        // Seed Enterprise Mock Data (50 realistic applicants)
        await EnterpriseMockSeeder.SeedEnterpriseMockDataAsync(dbContext, logger);

        // Seed sample employees + applicants for local dev (idempotent).
        await TestDataSeeder.SeedTestDataAsync(dbContext);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database migration/seeding on startup.");
    }
}

// Initialize SQLite Analytics Database (separate from PostgreSQL)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
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


