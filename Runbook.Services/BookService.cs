using Dapper;
using Microsoft.Extensions.Configuration;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    /// <summary>
    /// This BookService class have methods to performing create a book,select particular book, 
    /// get all books,get book Statuses,modify Book by environment
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IDbConnection _Idbconnection;

        /// <summary>
        /// This constructor is to inject IDBConnection using constructor dependency injuction
        /// </summary>
        /// <param name="dbConnection"></param>
        public BookService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        /// <summary>
        /// create new book
        /// </summary>
        /// <param name="book"></param>
        /// <returns>Success message</returns>       
        public bool CreateBook(Book book)
        {
            string bookEnvcmd = @"INSERT INTO [dbo].[BookEnvironment](BookId,EnvId,TenantId)
                VALUES(@BookId,@EnvId,@TenantId)";

            var bookparams = new DynamicParameters();
            bookparams.Add("@BookName", book.BookName);
            bookparams.Add("@TargetedDate", book.TargetedDate);
            bookparams.Add("@UserId", book.UserId);
            bookparams.Add("@Description", book.Description);
            bookparams.Add("@TenantId", book.TenantId);
            bookparams.Add("@InsertedBookId", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                int createBookEnv = 0;
                var sqltrans = con.BeginTransaction();
                var createdbook = con.Execute("[dbo].sp_CreateBook", bookparams, sqltrans, 0, commandType: CommandType.StoredProcedure);
                int insertedBookId = bookparams.Get<int>("@InsertedBookId");

                foreach (var env in book.Environments)
                {

                    createBookEnv = con.Execute(bookEnvcmd,
                    new
                    {
                        BookId = insertedBookId,
                        EnvId = env.EnvId,
                        TenantId = book.TenantId
                    }, sqltrans
                    );
                }

                if (createdbook > 0 && createBookEnv > 0)
                {
                    sqltrans.Commit();
                }
                else
                {
                    sqltrans.Rollback();
                }
                con.Close();

                if (createdbook > 0 && createBookEnv > 0)
                    return true;
            }
            return false;

        }

        /// <summary>
        /// select particular book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>book details</returns>
        public Book GetBook(int id)
        {
            string bookcmd = @"SELECT * FROM [dbo].[BOOK] WHERE BookId=@BookId";
            string envcmd = @"SELECT benv.BookId,env.EnvId,env.Environment,benv.StatusId
                            FROM [dbo].[BookEnvironment] benv
                                JOIN [dbo].[UserDefinedEnvironments] env ON benv.envid = env.envid
                            WHERE benv.BookId = @BookId";
            try
            {
                Book book = null;
                IEnumerable<Environments> environments = null;

                IDbConnection con = _Idbconnection;
                con.Open();
                book = con.QueryFirstOrDefault<Book>(bookcmd, new { BookId = id });
                environments = con.Query<Environments>(envcmd, new { BookId = id });
                con.Close();

                foreach (var env in environments)
                {
                    book.Environments.Add(env);
                }

                return book;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// select all books
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tenantId"></param>
        /// <returns>List of books</returns>
        public IEnumerable<Book> GetAllBooks(int userId, int tenantId)
        {
            try
            {
                string bookcmd = @"SELECT * FROM [dbo].[BOOK] WHERE  TenantId = @TenantId OR userId = @UserId";

                string appscmd = @"	SELECT bookapp.BookId,app.AppId,app.ApplicationName 
                                FROM [dbo].[BookApplication] bookapp
                                    JOIN [dbo].[Application] app ON bookapp.AppId = app.AppId
                                    JOIN [dbo].[Book] book ON bookapp.BookId = book.BookId
                                WHERE book.TenantId = @TenantId OR book.UserId = @UserId";

                string envcmd = @"	SELECT benv.BookId,env.EnvId,env.Environment,benv.StatusId
                            FROM [dbo].[BookEnvironment] benv
                                JOIN [dbo].[UserDefinedEnvironments] env ON benv.envid = env.envid
                                JOIN [dbo].[Book] book ON benv.bookid = book.bookid
                            WHERE benv.TenantId = @TenantId OR book.UserId = @UserId";

                IEnumerable<Book> books = null;
                IEnumerable<Environments> envres = null;
                IEnumerable<Application> apps = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();

                    books = con.Query<Book>(bookcmd, new { TenantId = tenantId, UserId = userId });
                    envres = con.Query<Environments>(envcmd, new { TenantId = tenantId, UserId = userId });
                    apps = con.Query<Application>(appscmd, new { TenantId = tenantId, UserId = userId });

                    con.Close();
                    foreach (var item in envres)
                    {
                        foreach (var book in books)
                        {
                            if (item.BookId == book.BookId)
                            {
                                book.Environments.Add(item);
                            }
                        }
                    }
                    foreach (var app in apps)
                    {
                        foreach (var book in books)
                        {
                            if (app.BookId == book.BookId)
                            {
                                book.Applications.Add(app);
                            }
                        }
                    }
                }
                return books;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// status of books
        /// </summary>
        /// <returns>book status</returns>
        public IEnumerable<Status> GetStatuses()
        {
            string statuscmd = @"SELECT * FROM [dbo].[STATUS]";
            IEnumerable<Status> statuses = null;
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                statuses = con.Query<Status>(statuscmd);
                con.Close();
            }
            return statuses;
        }

        /// <summary>
        /// modify book details
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="envId"></param>
        /// <param name="statusId"></param>
        /// <returns>Success message</returns>
        public bool UpdateBookStatus(int bookId, int envId, int statusId)
        {
            string updatecmd = @"UPDATE [dbo].[BookEnvironment] SET StatusId = @StatusId 
                                WHERE BookId = @BookId AND EnvId = @EnvId";
            int affectedRows = 0;
            IDbConnection con = _Idbconnection;
            con.Open();
            var sqltrans = con.BeginTransaction();
            affectedRows = con.Execute(updatecmd,
                    new
                    {
                        statusId = statusId,
                        BookId = bookId,
                        EnvId = envId
                    }, sqltrans);

            if (affectedRows > 0)
            {
                sqltrans.Commit();
            }
            else
            {
                sqltrans.Rollback();
            }
            con.Close();
            if (affectedRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}