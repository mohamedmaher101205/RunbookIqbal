using System;

namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate authenticate request and data mapping
    /// </summary>
    public class AuthRequest
    {
        /// <summary>
        /// Gets or sets the token of the AuthRequest
        /// </summary>
        /// <value>Token</value>
        public string Token { get; set; }
        /// <summary>
        /// Gets or sets the expire date of the AuthRequest
        /// </summary>
        /// <value>ExpiresIn</value>
        public DateTime ExpiresIn { get; set; }
    }
}