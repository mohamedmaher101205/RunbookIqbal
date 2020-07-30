using System;

namespace Runbook.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public int StageId { get; set; }
        public string StageName { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime CompletedByDate { get; set; }
        public string AssignedTo { get; set; }
        public int StatusId { get; set; }
        public string ReleaseNote { get; set; }
    }
}