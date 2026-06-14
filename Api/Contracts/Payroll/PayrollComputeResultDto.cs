namespace Api.Contracts.Payroll
{
    public class PayrollComputeResultDto
    {
        public int Employee_Id { get; set; }
        public DateOnly Cutoff_Date { get; set; }
        public int? Payroll_Run_Id { get; set; }
        public decimal Daily_Rate { get; set; }
        public int Days_Worked { get; set; }
        public decimal Basic_Pay { get; set; }
        public decimal OT_Hours { get; set; }
        public decimal OT_Pay { get; set; }
        public decimal Sss_Deduction { get; set; }
        public decimal PhilHealth_Deduction { get; set; }
        public decimal PagIbig_Deduction { get; set; }
        public decimal Tax_Deduction { get; set; }
        public decimal Total_Deductions { get; set; }
        public decimal Net_Pay { get; set; }
        public List<PayrollLineItemDto> Line_Items { get; set; } = new();
    }
}
