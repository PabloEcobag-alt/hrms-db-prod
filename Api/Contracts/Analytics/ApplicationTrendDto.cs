namespace Api.Contracts.Analytics
{
    public class ApplicationTrendDto
    {
        public string Date { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
