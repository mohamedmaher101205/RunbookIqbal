namespace Runbook.Models
{
    public class Stage
    {
        public int StageId { get; set; }
        public int BookId { get; set; }
        public string StageName { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public int EnvId { get; set; }
    }
}