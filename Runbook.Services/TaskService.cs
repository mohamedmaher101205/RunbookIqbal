﻿using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    public class TaskService : ITaskService
    {
        private readonly IDbConnection _Idbconnection;

        public TaskService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        public bool CreateTask(Task task, string stageName, int bookId)
        {
            string taskcmd = @"INSERT INTO [dbo].[Task](TaskName,StageId,Description,CompletedByDate,AssignedTo,StatusId) 
                VALUES(@TaskName,@StageId,@Description,@CompletedByDate,@AssignedTo,@StatusId)";

            string envcmd = @"SELECT * FROM [dbo].[STAGES]
                            WHERE BookId = @BookId AND StageName = @StageName";

            int taskCreated = 0;
            IEnumerable<Stage> stagesInEnvs = null;
            List<Task> createTaskForEnvs = new List<Task>();

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                //var sqltrans = con.BeginTransaction();
                stagesInEnvs = con.Query<Stage>(envcmd, new { BookId = bookId, StageName = stageName });
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
                taskCreated = con.Execute(taskcmd, createTaskForEnvs);

                // if(taskCreated > 0){
                //     sqltrans.Commit();
                // }
                // else{
                //     sqltrans.Rollback();
                // }
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

        public IEnumerable<Task> GetAllTasks(int stageId)
        {
            string taskscmd = @"SELECT * FROM [dbo].[Task] WHERE StageId=@StageId";

            IEnumerable<Task> tasks = null;
            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                tasks = con.Query<Task>(taskscmd, new { StageId = stageId });
                con.Close();
            }
            return tasks;
        }

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

        public int UpdateTask(int taskId, Task task)
        {
            try
            {
                string updateTaskCmd = @"UPDATE [dbo].[Task] SET TaskName = @TaskName, Description = @Description,
	                                        ReleaseNote = @ReleaseNote WHERE TaskId = @TaskId";

                int taskUpdated = 0;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    taskUpdated = con.Execute(updateTaskCmd, new
                    {
                        TaskName = task.TaskName,
                        Description = task.Description,
                        ReleaseNote = task.ReleaseNote,
                        TaskId = task.TaskId
                    });
                    con.Close();
                }
                return taskUpdated;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}