using System.Collections;
using System;
using System.Collections.Generic;

namespace RunbookAPI.Models
{
    public class ApplicationType
    {
        public int AppTypeId { get; set; }
        public string AppTypeName { get; set; }
        public int TenantId { get; set; }
    }
}