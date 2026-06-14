using Microsoft.EntityFrameworkCore;
using ApiHrm.Domains.Entities;

namespace ApiHrm.Infrastructures.Persistence.Seeders
{
    public static class StatutoryDataSeeder
    {
        public static async Task SeedStatutoryDataAsync(hrmAppDbContext context)
        {
            // Check if 2024 data already exists for any statutory table
            if (await context.SssBrackets.AnyAsync(b => b.EffectiveYear == 2024) ||
                await context.PhilHealthBrackets.AnyAsync(b => b.EffectiveYear == 2024) ||
                await context.PagIbigBrackets.AnyAsync(b => b.EffectiveYear == 2024) ||
                await context.TaxBrackets.AnyAsync(b => b.EffectiveYear == 2024))
            {
                return; // Skip seeding if 2024 data already exists
            }

            // Seed SSS Brackets (2024 rates: 4.5% employee, 9.5% employer, MSC 30,000)
            // Dynamically generate brackets in 500-peso intervals
            const decimal sssEmployeeRate = 0.045m; // 4.5%
            const decimal sssEmployerRate = 0.095m; // 9.5%
            const decimal sssMsc = 30000m;
            const decimal sssInterval = 500m;

            for (decimal lower = 0; lower < sssMsc; lower += sssInterval)
            {
                decimal upper = lower + sssInterval;
                if (upper > sssMsc) upper = sssMsc;

                var monthlyContribution = (upper * sssEmployeeRate) + (upper * sssEmployerRate);
                var employeeShare = upper * sssEmployeeRate;
                var employerShare = upper * sssEmployerRate;

                context.SssBrackets.Add(new SssBracket
                {
                    SalaryRangeStart = lower,
                    SalaryRangeEnd = upper,
                    EmployeeShareRate = sssEmployeeRate,
                    EmployerShareRate = sssEmployerRate,
                    MonthlyContribution = monthlyContribution,
                    EffectiveYear = 2024
                });
            }

            // Seed PhilHealth Brackets (2024: 5% premium, 10k floor, 100k ceiling)
            // Scenario 1: Below 10,000 - Fixed 500 (250/250)
            context.PhilHealthBrackets.Add(new PhilHealthBracket
            {
                SalaryRangeStart = 0,
                SalaryRangeEnd = 9999.99m,
                EmployeeShareRate = 0.025m, // 2.5%
                EmployerShareRate = 0.025m, // 2.5%
                MonthlyContribution = 500m,
                EffectiveYear = 2024
            });

            // Scenario 2: 10,000 to 100,000 - 5% (2.5%/2.5%)
            context.PhilHealthBrackets.Add(new PhilHealthBracket
            {
                SalaryRangeStart = 10000m,
                SalaryRangeEnd = 100000m,
                EmployeeShareRate = 0.025m, // 2.5%
                EmployerShareRate = 0.025m, // 2.5%
                MonthlyContribution = 0m, // Calculated dynamically based on salary
                EffectiveYear = 2024
            });

            // Scenario 3: Above 100,000 - Fixed 5,000 (2,500/2,500)
            context.PhilHealthBrackets.Add(new PhilHealthBracket
            {
                SalaryRangeStart = 100000.01m,
                SalaryRangeEnd = 999999999.99m,
                EmployeeShareRate = 0.025m, // 2.5%
                EmployerShareRate = 0.025m, // 2.5%
                MonthlyContribution = 5000m,
                EffectiveYear = 2024
            });

            // Seed Pag-IBIG Brackets (2024: 1%/2% below 1,500, 2%/2% above, MSC 10,000)
            // Scenario 1: 0 to 1,500 - Employee 1%, Employer 2%
            context.PagIbigBrackets.Add(new PagIbigBracket
            {
                SalaryRangeStart = 0,
                SalaryRangeEnd = 1500m,
                EmployeeShareRate = 0.01m, // 1%
                EmployerShareRate = 0.02m, // 2%
                MonthlyContribution = 0m, // Calculated dynamically
                EffectiveYear = 2024
            });

            // Scenario 2: 1,500.01 to 10,000 - Employee 2%, Employer 2%
            context.PagIbigBrackets.Add(new PagIbigBracket
            {
                SalaryRangeStart = 1500.01m,
                SalaryRangeEnd = 10000m,
                EmployeeShareRate = 0.02m, // 2%
                EmployerShareRate = 0.02m, // 2%
                MonthlyContribution = 0m, // Calculated dynamically
                EffectiveYear = 2024
            });

            // Seed Tax Brackets (TRAIN Law 2024 - Annual)
            context.TaxBrackets.Add(new TaxBracket
            {
                AnnualRangeStart = 0,
                AnnualRangeEnd = 250000m,
                TaxRate = 0m, // 0%
                BaseTax = 0m,
                EffectiveYear = 2024
            });

            context.TaxBrackets.Add(new TaxBracket
            {
                AnnualRangeStart = 250000.01m,
                AnnualRangeEnd = 400000m,
                TaxRate = 0.15m, // 15%
                BaseTax = 0m,
                EffectiveYear = 2024
            });

            context.TaxBrackets.Add(new TaxBracket
            {
                AnnualRangeStart = 400000.01m,
                AnnualRangeEnd = 800000m,
                TaxRate = 0.20m, // 20%
                BaseTax = 22500m,
                EffectiveYear = 2024
            });

            context.TaxBrackets.Add(new TaxBracket
            {
                AnnualRangeStart = 800000.01m,
                AnnualRangeEnd = 2000000m,
                TaxRate = 0.25m, // 25%
                BaseTax = 102500m,
                EffectiveYear = 2024
            });

            context.TaxBrackets.Add(new TaxBracket
            {
                AnnualRangeStart = 2000000.01m,
                AnnualRangeEnd = 8000000m,
                TaxRate = 0.30m, // 30%
                BaseTax = 402500m,
                EffectiveYear = 2024
            });

            context.TaxBrackets.Add(new TaxBracket
            {
                AnnualRangeStart = 8000000.01m,
                AnnualRangeEnd = 999999999.99m,
                TaxRate = 0.35m, // 35%
                BaseTax = 2202500m,
                EffectiveYear = 2024
            });

            await context.SaveChangesAsync();
        }
    }
}
