using System.Collections;
using System;
using System.Collections.Generic;

namespace RunbookAPI.Models
{
    public class Application
    {
        public int BookId { get; set; }
        public int AppId { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }
        public string AppTypeName { get; set; }
        public int TenantId { get; set; }
        public List<Resource> Resources {get;set;} = new List<Resource>();
    }
}