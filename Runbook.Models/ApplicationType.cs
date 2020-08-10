namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate application type and data mapping
    /// </summary>
    public class ApplicationType
    {
        /// <summary>
        /// Gets or sets the application type id of the application type
        /// </summary>
        /// <value>AppTypeId</value>
        public int AppTypeId { get; set; }
        /// <summary>
        /// Gets or sets the application type name of the application type
        /// </summary>
        /// <value>AppTypeName</value>
        public string AppTypeName { get; set; }
        /// <summary>
        /// Gets or sets the tenant id of the application type
        /// </summary>
        /// <value>TenantId</value>
        public int TenantId { get; set; }
    }
}