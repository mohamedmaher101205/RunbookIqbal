using System;
using System.Collections.Generic;
using System.Text;
using Runbook.Models;

namespace Runbook.Services.Interfaces
{
    public interface ITaskService
    {
        bool CreateTask(Task task, string stageName, int bookId);

        IEnumerable<Task> GetAllTasks(int stageId);

        bool UpdateTaskStatus(int[] taskids, int statusId);

        int DeleteTasks(int bookId, string taskName);

        int UpdateTask(int taskId, Task task);
    }
}
