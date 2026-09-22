namespace Api.Contracts.Analytics
{
    public class HrmsDashboardSummaryDto
    {
        public int TotalEmployees { get; set; }
        public int RegularEmployees { get; set; }
        public int TotalApplicants { get; set; }
        public int ActiveApplicants { get; set; }
        public int TotalAttendanceRecords { get; set; }
        public int OnTimeCount { get; set; }
        public decimal TotalPayroll { get; set; }
    }
}
