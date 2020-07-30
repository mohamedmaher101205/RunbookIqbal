﻿namespace Runbook.API.Templates
{
    public static class InviteUserTemplate
    {
        public static string emailTemplate = @"<section>
    <p>Hi,</p> 
    <p>You have been invited for the RunBook application.</p> 
    <p>Click on the below link to register</p> 
    <p>
        <a href='https://runbook-dev.azurewebsites.net/signup'>
            https://runbook-dev.azurewebsites.net/signup
        </a>
    </p>
</section>";
    }
}
