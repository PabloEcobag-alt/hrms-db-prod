namespace Api.Contracts.CashAdvance
{
    public class CashAdvanceReadDto
    {
        public int Id { get; set; }
        public int Employee_Id { get; set; }
        public decimal Amount_Requested { get; set; }
        public string Reason { get; set; } = default!;
        public string Status { get; set; } = default!;
    }
}
