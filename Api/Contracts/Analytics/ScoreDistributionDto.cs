namespace Api.Contracts.Analytics
{
    public class ScoreDistributionDto
    {
        public List<ScoreBucketDto> Buckets { get; set; } = new();
    }
}
