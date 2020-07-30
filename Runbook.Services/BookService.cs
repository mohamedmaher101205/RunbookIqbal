using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    public class BookService : IBookService
    {
        private readonly IDbConnection _Idbconnection;

        public BookService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        public bool CreateBook(Book book, int userId, int tenantId)
        {
            string bookEnvcmd = @"INSERT INTO [dbo].[BookEnvironment](BookId,EnvId,TenantId)
                VALUES(@BookId,@EnvId,@TenantId)";

            var bookparams = new DynamicParameters();
            bookparams.Add("@BookName", book.BookName);
            bookparams.Add("@TargetedDate", book.TargetedDate);
            bookparams.Add("@UserId", userId);
            bookparams.Add("@Description", book.Description);
            bookparams.Add("@TenantId", tenantId);
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
                        TenantId = tenantId
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

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    book = con.QueryFirstOrDefault<Book>(bookcmd, new { BookId = id });
                    environments = con.Query<Environments>(envcmd, new { BookId = id });
                    con.Close();

                    foreach (var env in environments)
                    {
                        book.Environments.Add(env);
                    }
                }
                return book;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

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

        public bool UpdateBookStatus(int bookId, int envId, int statusId)
        {
            string updatecmd = @"UPDATE [dbo].[BookEnvironment] SET StatusId = @StatusId 
                                WHERE BookId = @BookId AND EnvId = @EnvId";
            int affectedRows = 0;
            using (IDbConnection con = _Idbconnection)
            {
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
            }
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