namespace Api.Contracts.Digital201
{
    public class LinkErpUserRequest
    {
        public int EmployeeId { get; set; }
        public string ErpUserId { get; set; } = string.Empty;
    }
}
