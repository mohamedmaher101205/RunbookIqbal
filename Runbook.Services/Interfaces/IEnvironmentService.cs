using System;
using System.Collections.Generic;
using System.Text;
using Runbook.Models;

namespace Runbook.Services.Interfaces
{
    public interface IEnvironmentService
    {
        int CreateEnvironment(Environments env, int tenantId);

        IEnumerable<Environments> GetAllEnvironments(int tenantId);
    }
}
