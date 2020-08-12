namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate Resource Type for data mapping
    /// </summary>
    public class ResourceType
    {
        /// <summary>
        /// Gets or sets the resurcetype id   of the resource type
        /// </summary>
        /// <value>ResourceTypeId</value>
        public int ResourceTypeId { get; set; }
        /// <summary>
        /// Gets or sets the resoursetype name of the resource type
        /// </summary>
        /// <value>ResourceTypeName</value>
        public string ResourceTypeName { get; set; }
        /// <summary>
        /// Gets or sets the tenant id of the resource type
        /// </summary>
        /// <value>TenantId</value>
        public int TenantId { get; set; }
    }
}