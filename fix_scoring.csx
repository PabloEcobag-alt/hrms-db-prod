using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Infrastructures.Persistence.Analytics;
using Microsoft.Extensions.DependencyInjection;
using ApiHrm.Domains.Entities;

var services = new ServiceCollection();
services.AddDbContext<hrmAppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=hrm_db;Username=postgres;Password=postgres"));
services.AddDbContext<AnalyticsDbContext>(options =>
    options.UseSqlite("Data Source=/Users/papap/Multi-Repo/api-hrms/api-hrms/data/recruitment_analytics.db"));

var provider = services.BuildServiceProvider();
var hrmContext = provider.GetRequiredService<hrmAppDbContext>();
var analyticsContext = provider.GetRequiredService<AnalyticsDbContext>();

var applicants = hrmContext.Applicants.Select(a => a.Applicant_ID).ToList();
var scored = analyticsContext.Predictions.Select(p => p.CandidateId).ToList();

var unscored = applicants.Where(id => !scored.Contains(id.ToString())).ToList();
Console.WriteLine($"Found {unscored.Count} unscored applicants: {string.Join(", ", unscored)}");
