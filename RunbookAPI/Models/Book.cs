using System.Collections;
using System;
using System.Collections.Generic;

namespace RunbookAPI.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public DateTime TargetedDate { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public int TenantId { get; set; }
        public List<Environments> Environments { get; set; } = new List<Environments>();
        public List<Application> Applications { get; set; } = new List<Application>();
    }
}