namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate group user for data mapping
    /// </summary>
    public class GroupUser
    {
        /// <summary>
        /// Gets or sets the group user  of the Group User
        /// </summary>
        /// <value>GroupUserId</value>
        public int GroupUserId { get; set; }
        /// <summary>
        /// Gets or sets the group id  of the Group User
        /// </summary>
        /// <value>GroupId</value>
        public int GroupId { get; set; }
        /// <summary>
        /// Gets or sets the user id of the Group User
        /// </summary>
        /// <value>UserId</value>
        public int UserId { get; set; }
    }
}