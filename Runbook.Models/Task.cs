using System;

namespace Runbook.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Task
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int TaskId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int StageId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string StageName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string TaskName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public DateTime CompletedByDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string AssignedTo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int StatusId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ReleaseNote { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Subscribers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int TenantId { get; set; }
    }
}