using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;

namespace Runbook.Services
{
    /// <summary>
    /// This TaskService class have methods to performing create a task,select particular book, 
    /// get all task,modify task Statuses,modify task and remove the task
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly IDbConnection _Idbconnection;

        private readonly IMailService _mail;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="dbConnection"></param>
        public TaskService(IDbConnection dbConnection, IMailService mail)
        {
            _Idbconnection = dbConnection;
            _mail = mail;
        }

        /// <summary>
        /// create a task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="stageName"></param>
        /// <param name="bookId"></param>
        /// <returns>true or false</returns>
        public bool CreateTask(Task task, string stageName, int bookId)
        {
            string taskcmd = @"INSERT INTO [dbo].[Task](TaskName,StageId,Description,CompletedByDate,AssignedTo,StatusId) 
                VALUES(@TaskName,@StageId,@Description,@CompletedByDate,@AssignedTo,@StatusId)";

            string envcmd = @"SELECT * FROM [dbo].[STAGES]
                            WHERE BookId = @BookId AND StageName = @StageName";

            string subject = null;
            string body = null;
            int taskCreated = 0;
            IEnumerable<Stage> stagesInEnvs = null;
            List<Task> createTaskForEnvs = new List<Task>();

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                var sqltrans = con.BeginTransaction();
                var runbookName = con.Query<Book>("SELECT * FROM [dbo].[BOOK] WHERE BookId=@BookId", new { BookId = bookId }, sqltrans).Select(c => c.BookName).FirstOrDefault();
                var statusname = con.Query<Status>("SELECT * FROM [dbo].[STATUS] WHERE StatusId=@StatusId", new { StatusId = task.StatusId }, sqltrans).Select(b => b.Description).FirstOrDefault();
                var tenantname = con.Query<Tenant>("select * from Tenant where TenantId =@TenantId", new { TenantId = task.TenantId }, sqltrans).Select(b => b.TenantName).FirstOrDefault();
                subject = $"{runbookName} Task#{task.TaskId}";
                body = @"<section><p>Hi Team,</p> <p>" + runbookName + " - task#" + task.TaskId.ToString() + " has been" + statusname + "<p>Regards</p><p>" + tenantname + "</p></section>";
                stagesInEnvs = con.Query<Stage>(envcmd, new { BookId = bookId, StageName = stageName }, sqltrans);
                foreach (var stage in stagesInEnvs)
                {
                    createTaskForEnvs.Add(new Task
                    {
                        TaskName = task.TaskName,
                        Description = task.Description,
                        StageId = stage.StageId,
                        CompletedByDate = task.CompletedByDate,
                        AssignedTo = task.AssignedTo,
                        StatusId = 0
                    });
                }
                taskCreated = con.Execute(taskcmd, createTaskForEnvs, sqltrans);

                if (taskCreated > 0)
                {
                    sqltrans.Commit();
                    _mail.SendEmail(task.AssignedTo, subject, body, string.Empty);
                }
                else
                {
                    sqltrans.Rollback();
                }
                con.Close();
            }
            if (taskCreated > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// read all tasks
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns>list of tasks</returns>
        public IEnumerable<Task> GetAllTasks(int stageId)
        {
            string taskscmd = @"SELECT T.TaskId,T.TaskName,T.StageId,T.Description as Description, T.CompletedByDate,
                                T.AssignedTo,T.ReleaseNote,T.TenantId, S.description as StatusDescription,T.StatusId 
                                FROM [dbo].[Task] T
                                    JOIN dbo.[status] S ON T.StatusId = S.StatusId 
                                WHERE StageId=@StageId";

            IEnumerable<Task> tasks = null;
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                tasks = con.Query<Task>(taskscmd, new { StageId = stageId });
                con.Close();
            }
            return tasks;
        }

        /// <summary>
        /// Fetch Tasks by Book Id
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public IEnumerable<Task> GetAllTasksByBookID(int bookId)
        {
            string taskscmd = @"select TaskId,TaskName, t.Description,t.tenantID,
                              case  
                              when t.statusid=0 then 'Assigned' 
                              when t.statusid=1 then 'In progress' 
                              when t.statusid=2 then 'Completed' 
                              when t.statusid=3 then 'Cancelled' 
                              when t.statusid=4 then 'Rollback' 
                              end  Status, t.StatusId
                              from Task t   where t.TenantId=(select b.TenantId from book b where b.bookid = " + bookId + ")";

            IEnumerable<Task> tasks = null;
            IDbConnection con = _Idbconnection;
            con.Open();
            tasks = con.Query<Task>(taskscmd);
            con.Close();
            return tasks;
        }


