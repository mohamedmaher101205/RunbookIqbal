namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate environments and data mapping
    /// </summary>
    public class Environments
    {
        /// <summary>
        /// Gets or sets the book id of the environments
        /// </summary>
        /// <value>BookId</value>
        public int BookId { get; set; }
        /// <summary>
        /// Gets or sets the environment id of the environments
        /// </summary>
        /// <value>EnvId</value>
        public int EnvId { get; set; }
        /// <summary>
        /// Gets or sets the environment of the environments
        /// </summary>
        /// <value>Environment</value>
        public string Environment { get; set; }
        /// <summary>
        /// Gets or sets the status id of the environments
        /// </summary>
        /// <value>StatusId</value>
        public int StatusId { get; set; }
        /// <summary>
        /// Gets or sets the tenant id of the environments
        /// </summary>
        /// <value>TenantId</value>
        public int TenantId { get; set; }
    }
}