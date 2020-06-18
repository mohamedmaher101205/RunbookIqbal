using System.Collections;
using System;
using System.Collections.Generic;

namespace RunbookAPI.Models
{
    public class GroupUser
    {
        public int GroupUserId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }
}