using System.Collections.Generic;

namespace Runbook.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class User
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string FirstName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string LastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string UserEmail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int TenantId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string Salt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public List<string> Permissions { get; set; }
    }
}