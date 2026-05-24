using Microsoft.EntityFrameworkCore;
using Applications.Services;
using ApiHrm.Infrastructures.Mapping;
using Applications.Interfaces;
using ApiHrm.Infrastructures.Persistence;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database connection string from environment variables
var connectionString =
    $"Host={Environment.GetEnvironmentVariable("POSTGRES_DB_HOST")};" +
    $"Port={Environment.GetEnvironmentVariable("POSTGRES_DB_PORT")};" +
    $"Database=hrm_db;" +
    $"Username={Environment.GetEnvironmentVariable("POSTGRES_USERNAME")};" +
    $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";

builder.Services.AddDbContext<hrmAppDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IApplicantService, ApplicantService>();
builder.Services.AddScoped<IChecklistService, ChecklistService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IExitService, ExitService>();


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

app.UseAuthorization();

app.MapControllers();

app.Run();
