using System.Collections.Generic;

namespace Runbook.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public int TenantId { get; set; }
        public string Salt { get; set; }
        public bool IsAdmin { get; set; }
        public List<string> Permissions { get; set; }
    }
}