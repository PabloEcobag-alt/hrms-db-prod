namespace Api.Contracts.Checklist
{
    public class ChecklistUpdateDto
    {
        public int Checklist_ID { get; set; }
        public bool? Has_NBI { get; set; }
        public bool? Has_Medical { get; set; }
        public bool? Has_Xray { get; set; }
        public bool? has_SSS { get; set; }
        public bool? has_PAGIBIG { get; set; }
        public bool? has_PhilHealth { get; set; }
        public bool? has_TIN { get; set; }
    }
}