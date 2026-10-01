using Microsoft.EntityFrameworkCore;
using ApiHrm.Domains.Entities;

namespace ApiHrm.Infrastructures.Persistence.Seeders
{
    public static class TestDataSeeder
    {
        public static async Task SeedTestDataAsync(hrmAppDbContext context)
        {
            // Seed unregistered employees (no ErpUserId) - only if they don't exist
            if (!await context.Employees.AnyAsync(e => e.FirstName == "Alice" && e.LastName == "SuperAdmin"))
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
                    },
                    new Employee
                    {
                        ErpUserId = null,
                        FirstName = "Alice",
                        LastName = "SuperAdmin",
                        MiddleName = null,
                        DateOfBirth = new DateOnly(1980, 5, 10),
                        Gender = "Female",
                        CivilStatus = "Married",
                        AvatarIndex = 3,
                        BloodType = "A+",
                        Role_ID = 1,
                        Status = "Active",
                        EmploymentDetails = new EmploymentDetails
                        {
                            Department = "Management",
                            Position = "Super Admin",
                            HireDate = new DateOnly(2023, 1, 1),
                            BasePay = 100000,
                            EmploymentStatus = "Active"
                        },
                        ContactInformation = new ContactInformation
                        {
                            EmailAddress = "alice@example.com",
                            PhoneNumber = "09171234567",
                            PresentAddress = "123 Management Blvd, BGC"
                        }
                    },
                    new Employee
                    {
                        ErpUserId = null,
                        FirstName = "Bob",
                        LastName = "Employee",
                        MiddleName = null,
                        DateOfBirth = new DateOnly(1995, 8, 15),
                        Gender = "Male",
                        CivilStatus = "Single",
                        AvatarIndex = 4,
                        BloodType = "O+",
                        Role_ID = 1,
                        Status = "Active",
                        EmploymentDetails = new EmploymentDetails
                        {
                            Department = "Operations",
                            Position = "Staff",
                            HireDate = new DateOnly(2024, 3, 1),
                            BasePay = 35000,
                            EmploymentStatus = "Active"
                        },
                        ContactInformation = new ContactInformation
                        {
                            EmailAddress = "bob@example.com",
                            PhoneNumber = "09181234567",
                            PresentAddress = "456 Operations St, Pasig"
                        }
                    },
                    new Employee
                    {
                        ErpUserId = null,
                        FirstName = "Carol",
                        LastName = "SuperAdmin",
                        MiddleName = null,
                        DateOfBirth = new DateOnly(1982, 11, 20),
                        Gender = "Female",
                        CivilStatus = "Single",
                        AvatarIndex = 5,
                        BloodType = "B+",
                        Role_ID = 1,
                        Status = "Active",
                        EmploymentDetails = new EmploymentDetails
                        {
                            Department = "Management",
                            Position = "Super Admin",
                            HireDate = new DateOnly(2023, 2, 1),
                            BasePay = 95000,
                            EmploymentStatus = "Active"
                        },
                        ContactInformation = new ContactInformation
                        {
                            EmailAddress = "carol@example.com",
                            PhoneNumber = "09191234567",
                            PresentAddress = "789 Admin Ave, Makati"
                        }
                    },
                    new Employee
                    {
                        ErpUserId = null,
                        FirstName = "Derek",
                        LastName = "Executive",
                        MiddleName = null,
                        DateOfBirth = new DateOnly(1975, 4, 5),
                        Gender = "Male",
                        CivilStatus = "Married",
                        AvatarIndex = 6,
                        BloodType = "AB+",
                        Role_ID = 1,
                        Status = "Active",
                        EmploymentDetails = new EmploymentDetails
                        {
                            Department = "Executive",
                            Position = "CEO",
                            HireDate = new DateOnly(2022, 1, 15),
                            BasePay = 250000,
                            EmploymentStatus = "Active"
                        },
                        ContactInformation = new ContactInformation
                        {
                            EmailAddress = "derek@example.com",
                            PhoneNumber = "09201234567",
                            PresentAddress = "101 Executive Village, Alabang"
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
                        Position = "Store Attendant",
                        Email = "michael.johnson@example.com",
                        Mobile = "09234567890",
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
                        Position = "Merchandiser On Call",
                        Email = "sarah.williams@example.com",
                        Mobile = "09234567891",
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
                        Position = "Commissary Helper",
                        Email = "david.brown@example.com",
                        Mobile = "09234567892",
                        Status = "Pending",
                        Hiring_Stage = "Initial Screening",
                        Source = "Website Portal",
                        Resume_URL = "https://example.com/resumes/david.pdf",
                        Application_Date = new DateOnly(2024, 6, 19)
                    }
                };

                context.Applicants.AddRange(applicants);
            }

            // --- PATCH TO FIX APPLICANTS AND REMOVE UNLINKED EMPLOYEES ---
            
            // Task 1: Fix transformed applicants
            var applicantIdsToFix = new[] { 51, 52, 53 };
            var applicantsToFix = await context.Applicants
                .Where(a => applicantIdsToFix.Contains(a.Applicant_ID))
                .ToListAsync();

            foreach (var a in applicantsToFix)
            {
                if (a.Probationary_End_Date.HasValue)
                {
                    a.Hiring_Stage = "Probationary";
                    a.Status = "Probationary";
                }
                else
                {
                    a.Hiring_Stage = "Hired";
                    a.Status = "Regular";
                }
            }

            // Task 2: Cleanup duplicate and dummy seeded employees
            var targetFirstNames = new[] { "John", "Jane", "Admin", "Alice", "Bob", "Carol", "Derek" };
            var allTargetEmployees = await context.Employees
                .Where(e => targetFirstNames.Contains(e.FirstName))
                .ToListAsync();

            var employeesToDelete = allTargetEmployees
                .Where(e => (e.FirstName == "John" && e.LastName == "Doe") ||
                            (e.FirstName == "Jane" && e.LastName == "Smith") ||
                            (e.FirstName == "Admin" && e.LastName == "User"))
                .ToList();

            // Deduplicate Alice, Bob, Carol, Derek (keep the first one)
            var duplicateNames = new[] { "Alice", "Bob", "Carol", "Derek" };
            foreach (var firstName in duplicateNames)
            {
                var duplicates = allTargetEmployees
                    .Where(e => e.FirstName == firstName)
                    .OrderBy(e => e.EmployeeId)
                    .Skip(1)
                    .ToList();
                employeesToDelete.AddRange(duplicates);
            }

            if (employeesToDelete.Any())
            {
                var employeeIdsToDelete = employeesToDelete.Select(e => e.EmployeeId).ToList();
                
                // Due to RESTRICT constraints, delete related records first
                var contactInfos = await context.ContactInformation
                    .Where(ci => employeeIdsToDelete.Contains(ci.EmployeeId))
                    .ToListAsync();
                context.ContactInformation.RemoveRange(contactInfos);

                var empDetails = await context.EmploymentDetails
                    .Where(ed => employeeIdsToDelete.Contains(ed.EmployeeId))
                    .ToListAsync();
                context.EmploymentDetails.RemoveRange(empDetails);

                context.Employees.RemoveRange(employeesToDelete);
            }
            // --- END PATCH ---

            await context.SaveChangesAsync();
        }
    }
}
