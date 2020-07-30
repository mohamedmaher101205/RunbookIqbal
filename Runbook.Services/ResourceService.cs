using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IDbConnection _Idbconnection;

        public ResourceService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

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
