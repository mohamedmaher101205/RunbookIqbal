using System;

namespace Runbook.Models
{
    public class AuthRequest
    {
        public string Token { get; set; }
        public DateTime ExpiresIn { get; set; }
    }
}