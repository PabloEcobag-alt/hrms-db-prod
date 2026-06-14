namespace Api.Contracts.Payroll
{
    public class PayrollLineItemDto
    {
        public string Name { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Type { get; set; } = default!;
    }
}
