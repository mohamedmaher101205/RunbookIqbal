using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Runbook.Services
{
    /// <summary>
    /// This ApplicationService class used to create Application and tenant, Read particular application or all, Read particular 
    /// application type or all, add application to book, read application using book
    /// </summary>
    public class ApplicationService : IApplicationService
    {
        private readonly IDbConnection _Idbconnection;

        private readonly IMailService _mail;

        /// <summary>
        /// This constructor is to inject IDBConnection using constructor dependency injuction
        /// </summary>
        /// <param name="dbConnection"></param>
        public ApplicationService(IDbConnection dbConnection, IMailService mail)
        {
            _Idbconnection = dbConnection;
            _mail = mail;
        }

        /// <summary>
        /// create an application
        /// </summary>
        /// <param name="app"></param>
        /// <param name="tenantId"></param>
        /// <returns>true or false</returns>
        public bool CreateApplication(Application app, int tenantId)
        {
            try
            {
                string appResourcesCmd = @"INSERT INTO [dbo].[ApplicationResources](AppId,ResourceId,TenantId)
	                                    VALUES(@AppId,@ResourceId,@TenantId)";

                var appParameters = new DynamicParameters();
                appParameters.Add("@ApplicationName", app.ApplicationName);
                appParameters.Add("@Description", app.Description);
                appParameters.Add("@TenantId", tenantId);
                appParameters.Add("@CreatedAppId", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                string apptypecmd = @"SELECT AppTypeId FROM [dbo].[UserDefinedapplicationType] 
                                        WHERE AppTypeName = @AppTypeName AND TenantId = @TenantId";

                int appcreated = 0;

                List<ApplicationResources> appResources = new List<ApplicationResources>();

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    ApplicationType apptype = con.QueryFirstOrDefault<ApplicationType>(apptypecmd, new { AppTypeName = app.AppTypeName, TenantId = tenantId });

                    appParameters.Add("@AppTypeId", apptype.AppTypeId);
                    appcreated = con.Execute("[dbo].sp_CreateApplication", appParameters, commandType: CommandType.StoredProcedure);
                    int createdAppId = appParameters.Get<int>("@CreatedAppId");

                    foreach (var resType in app.Resources)
                    {
                        appResources.Add(new ApplicationResources
                        {
                            AppId = createdAppId,
                            ResourceId = resType.ResourceId,
                            TenantId = tenantId
                        });
                    }
                    var appResourcesCreated = con.Execute(appResourcesCmd, appResources);
                    con.Close();
                }
                if (appcreated > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Read all application details
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of applications</returns>
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
                apps = con.Query<Application>(getappscmd, new { TenantId = tenantId });
                resources = con.Query<Resource>(resourcesCmd, new { TenantId = tenantId });
                con.Close();
            }
            foreach (var app in apps)
            {
                foreach (var resource in resources)
                {
                    if (app.AppId == resource.AppId)
                    {
                        app.Resources.Add(resource);
                    }
                }
            }
            return apps;
        }

        /// <summary>
        /// Read all application type details
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of applications details</returns>
        public IEnumerable<ApplicationType> GetApplicationTypes(int tenantId)
        {
            string apptypecmd = @"SELECT * FROM [dbo].[UserDefinedapplicationType] 
                                        WHERE tenantId = @TenantID";

            IEnumerable<ApplicationType> apptypes = null;

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                apptypes = con.Query<ApplicationType>(apptypecmd, new { TenantId = tenantId });
                con.Close();
            }
            return apptypes;
        }

        /// <summary>
        /// add new application
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="appIds"></param>
        /// <returns>inserted row</returns>
        public int AddApplications(int bookId, int[] appIds)
        {
            string appinsertcmd = @"INSERT INTO [dbo].[BookApplication](BookId,AppId) VALUES(@BookId,@AppId)";
            string deleteExistingApps = @"DELETE FROM [dbo].[BookApplication] WHERE BookId = @BookId";

            List<object> appAndBook = new List<object>();
            for (int i = 0; i < appIds.Length; i++)
            {
                appAndBook.Add(new { BookId = bookId, AppId = appIds[i] });
            }
            int insertedrows = 0;
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                //var sqltrans = con.BeginTransaction();
                var deletedRows = con.Execute(deleteExistingApps, new { BookId = bookId });
                insertedrows = con.Execute(appinsertcmd, appAndBook);
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

        /// <summary>
        /// Read all application details using book id 
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns>List of applications</returns>

        /// <returns></returns>
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
                apps = con.Query<Application>(getapps, new { BookId = bookId });
                con.Close();
            }
            return apps;
        }

        /// <summary>
        /// add new applicaion type
        /// </summary>
        /// <param name="appType"></param>
        /// <param name="tenantId"></param>
        /// <returns> integer value</returns>
        public int CreateCustomApplicationType(ApplicationType appType, int tenantId)
        {
            try
            {
                string appTypeCmd = @"INSERT INTO [dbo].[UserDefinedApplicationType](AppTypeName,TenantId)
                                        VALUES (@AppTypeName,@TenantId)";

                int insertedAppType = 0;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    insertedAppType = con.Execute(appTypeCmd,
                        new
                        {
                            AppTypeName = appType.AppTypeName,
                            TenantId = tenantId
                        });
                    con.Close();
                }
                return insertedAppType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public bool SendMailMultipleresourceOnSameDate(Book book)
        {
            try
            {
                var inviteUserparams = new DynamicParameters();
                inviteUserparams.Add("@TenantId", book.TenantId);

                IEnumerable<InviteUsers> inviteUser = null;
                string subject = "";
                string body = "";
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    inviteUser = con.Query<InviteUsers>("[sp_GetResourceOnMultipleRelease]", inviteUserparams, commandType: CommandType.StoredProcedure);
                    var applist = con.Query<Application>("select app.* from BookApplication bookapp inner join Application app on bookapp.AppId = app.AppId and bookapp.BookId = @BookId", new { BookId = book.BookId }).Select(u =>u.ApplicationName).ToList();
                    var appwithComma = String.Join(",  ", applist);
                    subject = $"{book.BookName} - Conflict on Release Dates for {appwithComma}";
                    body = @"<section><p>Hi Team,</p><p>There is a conflict in the realease date of the application - "+ appwithComma +" for "+book.BookName+"</p><p>Regards,</p><p>Runbook Team</p></section>";
                    con.Close();
                }
                var emailList = inviteUser.Select(c => c.InviteUserEmailId).ToList();
                _mail.SendEmail(emailList, subject, body);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Book GetBookForMultipleRelease(int id)
        {
            string bookcmd = @"SELECT * FROM [dbo].[BOOK] WHERE BookId=@BookId";
            try
            {
                Book book = null;
                IDbConnection con = _Idbconnection;
                con.Open();
                book = con.QueryFirstOrDefault<Book>(bookcmd, new { BookId = id });
                con.Close();
                return book;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}