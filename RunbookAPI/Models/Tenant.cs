using System.Collections;
using System;
using System.Collections.Generic;

namespace RunbookAPI.Models
{
    public class Tenant
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string Domain { get; set; }
    }
}