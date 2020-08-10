using System;
using System.Collections.Generic;
using System.Text;
using Runbook.Models;

namespace Runbook.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="stageName"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        bool CreateTask(Task task, string stageName, int bookId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns></returns>
        IEnumerable<Task> GetAllTasks(int stageId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskids"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        bool UpdateTaskStatus(int[] taskids, int statusId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        int DeleteTasks(int bookId, string taskName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        int UpdateTask(int taskId, Task task);
    }
}
