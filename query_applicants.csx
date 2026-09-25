using System;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using Microsoft.Extensions.DependencyInjection;
using ApiHrm.Domains.Entities;
using System.Linq;

var services = new ServiceCollection();
services.AddDbContext<hrmAppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=hrm_db;Username=postgres;Password=postgres"));

var provider = services.BuildServiceProvider();
var context = provider.GetRequiredService<hrmAppDbContext>();

var applicants = context.Applicants.ToList();
foreach(var a in applicants)
{
    Console.WriteLine($"{a.Applicant_ID}: {a.First_Name} {a.Last_Name} - {a.Hiring_Stage}");
}
