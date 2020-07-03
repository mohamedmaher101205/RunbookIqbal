using System.Collections;
using System;
using RunbookAPI.Models;
using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RunbookAPI.Services
{
    public class DataService : IDataService
    {
        private IDbConnection _Idbconnection;

        public DataService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        public User GetUser(User user)
        {
            try{
            User userResult = null;
            string sqlcmd = "SELECT * FROM [Runbook].[dbo].[User] where USEREMAIL=@useremail";
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                userResult = con.QueryFirstOrDefault<User>(sqlcmd, new { useremail = user.UserEmail});
                con.Close();
            }
            return userResult;
            }
            catch(Exception e){
                throw e;
            }
        }

        public bool CreateBook(Book book, int userId, int tenantId)
        {
            string bookEnvcmd = @"INSERT INTO [Runbook].[dbo].[BookEnvironment](BookId,EnvId,TenantId)
                VALUES(@BookId,@EnvId,@TenantId)";
           
            var bookparams = new DynamicParameters();
            bookparams.Add("@BookName",book.BookName);
            bookparams.Add("@TargetedDate",book.TargetedDate);
            bookparams.Add("@UserId",userId);
            bookparams.Add("@Description",book.Description);
            bookparams.Add("@TenantId",tenantId);
            bookparams.Add("@InsertedBookId", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

            // var envparams = new DynamicParameters();
            // envparams.Add("@UserID",userId);
            // envparams.Add("@IsActive",1);
            // envparams.Add("@TenantId",tenantId);
            // envparams.Add("@InsertedEnvId", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                 int createBookEnv=0;
                 var sqltrans = con.BeginTransaction();
                    var createdbook = con.Execute("[Runbook].[dbo].sp_CreateBook", bookparams,sqltrans,0, commandType: CommandType.StoredProcedure);
                    int insertedBookId = bookparams.Get<int>("@InsertedBookId");

                    foreach(var env in book.Environments)
                    {
                        // envparams.Add("@Environment",env);
                        // createEnv = con.Execute("[Runbook].[dbo].sp_CreateEnvForBook",envparams,sqltrans,0, commandType: CommandType.StoredProcedure);
                        // int insertedEnvId = envparams.Get<int>("@InsertedEnvId");

                        createBookEnv = con.Execute(bookEnvcmd,
                        new {
                                BookId = insertedBookId,
                                EnvId = env.EnvId,
                                TenantId = tenantId
                            },sqltrans
                        );
                    }
                    
                    if(createdbook > 0 && createBookEnv > 0){
                        sqltrans.Commit();
                    }
                    else{
                        sqltrans.Rollback();
                    }
                con.Close();
                
             if(createdbook > 0 && createBookEnv > 0)
             return true;
            }
            return false;
            
        }

        public Book GetBook(int id)
        {
            string bookcmd = @"SELECT * FROM [Runbook].[dbo].[BOOK] WHERE BookId=@BookId";
            string envcmd = @"SELECT benv.BookId,env.EnvId,env.Environment,benv.StatusId
                            FROM [Runbook].[dbo].[BookEnvironment] benv
                                JOIN [Runbook].[dbo].[UserDefinedEnvironments] env ON benv.envid = env.envid
                            WHERE benv.BookId = @BookId";
            try{
            Book book = null;
            IEnumerable<Environments> environments = null;

            using(IDbConnection con = _Idbconnection)
            {
                con.Open();
                book = con.QueryFirstOrDefault<Book>(bookcmd,new {BookId = id});
                environments = con.Query<Environments>(envcmd,new{BookId = id});
                con.Close();

                foreach (var env in environments)
                {
                    book.Environments.Add(env);
                }
            }
            return book;
            }
            catch(Exception e){
                throw e;
            }
        }

        public IEnumerable<Book> GetAllBooks(int userId,int tenantId)
        { 
            try{
            string bookcmd = @"SELECT * FROM [Runbook].[dbo].[BOOK] WHERE  TenantId = @TenantId OR userId = @UserId";

            string appscmd = @"	SELECT bookapp.BookId,app.AppId,app.ApplicationName 
                                FROM [Runbook].[dbo].[BookApplication] bookapp
                                    JOIN [Runbook].[dbo].[Application] app ON bookapp.AppId = app.AppId
                                    JOIN [Runbook].[dbo].[Book] book ON bookapp.BookId = book.BookId
                                WHERE book.TenantId = @TenantId OR book.UserId = @UserId";

            string envcmd = @"	SELECT benv.BookId,env.EnvId,env.Environment,benv.StatusId
                            FROM [Runbook].[dbo].[BookEnvironment] benv
                                JOIN [Runbook].[dbo].[UserDefinedEnvironments] env ON benv.envid = env.envid
                                JOIN [Runbook].[dbo].[Book] book ON benv.bookid = book.bookid
                            WHERE benv.TenantId = @TenantId OR book.UserId = @UserId";

            IEnumerable<Book> books = null;
            IEnumerable<Environments> envres = null;
            IEnumerable<Application> apps = null;

            using(IDbConnection con = _Idbconnection)
            {
                con.Open();

                books = con.Query<Book>(bookcmd,new {TenantId = tenantId, UserId = userId});
                envres = con.Query<Environments>(envcmd,new {TenantId = tenantId, UserId = userId});
                apps = con.Query<Application>(appscmd,new{TenantId = tenantId, UserId = userId});

                con.Close();
                foreach (var item in envres)
                {
                     foreach (var book in books)
                     {
                         if(item.BookId == book.BookId){
                            book.Environments.Add(item);
                         }
                     }
                }
                foreach (var app in apps)
                {
                    foreach (var book in books)
                    {
                        if(app.BookId == book.BookId){
                            book.Applications.Add(app);
                        }
                    }
                }
            }
            return books;
            }
            catch(Exception e){
                throw e;
            }
         }
        
        public bool CreateStage(Stage stage,int bookId)
        {
            try{

            string stagecmd = @"INSERT INTO [Runbook].[dbo].[STAGES](StageName,BookId,Description,EnvId,StatusId) 
                            VALUES(@StageName,@BookId,@Description,@EnvId,@StatusId)";

            string getallenvcmd = @"SELECT bookenv.BookId,env.EnvID FROM [Runbook].[dbo].[BookEnvironment] bookenv
                        JOIN [Runbook].[dbo].[userdefinedenvironments] env on bookenv.envId = env.envId
                        WHERE bookenv.bookId = @BookId";

            int stageCreated = 0;

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();

                IEnumerable<Environments> envs = con.Query<Environments>(getallenvcmd,new{BookId = bookId});

                List<Stage> createStagesForEnvs = new List<Stage>();
                foreach (var environment in envs)
                {
                    createStagesForEnvs.Add(new Stage{
                        StageName = stage.StageName,
                         Description = stage.Description,
                         BookId = bookId,
                         EnvId = environment.EnvId,
                         StatusId = 0
                        });
                }
                //var sqltrans = con.BeginTransaction();
                stageCreated = con.Execute(stagecmd,createStagesForEnvs);

                // if(stageCreated > 0){
                //     sqltrans.Commit();
                // }
                // else{
                //     sqltrans.Rollback();
                // }
                con.Close();
            }
            if(stageCreated > 0)
            {
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

        public IEnumerable<Stage> GetAllStages(int bookId,int envId)
        {
            try{
            string stagescmd = @"SELECT * FROM [Runbook].[dbo].[STAGES] WHERE BookId=@BookId AND EnvId = @EnvId";

            IEnumerable<Stage> stages = null;
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                stages = con.Query<Stage>(stagescmd,new {BookId=bookId,EnvId=envId});
                con.Close();
            }
            return stages;
            }
            catch(Exception e){
                throw e;
            }
        }

        public bool CreateTask(Task task,string stageName,int bookId)
        {
            string taskcmd = @"INSERT INTO [Runbook].[dbo].[Task](TaskName,StageId,Description,CompletedByDate,AssignedTo,StatusId) 
                VALUES(@TaskName,@StageId,@Description,@CompletedByDate,@AssignedTo,@StatusId)";

            string envcmd = @"SELECT * FROM [Runbook].[dbo].[STAGES]
                            WHERE BookId = @BookId AND StageName = @StageName";

            int taskCreated = 0;
            IEnumerable<Stage> stagesInEnvs = null;
            List<Task> createTaskForEnvs = new List<Task>();

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                //var sqltrans = con.BeginTransaction();
                stagesInEnvs = con.Query<Stage>(envcmd,new{BookId=bookId,StageName=stageName});
                foreach (var stage in stagesInEnvs)
                {
                    createTaskForEnvs.Add(new Task{
                    TaskName = task.TaskName,
                    Description = task.Description,
                    StageId = stage.StageId,
                    CompletedByDate = task.CompletedByDate,
                    AssignedTo = task.AssignedTo,
                    StatusId = 0
                    });
                }
                taskCreated = con.Execute(taskcmd,createTaskForEnvs);

                // if(taskCreated > 0){
                //     sqltrans.Commit();
                // }
                // else{
                //     sqltrans.Rollback();
                // }
                con.Close();
            }
            if(taskCreated > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<Task> GetAllTasks(int stageId)
        {
            string taskscmd = @"SELECT * FROM [Runbook].[dbo].[Task] WHERE StageId=@StageId";

            IEnumerable<Task> tasks = null;
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                tasks = con.Query<Task>(taskscmd,new {StageId=stageId});
                con.Close();
            }
            return tasks;
        }

        public IEnumerable<Status> GetStatuses(){
            string statuscmd = @"SELECT * FROM [Runbook].[dbo].[STATUS]";
            IEnumerable<Status> statuses = null;
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                statuses = con.Query<Status>(statuscmd);
                con.Close();
            }
            return statuses;
        }

        public bool UpdateTaskStatus(int[] taskids,int statusId)
        {
            string taskupdatecmd = @"UPDATE [Runbook].[dbo].[Task] SET StatusId = @StatusId WHERE TaskId IN @ids";
            int taskidupdate = 0;
            if(statusId > 2){
                statusId = 2;
            }
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                var sqltrans = con.BeginTransaction();
                taskidupdate = con.Execute(taskupdatecmd,new{ids = taskids,StatusId = statusId},sqltrans);
                if(taskidupdate > 0){
                    sqltrans.Commit();
                }
                else{
                    sqltrans.Rollback();
                }
                con.Close();
            }
            if(taskidupdate > 0){
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateStageStatus(int stageid, int nextStageId,int statusId)
        {
            try{
            string stageupdatecmd = @"UPDATE [Runbook].[dbo].[Stages] SET StatusId = @StatusId WHERE StageId = @ID";
            string nextstagestatus = @"UPDATE [Runbook].[dbo].[Stages] SET StatusId = 1 WHERE StageId = @NextId";
            int stagestatusupdate = 0, nextStageStatusUpdate = 0;
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                var sqltrans = con.BeginTransaction();
                stagestatusupdate = con.Execute(stageupdatecmd,new {ID = stageid,StatusId = statusId},sqltrans);
                if(nextStageId > 0){
                    nextStageStatusUpdate = con.Execute(nextstagestatus,new {NextId = nextStageId},sqltrans);
                }
                if(stagestatusupdate > 0){
                    sqltrans.Commit();
                }
                else{
                    sqltrans.Rollback();
                }
                con.Close();
            }
            if(stagestatusupdate > 0){
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

        public IEnumerable<Environments> GetAllEnvironments(int tenantId)
        {
            string envcmd = @"SELECT * FROM [Runbook].[dbo].[userdefinedenvironments] WHERE TenantId = @TenantId";

            IEnumerable<Environments> envs = null;

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                envs = con.Query<Environments>(envcmd,new{TenantId = tenantId});
                con.Close();
            }
            return envs;
        }

        public bool UpdateBookStatus(int bookId,int envId,int statusId)
        {
            string updatecmd = @"UPDATE [Runbook].[dbo].[BookEnvironment] SET StatusId = @StatusId 
                                WHERE BookId = @BookId AND EnvId = @EnvId";
            int affectedRows = 0;
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                var sqltrans = con.BeginTransaction();
                affectedRows = con.Execute(updatecmd,
                        new {
                            statusId = statusId,
                            BookId = bookId,
                            EnvId = envId
                        },sqltrans);

                if(affectedRows > 0){
                    sqltrans.Commit();
                }
                else{
                    sqltrans.Rollback();
                }
                con.Close();
            }
            if(affectedRows > 0){
                return true;
            }else{
                return false;
            }
        }

        public int DeleteTasks(int bookId,string taskName)
        {
            string gettaskids = @"SELECT t.TaskId FROM [Runbook].[dbo].[Task] t
                                    JOIN [Runbook].[dbo].[STAGES] stg ON stg.StageId = t.StageId
                                WHERE stg.BookId = @BookId AND t.TaskName = @TaskName";

            string deletetask = @"DELETE FROM [Runbook].[dbo].[Task] WHERE TaskId in @TaskIds";
            
            IEnumerable<int> taskids = null;
            int taskdeleted = 0;

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                var sqltrans = con.BeginTransaction();
                taskids = con.Query<int>(gettaskids,new{BookId=bookId,TaskName=taskName},sqltrans);
                taskdeleted = con.Execute(deletetask,new{TaskIds = taskids},sqltrans);
                if(taskdeleted > 0 ){
                    sqltrans.Commit();
                }
                else{
                    sqltrans.Rollback();
                }
                con.Close();
            }
            
            return taskdeleted;
        }

        public IEnumerable<User> GetAllUsers()
        {
            try{
                string userscmd = @"SELECT UserId,FirstName,LastName,UserEmail FROM 
                                    [RunBook].[dbo].[User]";

                IEnumerable<User> users = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    users = con.Query<User>(userscmd);
                    con.Close();
                }

                return users;
            }catch(Exception ex){
                throw ex;
            }
        }
    }
}