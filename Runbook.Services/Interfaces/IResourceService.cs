using System;
using System.Collections.Generic;
using System.Text;
using Runbook.Models;

namespace Runbook.Services.Interfaces
{
    public interface IResourceService
    {
        int CreateResourceType(ResourceType resourceType, int tenantId);

        IEnumerable<ResourceType> GetResourceTypes(int tenantId);

        int CreateResource(Resource resource, int tenantId);

        IEnumerable<Resource> GetAllResources(int tenantId);
    }
}
