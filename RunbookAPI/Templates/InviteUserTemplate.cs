namespace Runbook.API.Templates
{
    /// <summary>
    /// This class is to invite the user to register
    /// </summary>
    public static class InviteUserTemplate
    {
        /// <summary>
        /// email template to invite to register the user
        /// </summary>
        public static string emailTemplate = @"<section>
    <p>Hi,</p> 
    <p>{UserName} has been invited for the {InviteRoleLevel} level of the RunBook application.</p> 
    <p>Click on the below link to register</p> 
    <p>
        <a href='https://runbook-dev.azurewebsites.net/signup'>
            https://runbook-dev.azurewebsites.net/signup
        </a>
    </p>
</section>";
    }
}
