using System;
using System.Collections.Generic;

namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate book and data mapping
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Gets or sets the book id  of the book
        /// </summary>
        /// <value>BookId</value>
        public int BookId { get; set; }
        /// <summary>
        /// Gets or sets the book name  of the book
        /// </summary>
        /// <value>BookName</value>
        public string BookName { get; set; }
        /// <summary>
        /// Gets or sets the targeted date  of the book
        /// </summary>
        /// <value>TargetedDate</value>
        public DateTime TargetedDate { get; set; }
        /// <summary>
        /// Gets or sets the user id  of the book
        /// </summary>
        /// <value>UserId</value>
        public int UserId { get; set; }
        /// <summary>
        ///  Gets or sets the description  of the book
        /// </summary>
        /// <value>Description</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the tenant id  of the book
        /// </summary>
        /// <value>TenantId</value>
        public int TenantId { get; set; }
        /// <summary>
        /// Gets or sets the environment list of the book
        /// </summary>
        /// <returns>list of environments</returns>
        public List<Environments> Environments { get; set; } = new List<Environments>();
        /// <summary>
        /// Gets or sets the application list of the book
        /// </summary>
        /// <returns>list of applications</returns>
        public List<Application> Applications { get; set; } = new List<Application>();
    }
}