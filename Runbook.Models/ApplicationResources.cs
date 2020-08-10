namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate application resource and data mapping
    /// </summary>
    public class ApplicationResources
    {
        /// <summary>
        /// Gets or sets the application id of the application resource
        /// </summary>
        /// <value>AppId</value>
        public int AppId { get; set; }
        /// <summary>
        /// Gets or sets the resource id of the application resource
        /// </summary>
        /// <value>ResourceId</value>
        public int ResourceId { get; set; }
        /// <summary>
        /// Gets or sets the tenant id of the application resource
        /// </summary>
        /// <value>TenantId</value>
        public int TenantId { get; set; }
    }
}