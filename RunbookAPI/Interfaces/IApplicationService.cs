using System.Collections;
using System.Collections.Generic;
namespace RunbookAPI.Models
{
    public interface IApplicationService
    {
        bool CreateApplication(Application app,int tenantId);

        IEnumerable<Application> GetAllApplications(int tenantId);

        IEnumerable<ApplicationType> GetApplicationTypes(int tenantId);

        int AddApplications(int bookId,int[] appIds);

        IEnumerable<Application> GetApplicationByBookId(int bookId);

        int CreateCustomApplicationType(ApplicationType appType,int tenantId);

        int CreateResourceType(ResourceType resourceType,int tenantId);

        IEnumerable<ResourceType> GetResourceTypes(int tenantId);

        int CreateResource(Resource resource,int tenantId);

        IEnumerable<Resource> GetAllResources(int tenantId);

    }
}