using System.Collections;
using System;
using System.Collections.Generic;

namespace RunbookAPI.Models
{
    public class ResourceType
    {
        public int ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
        public int TenantId { get; set; }
    }
}