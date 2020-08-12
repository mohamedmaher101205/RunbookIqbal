using System.Collections.Generic;

namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate application and data mapping
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Gets or sets the book id of the application.
        /// </summary>
        /// <value>BookId</value>
        public int BookId { get; set; }
        /// <summary>
        /// Gets or sets the application id of the application.
        /// </summary>
        /// <value>AppId</value>
        public int AppId { get; set; }
        /// <summary>
        ///  Gets or sets the application name of the application.
        /// </summary>
        /// <value>ApplicationName</value>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Gets or sets the description of the application.
        /// </summary>
        /// <value>Description</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the applcation type  of the application.
        /// </summary>
        /// <value>AppTypeName</value>
        public string AppTypeName { get; set; }
        /// <summary>
        /// Gets or sets the tenant id of the application.
        /// </summary>
        /// <value>TenantId</value>
        public int TenantId { get; set; }
        /// <summary>
        /// Gets or sets the resources list of the application.
        /// </summary>
        /// <returns>resourse list</returns>
        public List<Resource> Resources { get; set; } = new List<Resource>();
    }
}