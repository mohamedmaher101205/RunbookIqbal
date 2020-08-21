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

        public string TenantId {get;set;}

        public bool Accepted {get;set;}

        public string UserName {get;set;}

        public string  UserId {get;set;}


    }
}