using ApiHrm.Domains.Entities;
using ApiHrm.Infrastructures.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiHrm.Infrastructures.Persistence.Seeders
{
    public static class EnterpriseMockSeeder
    {
        public static async Task SeedEnterpriseMockDataAsync(hrmAppDbContext context, ILogger<Program> logger)
        {
            try
            {
                // Strict arrays for data validation
                var validPositions = new[]
                {
                    "On Call",
                    "Merchandiser On Call",
                    "OJT (On-the-Job Trainee)",
                    "Store Attendant",
                    "Commissary Helper",
                    "Intern/Summer Job"
                };

                var validHiringStages = new[] { "Applied", "Initial Interview", "Final Interview", "Job Offer", "Failed" };

                // Sanitize existing data
                logger.LogInformation("Sanitizing existing applicant data");
                var existingApplicants = await context.Applicants.ToListAsync();
                var random = new Random();
                var sanitizedCount = 0;

                foreach (var applicant in existingApplicants)
                {
                    var needsUpdate = false;

                    // Sanitize Position
                    if (!validPositions.Contains(applicant.Position))
                    {
                        applicant.Position = validPositions[random.Next(validPositions.Length)];
                        needsUpdate = true;
                    }

                    // Sanitize Hiring_Stage
                    if (!validHiringStages.Contains(applicant.Hiring_Stage))
                    {
                        applicant.Hiring_Stage = "Initial Interview";
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        sanitizedCount++;
                    }
                }

                if (sanitizedCount > 0)
                {
                    await context.SaveChangesAsync();
                    logger.LogInformation($"Sanitized {sanitizedCount} existing applicants");
                }

                // Check if we need to seed more applicants
                var existingCount = await context.Applicants.CountAsync();
                if (existingCount >= 50)
                {
                    logger.LogInformation("50 or more applicants already exist, skipping enterprise mock seeding");
                    return;
                }

                var applicantsToCreate = 50 - existingCount;
                logger.LogInformation($"Starting enterprise mock seeder for {applicantsToCreate} applicants (existing: {existingCount})");

                var positions = validPositions;
                var sources = new[] { "Website Portal", "LinkedIn", "Referral", "Gmail Submission", "Walk-In" };
                var hiringStages = validHiringStages;
                var firstNames = new[] { "John", "Jane", "Michael", "Sarah", "David", "Emily", "Robert", "Lisa", "James", "Maria", "William", "Jennifer", "Richard", "Patricia", "Joseph", "Linda", "Thomas", "Barbara", "Charles", "Susan", "Christopher", "Jessica", "Daniel", "Karen", "Matthew", "Nancy", "Anthony", "Betty", "Mark", "Helen", "Steven", "Dorothy", "Paul", "Sandra", "Andrew", "Carol", "Joshua", "Michelle", "Kenneth", "Emily", "Kevin", "Laura", "Brian", "Shirley", "George", "Donna", "Timothy", "Carolyn", "Ronald", "Melissa" };
                var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores", "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts" };

                var applicants = new List<Applicant>();

                var skillsPool = new[]
                {
                    "Customer service, cash handling, inventory management, POS systems",
                    "Sales, customer relations, product knowledge, upselling",
                    "Stock management, logistics, forklift operation, warehouse safety",
                    "Communication, problem-solving, conflict resolution, teamwork",
                    "Time management, organization, attention to detail, multitasking",
                    "Computer literacy, Microsoft Office, data entry, reporting",
                    "Team leadership, scheduling, performance management, coaching",
                    "Product display, visual merchandising, inventory tracking, restocking",
                    "Delivery logistics, route planning, customer communication, vehicle maintenance",
                    "Quality control, inspection, documentation, compliance standards"
                };

                var experiencePool = new[]
                {
                    "3 years in retail sales with focus on customer satisfaction and sales targets",
                    "2 years as warehouse associate with experience in inventory management and logistics",
                    "4 years in customer service across retail and food service industries",
                    "1 year as cashier with experience in high-volume retail environments",
                    "5 years in retail management with proven track record of team leadership",
                    "2 years in delivery and logistics with valid driver's license and clean record",
                    "3 years in merchandising with experience in visual display and inventory control",
                    "1 year as stock clerk with experience in receiving and inventory systems",
                    "4 years in customer service with strong communication and problem-solving skills",
                    "2 years in retail with cross-training in multiple departments"
                };

                for (int i = 0; i < applicantsToCreate; i++)
                {
                    var firstName = firstNames[random.Next(firstNames.Length)];
                    var lastName = lastNames[random.Next(lastNames.Length)];
                    var position = positions[random.Next(positions.Length)];
                    var source = sources[random.Next(sources.Length)];
                    var hiringStage = hiringStages[random.Next(hiringStages.Length)];
                    var applicationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-random.Next(1, 90)));

                    var applicant = new Applicant
                    {
                        First_Name = firstName,
                        Middle_Name = random.Next(2) == 0 ? $"{(char)('A' + random.Next(26))}." : null,
                        Last_Name = lastName,
                        Extension_Name = random.Next(5) == 0 ? "Jr." : null,
                        Position = position,
                        Email = $"{firstName.ToLower()}.{lastName.ToLower()}@example.com",
                        Mobile = $"+63{random.Next(900, 999)}{random.Next(10000000, 99999999)}",
                        Contact_Details = random.Next(2) == 0 ? "Available for interview" : null,
                        Payment_Method = random.Next(2) == 0 ? "Bank Transfer" : null,
                        Resume_URL = $"https://example.com/resumes/{firstName}_{lastName}.pdf",
                        Skills = skillsPool[random.Next(skillsPool.Length)],
                        Experience = experiencePool[random.Next(experiencePool.Length)],
                        Status = "Pending",
                        Hiring_Stage = hiringStage,
                        Source = source,
                        NBI_Document_Completed = random.Next(2) == 0,
                        Medical_Document_Completed = random.Next(2) == 0,
                        XRay_Document_Completed = random.Next(2) == 0,
                        Application_Date = applicationDate,
                        Date_Of_Birth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-random.Next(22, 55))),
                        Interview_Date = (hiringStage == "Initial Interview" || hiringStage == "Final Interview" || hiringStage == "Job Offer") ? applicationDate.AddDays(random.Next(5, 15)) : null,
                        Interview_Notes = random.Next(3) == 0 ? "Strong candidate with relevant experience" : null
                    };

                    applicants.Add(applicant);
                }

                context.Applicants.AddRange(applicants);
                await context.SaveChangesAsync();

                logger.LogInformation($"Successfully seeded {applicantsToCreate} mock applicants");
                logger.LogInformation("Enterprise mock seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during enterprise mock seeding");
                throw;
            }
        }

    }
}
