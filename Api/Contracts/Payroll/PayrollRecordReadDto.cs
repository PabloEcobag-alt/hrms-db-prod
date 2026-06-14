namespace Api.Contracts.Payroll
{
    public class PayrollRecordReadDto
    {
        public int Id { get; set; }
        public int Payroll_Run_Id { get; set; }
        public int Employee_Id { get; set; }
        public decimal Basic_Pay { get; set; }
        public decimal OT_Pay { get; set; }
        public decimal Sss_Deduction { get; set; }
        public decimal PhilHealth_Deduction { get; set; }
        public decimal PagIbig_Deduction { get; set; }
        public decimal Net_Pay { get; set; }
    }
}
