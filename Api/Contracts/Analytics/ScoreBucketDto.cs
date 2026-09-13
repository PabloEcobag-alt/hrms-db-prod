namespace Api.Contracts.Analytics
{
    public class ScoreBucketDto
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }
}
