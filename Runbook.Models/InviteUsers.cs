namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate permissions for data mapping
    /// </summary>
    public class InviteUsers
    {

        public string InviteUserEmailId { get; set; }

        public string InviteUrl { get; set; }

        public string InviteRoleLevel { get; set; }

        public string InviteTenanteLevel {get;set;}

        public bool InviteUserStatus {get;set;}

        public string UserName {get;set;}


    }
}