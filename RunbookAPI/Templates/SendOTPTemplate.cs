namespace Runbook.API.Templates
{
    /// <summary>
    /// This class is to invite the user to register
    /// </summary>
    public static class OneTimePasswordTemplate
    {
        /// <summary>
        /// email template to invite to register the user
        /// </summary>
        public static string emailTemplate = @"<section>

    <p>Hi,</p> 

    <p>We have created One-time password for this email account.</p> 

    <p>Please sign in to the account with this password and set a new, permanent password</p> 

    <p>

      <h3>   One-time password : {OTP}  </h3>

    </p>

</section>";
    }
}