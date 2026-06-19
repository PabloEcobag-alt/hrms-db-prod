using Microsoft.EntityFrameworkCore;
using ApiHrm.Domains.Entities;
using ApiHrm.Infrastructures.Persistence;

namespace ApiHrm.Infrastructures.Persistence.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(hrmAppDbContext context)
        {
            // Check if roles already exist
            var existingRoles = await context.Roles.ToListAsync();
            if (existingRoles.Count > 0)
            {
                return; // Skip seeding if roles already exist
            }

            var roles = new List<Role>
            {
                new Role { Role_Description = "Admin" },
                new Role { Role_Description = "Manager" },
                new Role { Role_Description = "On Call" },
                new Role { Role_Description = "Merchandiser On Call" },
                new Role { Role_Description = "Store Attendant" },
                new Role { Role_Description = "Commissary Helper" },
                new Role { Role_Description = "Head Cook" },
                new Role { Role_Description = "Assistant Cook" },
                new Role { Role_Description = "Inventory Manager" },
                new Role { Role_Description = "Supply Chain & Marketing Manager" },
                new Role { Role_Description = "Logistics and Order Manager" },
                new Role { Role_Description = "OJT" },
                new Role { Role_Description = "Intern/Summer Job" }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }
    }
}