        /// <summary>
        /// modify the task status
        /// </summary>
        /// <param name="taskids"></param>
        /// <param name="statusId"></param>
        /// <returns>true or false</returns>
        public bool UpdateTaskStatus(int[] taskids, int statusId)
        {
            string taskupdatecmd = @"UPDATE [dbo].[Task] SET StatusId = @StatusId WHERE TaskId IN @ids";
            int taskidupdate = 0;
            if (statusId > 2)
            {
                statusId = 2;
            }
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                var sqltrans = con.BeginTransaction();
                taskidupdate = con.Execute(taskupdatecmd, new { ids = taskids, StatusId = statusId }, sqltrans);
                if (taskidupdate > 0)
                {
                    sqltrans.Commit();
                }
                else
                {
                    sqltrans.Rollback();
                }
                con.Close();
            }
            if (taskidupdate > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// remove the task
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="taskName"></param>
        /// <returns>removeed tasks details</returns>
        public int DeleteTasks(int bookId, string taskName)
        {
            string gettaskids = @"SELECT t.TaskId FROM [dbo].[Task] t
                                    JOIN [dbo].[STAGES] stg ON stg.StageId = t.StageId
                                WHERE stg.BookId = @BookId AND t.TaskName = @TaskName";

            string deletetask = @"DELETE FROM [dbo].[Task] WHERE TaskId in @TaskIds";

            IEnumerable<int> taskids = null;
            int taskdeleted = 0;

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                var sqltrans = con.BeginTransaction();
                taskids = con.Query<int>(gettaskids, new { BookId = bookId, TaskName = taskName }, sqltrans);
                taskdeleted = con.Execute(deletetask, new { TaskIds = taskids }, sqltrans);
                if (taskdeleted > 0)
                {
                    sqltrans.Commit();
                }
                else
                {
                    sqltrans.Rollback();
                }
                con.Close();
            }

            return taskdeleted;
        }

        /// <summary>
        /// modify the task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="taskId"></param>
        /// <returns>integer value</returns>
        public int UpdateTask(int taskId, Task task)
        {
            try
            {
                BookService bookService = new BookService(_Idbconnection);
                var statusname = bookService.GetStatuses().Where(u => u.StatusId == task.StatusId).Select(b => b.Description).FirstOrDefault();
                string subject = $"{task.TaskName} Task#{task.TaskId}";
                string body = null;
                string updateTaskCmd = @"UPDATE [dbo].[Task] SET TaskName = @TaskName, Description = @Description,
	                                        ReleaseNote = @ReleaseNote,AssignedTo = @AssignedTo,StatusId = @StatusId,Subscribers = @Subscribers
                                            WHERE TaskId = @TaskId";

                int taskUpdated = 0;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    var sqltrans = con.BeginTransaction();
                    var tenantname = con.Query<Tenant>("SELECT * FROM [dbo].[Tenant]", sqltrans).Where(u => u.TenantId == task.TenantId).Select(b => b.TenantName).FirstOrDefault();
                    body = @"<section><p>Hi Team,</p> <p>" + task.TaskName + " task#" + taskId.ToString() + " has been " + statusname + " </p><p>Regards</p><p>" + tenantname + "</p></section>";
                    taskUpdated = con.Execute(updateTaskCmd, new
                    {
                        TaskName = task.TaskName,
                        Description = task.Description,
                        ReleaseNote = task.ReleaseNote,
                        TaskId = task.TaskId,
                        AssignedTo = task.AssignedTo,
                        StatusId = task.StatusId,
                        Subscribers = task.Subscribers
                    }, sqltrans);
                    if (taskUpdated > 0)
                    {
                        sqltrans.Commit();
                        _mail.SendEmail(task.AssignedTo, subject, body, task.Subscribers);
                    }
                    else
                    {
                        sqltrans.Rollback();
                    }
                    con.Close();
                }
                return taskUpdated;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// modify the task for subscriber
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="emailId"></param>
        /// <returns>true or false</returns>
        public bool subscribeTask(int taskId, string emailId)
        {
            string taskupdatecmd = @"UPDATE Task SET Subscribers= @emailId WHERE TaskId = @taskId";
            int taskemailupdate = 0;
            string subject = null;
            string body = null;
            int TaskId = taskId;
            string Subscribers = emailId;
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                var sqltrans = con.BeginTransaction();
                var task = con.Query<Task>("SELECT * FROM Task WHERE TaskId = @taskId", new { TaskId = taskId }, sqltrans).FirstOrDefault();
                var statusname = con.Query<Status>("SELECT * FROM [dbo].[STATUS] WHERE StatusId=@StatusId", new { StatusId = task.StatusId }, sqltrans).Select(b => b.Description).FirstOrDefault();
                var tenantname = con.Query<Tenant>("select * from Tenant where TenantId =@TenantId", new { TenantId = task.TenantId }, sqltrans).Select(b => b.TenantName).FirstOrDefault();
                body = @"<section><p>Hi Team,</p> <p>" + task.TaskName + " - task#" + task.TaskId.ToString() + " has been" + statusname + "<p>Regards</p><p>" + tenantname + "</p></section>";
                subject = $"{task.TaskName} - Task#{taskId}";

                taskemailupdate = con.Execute(taskupdatecmd, new { taskId = taskId, emailId = emailId }, sqltrans);
                //Must declare the scalar variable "@emailId".
                if (taskemailupdate > 0)
                {
                    sqltrans.Commit();
                    _mail.SendEmail(task.AssignedTo, subject, body, emailId);
                }
                else
                {
                    sqltrans.Rollback();
                }
                con.Close();
            }
            if (taskemailupdate > 0)
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
