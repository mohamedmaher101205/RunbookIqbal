using System.Collections;
using System;
using System.Collections.Generic;

namespace RunbookAPI.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string Description { get; set; }
        public int ResourceTypeId { get; set; }
        public int TenantId { get; set; }
        public int AppId { get; set; }
    }
}