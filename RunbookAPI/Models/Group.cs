using System;

namespace RunbookAPI.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public int TenantId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
    }
}