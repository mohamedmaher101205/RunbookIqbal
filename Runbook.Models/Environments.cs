namespace Runbook.Models
{
    public class Environments
    {
        public int BookId { get; set; }
        public int EnvId { get; set; }
        public string Environment { get; set; }
        public int StatusId { get; set; }
        public int TenantId { get; set; }
    }
}