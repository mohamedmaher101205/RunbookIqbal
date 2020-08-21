using Runbook.Models;
using System.Collections.Generic;

namespace Runbook.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        bool CreateApplication(Application app, int tenantId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        IEnumerable<Application> GetAllApplications(int tenantId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        IEnumerable<ApplicationType> GetApplicationTypes(int tenantId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="appIds"></param>
        /// <returns></returns>
        int AddApplications(int bookId, int[] appIds);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        IEnumerable<Application> GetApplicationByBookId(int bookId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appType"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        int CreateCustomApplicationType(ApplicationType appType, int tenantId);
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        bool SendMailMultipleresourceOnSameDate(Book book);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        Book GetBookForMultipleRelease(int bookId);

    }
}