using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    /// <summary>
    /// This ResourceService class have methods to performing create a resource, create a resource type
    /// get all resource,get all resource type
    /// </summary>
    public class ResourceService : IResourceService
    {
        private readonly IDbConnection _Idbconnection;

        /// <summary>
        /// This constructor is to inject IDBConnection using constructor dependency injuction
        /// </summary>
        /// <param name="dbConnection"></param>
        public ResourceService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        /// <summary>
        /// create a resource
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="tenantId"></param>
        /// <returns>integer value</returns>
        public int CreateResourceType(ResourceType resourceType, int tenantId)
        {
            try
            {
                string resourceTypeCmd = @"INSERT INTO [dbo].[UserDefinedResourceTypes](ResourceTypeName,TenantId)
	                                        VALUES (@ResourceTypeName,@TenantId)";

                int insertedResourceType = 0;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    insertedResourceType = con.Execute(resourceTypeCmd,
                        new
                        {
                            ResourceTypeName = resourceType.ResourceTypeName,
                            TenantId = tenantId
                        });
                    con.Close();
                }
                return insertedResourceType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Read all resource list
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of resource type</returns>
        public IEnumerable<ResourceType> GetResourceTypes(int tenantId)
        {
            try
            {
                string resourceTypesCmd = @"SELECT * FROM [dbo].[UserDefinedResourceTypes] 
                                            WHERE TenantId = @TenantId";

                IEnumerable<ResourceType> resourceTypes = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    resourceTypes = con.Query<ResourceType>(resourceTypesCmd, new { TenantId = tenantId });
                    con.Close();
                }

                return resourceTypes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// create a resource
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="tenantId"></param>
        /// <returns>integer value</returns>
        public int CreateResource(Resource resource, int tenantId)
        {
            try
            {
                string resourceCmd = @"INSERT INTO [dbo].[Resources](ResourceName,Description,ResourceTypeId,TenantId)
	                                    VALUES(@ResourceName,@Description,@ResourceTypeId,@TenantId)";

                int insertedResource = 0;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    insertedResource = con.Execute(resourceCmd, new
                    {
                        ResourceName = resource.ResourceName,
                        Description = resource.Description,
                        ResourceTypeId = resource.ResourceTypeId,
                        TenantId = tenantId
                    });
                    con.Close();
                }

                return insertedResource;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Read all resource list
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of resource</returns>
        public IEnumerable<Resource> GetAllResources(int tenantId)
        {
            try
            {
                string resourcesCmd = @"SELECT * FROM [dbo].[Resources] WHERE TenantId = @TenantId";

                IEnumerable<Resource> resources = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    resources = con.Query<Resource>(resourcesCmd, new { TenantId = tenantId });
                    con.Close();
                }

                return resources;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
