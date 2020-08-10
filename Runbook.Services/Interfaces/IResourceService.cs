using System;
using System.Collections.Generic;
using System.Text;
using Runbook.Models;

namespace Runbook.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResourceService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        int CreateResourceType(ResourceType resourceType, int tenantId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        IEnumerable<ResourceType> GetResourceTypes(int tenantId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        int CreateResource(Resource resource, int tenantId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        IEnumerable<Resource> GetAllResources(int tenantId);
    }
}
