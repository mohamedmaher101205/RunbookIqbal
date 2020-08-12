namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulte group permissions for data mapping
    /// </summary>
    public class GroupPermissions
    {
        /// <summary>
        /// Gets or sets the group permission id of the Group Permissions
        /// </summary>
        /// <value>GroupPermissionId</value>
        public int GroupPermissionId { get; set; }
        /// <summary>
        /// Gets or sets the group id  of the Group Permissions
        /// </summary>
        /// <value>GroupId</value>
        public int GroupId { get; set; }
        /// <summary>
        /// Gets or sets the permission id  of the Group Permissions
        /// </summary>
        /// <value>PermissionId</value>
        public int PermissionId { get; set; }
    }
}