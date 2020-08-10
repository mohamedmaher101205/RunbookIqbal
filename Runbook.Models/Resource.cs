namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate resource for data mapping
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Gets or sets the resourceid  of the resource
        /// </summary>
        /// <value>resourceId</value>
        public int ResourceId { get; set; }
        /// <summary>
        /// Gets or sets the resource name  of the resource
        /// </summary>
        /// <value>resourceName</value>
        public string ResourceName { get; set; }
        /// <summary>
        /// Gets or sets the description  of the resource
        /// </summary>
        /// <value>Description</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the resourcetype id  of the resource
        /// </summary>
        /// <value>resourceTypeId</value>
        public int ResourceTypeId { get; set; }
        /// <summary>
        /// Gets or sets the tenant id  of the resource
        /// </summary>
        /// <value>TenantId</value>
        public int TenantId { get; set; }
        /// <summary>
        /// Gets or sets the application id  of the resource
        /// </summary>
        /// <value>AppId</value>
        public int AppId { get; set; }
    }
}