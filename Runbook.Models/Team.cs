namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate team entity data mapping
    /// </summary>
    public class Team
    {
        /// <summary>
        /// Gets or sets the team id of the Team
        /// </summary>
        /// <value>TeamId</value>
        public int TeamId { get; set; }

        /// <summary>
        /// Gets or sets the team name of the Team
        /// </summary>
        /// <value>TeamName</value>
        public string TeamName { get; set; }

        /// <summary>
        /// Gets or sets the team description of the team
        /// </summary>
        /// <value>Description</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tenant id of the Team
        /// </summary>
        /// <value>TenantId</value>
        public int TenantId { get; set; }

        /// <summary>
        /// Gets or sets the status of the Team
        /// </summary>
        /// <value>Status</value>
        public bool Status { get; set; }
    }
}