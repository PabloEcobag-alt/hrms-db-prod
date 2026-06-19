using Microsoft.EntityFrameworkCore;
using ApiHrm.Domains.Entities;

namespace ApiHrm.Infrastructures.Persistence.Seeders
{
    public static class TestDataSeeder
    {
        public static async Task SeedTestDataAsync(hrmAppDbContext context)
        {
            // Seed unregistered employees (no ErpUserId) - only if they don't exist
            if (!await context.Employees.AnyAsync(e => e.FirstName == "John" && e.LastName == "Doe"))
            {
                var unregisteredEmployees = new List<Employee>
                {
                    new Employee
                    {
                        ErpUserId = null,
                        FirstName = "John",
                        LastName = "Doe",
                        MiddleName = "A",
                        DateOfBirth = new DateOnly(1990, 5, 15),
                        Gender = "Male",
                        CivilStatus = "Single",
                        AvatarIndex = 1,
                        BloodType = "O+",
                        Role_ID = 1,
                        Status = "Active",
                        EmploymentDetails = new EmploymentDetails
                        {
                            Department = "Engineering",
                            Position = "Software Developer",
                            HireDate = new DateOnly(2024, 1, 10),
                            BasePay = 50000,
                            EmploymentStatus = "Active"
                        },
                        ContactInformation = new ContactInformation
                        {
                            EmailAddress = "john.doe@example.com",
                            PhoneNumber = "09123456789",
                            PresentAddress = "123 Main St, Manila"
                        }
                    },
                    new Employee
                    {
                        ErpUserId = null,
                        FirstName = "Jane",
                        LastName = "Smith",
                        MiddleName = "B",
                        DateOfBirth = new DateOnly(1992, 8, 20),
                        Gender = "Female",
                        CivilStatus = "Married",
                        AvatarIndex = 2,
                        BloodType = "A+",
                        Role_ID = 1,
                        Status = "Active",
                        EmploymentDetails = new EmploymentDetails
                        {
                            Department = "Marketing",
                            Position = "Marketing Specialist",
                            HireDate = new DateOnly(2024, 2, 15),
                            BasePay = 45000,
                            EmploymentStatus = "Active"
                        },
                        ContactInformation = new ContactInformation
                        {
                            EmailAddress = "jane.smith@example.com",
                            PhoneNumber = "09198765432",
                            PresentAddress = "456 Oak Ave, Quezon City"
                        }
                    },
                    new Employee
                    {
                        ErpUserId = "8a6e8970-05e7-403f-b769-0e6c1dec761b", // Registered employee
                        FirstName = "Admin",
                        LastName = "User",
                        MiddleName = null,
                        DateOfBirth = new DateOnly(1985, 1, 1),
                        Gender = "Male",
                        CivilStatus = "Single",
                        AvatarIndex = 0,
                        BloodType = "AB+",
                        Role_ID = 1,
                        Status = "Active",
                        EmploymentDetails = new EmploymentDetails
                        {
                            Department = "IT",
                            Position = "System Administrator",
                            HireDate = new DateOnly(2023, 6, 1),
                            BasePay = 60000,
                            EmploymentStatus = "Active"
                        },
                        ContactInformation = new ContactInformation
                        {
                            EmailAddress = "admin@r3b2p.com",
                            PhoneNumber = "09111111111",
                            PresentAddress = "789 Tech Blvd, Makati"
                        }
                    }
                };

                context.Employees.AddRange(unregisteredEmployees);
            }

            // Seed applicants - only if they don't exist by name
            if (!await context.Applicants.AnyAsync(a => a.First_Name == "Michael" && a.Last_Name == "Johnson"))
            {
                var applicants = new List<Applicant>
                {
                    new Applicant
                    {
                        First_Name = "Michael",
                        Last_Name = "Johnson",
                        Position = "Senior Developer",
                        Email = "michael.johnson@example.com",
                        Phone = "09234567890",
                        Status = "Pending",
                        Hiring_Stage = "Initial Screening",
                        Source = "Website Portal",
                        Resume_URL = "https://example.com/resumes/michael.pdf",
                        Application_Date = new DateOnly(2024, 6, 15)
                    },
                    new Applicant
                    {
                        First_Name = "Sarah",
                        Last_Name = "Williams",
                        Position = "UX Designer",
                        Email = "sarah.williams@example.com",
                        Phone = "09234567891",
                        Status = "Interviewed",
                        Hiring_Stage = "Technical Interview",
                        Source = "Gmail Submission",
                        Resume_URL = "https://example.com/resumes/sarah.pdf",
                        Application_Date = new DateOnly(2024, 6, 18)
                    },
                    new Applicant
                    {
                        First_Name = "David",
                        Last_Name = "Brown",
                        Position = "Project Manager",
                        Email = "david.brown@example.com",
                        Phone = "09234567892",
                        Status = "Pending",
                        Hiring_Stage = "Initial Screening",
                        Source = "Website Portal",
                        Resume_URL = "https://example.com/resumes/david.pdf",
                        Application_Date = new DateOnly(2024, 6, 19)
                    }
                };

                context.Applicants.AddRange(applicants);
            }

            await context.SaveChangesAsync();
        }
    }
}
