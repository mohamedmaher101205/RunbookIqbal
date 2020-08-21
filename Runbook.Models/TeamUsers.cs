namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate Team user for data mapping
    /// </summary>
    public class TeamUser
    {
        /// <summary>
        /// Gets or sets the Team users
        /// </summary>
        /// <value>TeamUserId</value>
        public int TeamUserId { get; set; }

        /// <summary>
        /// Gets or sets the Team id
        /// </summary>
        /// <value>TeamId</value>
        public int TeamId { get; set; }

        /// <summary>
        /// Gets or sets the user id of the Team User
        /// </summary>
        /// <value>UserId</value>
        public int UserId { get; set; }
    }
}