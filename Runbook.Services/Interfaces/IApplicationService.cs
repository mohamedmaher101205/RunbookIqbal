using Runbook.Models;
using System.Collections.Generic;

namespace Runbook.Services.Interfaces
{
    public interface IApplicationService
    {
        bool CreateApplication(Application app, int tenantId);

        IEnumerable<Application> GetAllApplications(int tenantId);

        IEnumerable<ApplicationType> GetApplicationTypes(int tenantId);

        int AddApplications(int bookId, int[] appIds);

        IEnumerable<Application> GetApplicationByBookId(int bookId);

        int CreateCustomApplicationType(ApplicationType appType, int tenantId);
    }
}