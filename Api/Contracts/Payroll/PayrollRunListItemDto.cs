namespace Api.Contracts.Payroll
{
    public class PayrollRunListItemDto
    {
        public int Id { get; set; }
        public int Employee_Id { get; set; }
        public string Employee_Name { get; set; } = default!;
        public string Position { get; set; } = default!;
        public decimal Basic_Pay { get; set; }
        public decimal OT_Pay { get; set; }
        public decimal Sss_Deduction { get; set; }
        public decimal PhilHealth_Deduction { get; set; }
        public decimal PagIbig_Deduction { get; set; }
        public decimal Tax { get; set; }
        public decimal Bonus { get; set; }
        public decimal Net_Pay { get; set; }
        public string Status { get; set; } = default!;
        public string Payout_Method { get; set; } = default!;
    }
}
