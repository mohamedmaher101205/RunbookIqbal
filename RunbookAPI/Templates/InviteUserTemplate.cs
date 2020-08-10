namespace Runbook.API.Templates
{
    public static class EmailTemplate
    {
        public static string InviteUserTemplate()
        {
            string emailTemplate = @"<section>
    <p>Hi,</p> 
    <p>You have been invited for the RunBook application.</p> 
    <p>Click on the below link to register</p> 
    <p>
        <a href='https://runbook-dev.azurewebsites.net/signup'>
            https://runbook-dev.azurewebsites.net/signup
        </a>
    </p>
</section>";
            return emailTemplate;
        }
        public static  string OneTimePasswordTemplate(string OTP)
        {
            string emailTemplate = @"<section>
    <p>Hi,</p> 
    <p>We have created One-time password for this email account.</p> 
    <p>Please sign in to the account with this password and set a new, permanent password</p> 
    <p>
      <h3>   One-time password : {OTP}  </h3>
    </p>
</section>";

            return emailTemplate;
        }
    }
}
