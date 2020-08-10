namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate group for data mapping
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Gets or sets the group id  of the Group
        /// </summary>
        /// <value>GroupId</value>
        public int GroupId { get; set; }
        /// <summary>
        /// Gets or sets the tenant id  of the Group
        /// </summary>
        /// <value>TenantId</value>
        public int TenantId { get; set; }
        /// <summary>
        /// Gets or sets the group name of the Group
        /// </summary>
        /// <value>GroupName</value>
        public string GroupName { get; set; }
        /// <summary>
        /// Gets or sets the description  of the Group
        /// </summary>
        /// <value>Description</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the permissions id  of the Group
        /// </summary>
        /// <value>Array of PermissionIds</value>
        public int[] PermissionIds { get; set; }
    }
}