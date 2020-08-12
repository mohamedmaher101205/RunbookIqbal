namespace Runbook.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Stage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int StageId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int BookId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string StageName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int StatusId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int EnvId { get; set; }
    }
}