namespace Api.Contracts.RoleExit
{
    public class ExitReadDto
    {
        public int Exit_ID { get; set; }
        public int Employee_Id { get; set; }
        public string EmployeeFullName { get; set; } = default!;
        public DateTime Date{ get; set; }
        public string Reason_For_Leaving { get; set; } = default!;
        public string Interviewed_By { get; set; } = default!;
    }
}