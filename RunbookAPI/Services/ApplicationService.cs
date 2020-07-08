using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using RunbookAPI.Models;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace RunbookAPI.Services
{
    public class ApplicationService : IApplicationService
    {
        private IDbConnection _Idbconnection;
        
        public ApplicationService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        public bool CreateApplication(Application app,int tenantId)
        {
            try{
                string appResourcesCmd = @"INSERT INTO [dbo].[ApplicationResources](AppId,ResourceId,TenantId)
	                                    VALUES(@AppId,@ResourceId,@TenantId)";

                var appParameters = new DynamicParameters();
                appParameters.Add("@ApplicationName",app.ApplicationName);
                appParameters.Add("@Description",app.Description);
                appParameters.Add("@TenantId",tenantId);
                appParameters.Add("@CreatedAppId", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            
                string apptypecmd = @"SELECT AppTypeId FROM [dbo].[UserDefinedapplicationType] 
                                        WHERE AppTypeName = @AppTypeName AND TenantId = @TenantId";
                
                int appcreated = 0;

                List<ApplicationResources> appResources = new List<ApplicationResources>(); 

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    ApplicationType apptype = con.QueryFirstOrDefault<ApplicationType>(apptypecmd,new {AppTypeName = app.AppTypeName, TenantId = tenantId});
                    
                    appParameters.Add("@AppTypeId",apptype.AppTypeId);
                    appcreated = con.Execute("[dbo].sp_CreateApplication", appParameters, commandType: CommandType.StoredProcedure);
                    int createdAppId = appParameters.Get<int>("@CreatedAppId");
                    
                    foreach (var resType in app.Resources)
                    {
                        appResources.Add(new ApplicationResources{
                            AppId = createdAppId,
                            ResourceId = resType.ResourceId,
                            TenantId = tenantId
                        });
                    }
                    var appResourcesCreated = con.Execute(appResourcesCmd,appResources);
                    con.Close();
                }
                if(appcreated > 0){
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e){
                throw e;
            }
        }

        public IEnumerable<Application> GetAllApplications(int tenantId)
        {
            string getappscmd = @"SELECT app.[AppId], app.[ApplicationName],app.[Description],
                                    apptype.[AppTypeName],app.[TenantId] 
                                        FROM [dbo].[Application] app
                                        JOIN [dbo].[UserDefinedApplicationType] apptype ON app.[AppTypeId] = apptype.[AppTypeId] 
                                    WHERE app.TenantId = @TenantId";

            string resourcesCmd = @"SELECT res.*,appres.AppId FROM [dbo].[Resources] res
                                        JOIN [dbo].[ApplicationResources] appres on res.ResourceId = appres.ResourceId
                                    Where res.TenantId = @TenantId";

            IEnumerable<Application> apps = null;
            IEnumerable<Resource> resources = null;

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                apps = con.Query<Application>(getappscmd,new {TenantId = tenantId});
                resources = con.Query<Resource>(resourcesCmd, new {TenantId = tenantId});
                con.Close();
            }
            foreach (var app in apps)
            {
                foreach (var resource in resources)
                {
                    if(app.AppId == resource.AppId){
                        app.Resources.Add(resource);
                    }
                }
            }
            return apps;
        }

        public IEnumerable<ApplicationType> GetApplicationTypes(int tenantId)
        {
            string apptypecmd = @"SELECT * FROM [dbo].[UserDefinedapplicationType] 
                                        WHERE tenantId = @TenantID";

            IEnumerable<ApplicationType> apptypes = null;

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                apptypes = con.Query<ApplicationType>(apptypecmd,new {TenantId = tenantId});
                con.Close();
            }
            return apptypes;
        }

        public int AddApplications(int bookId,int[] appIds)
        {
            string appinsertcmd = @"INSERT INTO [dbo].[BookApplication](BookId,AppId) VALUES(@BookId,@AppId)";
            string deleteExistingApps = @"DELETE FROM [dbo].[BookApplication] WHERE BookId = @BookId";

            List<object> appAndBook = new List<object>();
            for(int i=0;i<appIds.Length;i++){
                appAndBook.Add(new {BookId = bookId,AppId=appIds[i]});
            }
            int insertedrows = 0;
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                //var sqltrans = con.BeginTransaction();
                var deletedRows = con.Execute(deleteExistingApps,new {BookId = bookId});
                insertedrows = con.Execute(appinsertcmd,appAndBook);
                con.Close();

                // if(insertedrows > 0){
                //     sqltrans.Commit();
                // }
                // else{
                //     sqltrans.Rollback();
                // }
            }
            return insertedrows;
        }

        public IEnumerable<Application> GetApplicationByBookId(int bookId)
        {
            string getapps = @"SELECT app.AppId,app.ApplicationName,app.Description,apptype.appTypeName,app.TenantId
                                FROM [dbo].[Application] app
                                JOIN [dbo].[BookApplication] bookapp ON app.AppId = bookapp.AppId
                                JOIN [dbo].[UserDefinedApplicationType] apptype ON app.[AppTypeId] = apptype.[AppTypeId]
                                WHERE bookapp.BookId = @BookId";

            IEnumerable<Application> apps = null;

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                apps = con.Query<Application>(getapps,new{BookId = bookId});
                con.Close();
            }
            return apps;
        }

        public int CreateCustomApplicationType(ApplicationType appType,int tenantId)
        {
            try{
                string appTypeCmd = @"INSERT INTO [dbo].[UserDefinedApplicationType](AppTypeName,TenantId)
                                        VALUES (@AppTypeName,@TenantId)";
                
                int insertedAppType = 0;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    insertedAppType = con.Execute(appTypeCmd,
                        new {
                            AppTypeName = appType.AppTypeName,
                            TenantId = tenantId
                        });
                    con.Close();
                }
                return insertedAppType;
            }catch(Exception ex){
                throw ex;
            }
        }

        public int CreateResourceType(ResourceType resourceType,int tenantId)
        {
            try{
                string resourceTypeCmd = @"INSERT INTO [dbo].[UserDefinedResourceTypes](ResourceTypeName,TenantId)
	                                        VALUES (@ResourceTypeName,@TenantId)";

                int insertedResourceType = 0;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    insertedResourceType = con.Execute(resourceTypeCmd,
                        new {
                            ResourceTypeName = resourceType.ResourceTypeName,
                            TenantId = tenantId
                        });
                    con.Close();
                }
                return insertedResourceType;
            }catch(Exception ex){
                throw ex;
            }
        }

        public IEnumerable<ResourceType> GetResourceTypes(int tenantId)
        {
            try{
                string resourceTypesCmd = @"SELECT * FROM [dbo].[UserDefinedResourceTypes] 
                                            WHERE TenantId = @TenantId";

                IEnumerable<ResourceType> resourceTypes = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    resourceTypes = con.Query<ResourceType>(resourceTypesCmd, new {TenantId = tenantId});
                    con.Close();
                }

                return resourceTypes;
            }catch(Exception ex){
                throw ex;
            }
        }

        public int CreateResource(Resource resource,int tenantId)
        {
            try{
                string resourceCmd = @"INSERT INTO [dbo].[Resources](ResourceName,Description,ResourceTypeId,TenantId)
	                                    VALUES(@ResourceName,@Description,@ResourceTypeId,@TenantId)";

                int insertedResource = 0;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    insertedResource = con.Execute(resourceCmd, new{
                        ResourceName = resource.ResourceName,
                        Description = resource.Description,
                        ResourceTypeId = resource.ResourceTypeId,
                        TenantId = tenantId
                    });
                    con.Close();
                }

                return insertedResource;
            }catch(Exception ex){
                throw ex;
            }
        }

        public IEnumerable<Resource> GetAllResources(int tenantId)
        {
            try{
                string resourcesCmd = @"SELECT * FROM [dbo].[Resources] WHERE TenantId = @TenantId";

                IEnumerable<Resource> resources = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    resources = con.Query<Resource>(resourcesCmd, new {TenantId = tenantId});
                    con.Close();
                }

                return resources;
            }catch(Exception ex){
                throw ex;
            }
        }
    }
}