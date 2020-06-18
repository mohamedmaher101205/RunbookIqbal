using System.Collections;
using System;
using System.Collections.Generic;

namespace RunbookAPI.Models
{
    public class AuthRequest
    {
        public string Token { get; set; }
        public DateTime ExpiresIn { get; set; }
    }
}