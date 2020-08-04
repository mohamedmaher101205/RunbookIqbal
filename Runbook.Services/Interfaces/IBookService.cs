using Runbook.Models;
using System.Collections.Generic;

namespace Runbook.Services.Interfaces
{
    public interface IBookService
    {
        bool CreateBook(Book book);

        Book GetBook(int id);

        IEnumerable<Book> GetAllBooks(int userId, int tenantId);

        IEnumerable<Status> GetStatuses();

        bool UpdateBookStatus(int bookId, int envId, int statusId);
    }
}