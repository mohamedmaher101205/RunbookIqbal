using Runbook.Models;
using System.Collections.Generic;

namespace Runbook.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        bool CreateBook(Book book);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Book GetBook(int id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        IEnumerable<Book> GetAllBooks(int userId, int tenantId);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Status> GetStatuses();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="envId"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        bool UpdateBookStatus(int bookId, int envId, int statusId);
    }
}