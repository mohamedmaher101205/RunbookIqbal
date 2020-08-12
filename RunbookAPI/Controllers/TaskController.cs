using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Runbook.API.Controllers
{
    /// <summary>
    /// This TaskController class have methods to performing create a task,select particular book, 
    /// get all task,modify task Statuses,modify task and remove the task
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _task;
        private readonly ILogger _logger;

        /// <summary>
        /// This contructor is to inject object using dependency injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="task"></param>
        public TaskController(ILogger<TaskController> logger, ITaskService task)
        {
            _logger = logger;
            _task = task;
        }

        /// <summary>
        /// Create a task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="bookId"></param>
        /// <returns>Success message</returns>
        [HttpPost]
        [Route("CreateTask/{bookId}")]
        public IActionResult CreateTask([FromBody] Task task, int bookId)
        {
            try
            {
                string stageName = task.StageName;
                if (!string.IsNullOrEmpty(stageName) && !string.IsNullOrEmpty(task.TaskName))
                {

                    bool res = _task.CreateTask(task, stageName, bookId);

                    if (res)
                    {
                        return Ok("Task created successfully");
                    }
                    else
                    {
                        return Ok("Unsuccessful while creating Task");
                    }
                }
                else
                {
                    _logger.LogError($"Invalid Stage : {stageName} or Task : {task} in CreateTask");
                    return BadRequest($"Invalid Stage : {stageName} or Task : {task}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error : {ex} in CreateTask");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get all the tasks
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns>List of tasks</returns>
        [HttpGet]
        [Route("GetTasks/{StageId}")]
        public ActionResult<IEnumerable<Task>> GetAllTasks(int stageId)
        {
            try
            {
                if (stageId > 0)
                {
                    return Ok(_task.GetAllTasks(stageId));
                }
                else
                {
                    _logger.LogError($"Invalid stageId : {stageId} in GetAllTasks");
                    return BadRequest($"Invalid stageId : {stageId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server Error in GetAllTasks : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Modify task status
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="statusId"></param>
        /// <returns>True or false</returns>
        [HttpPut]
        [Route("UpdateTasks/{ids}/{statusId}")]
        public ActionResult<bool> UpdateTaskStatus(string ids, int statusId)
        {
            try
            {
                if (statusId > 0 && !string.IsNullOrEmpty(ids))
                {
                    int[] taskids = Array.ConvertAll(ids.Split(','), int.Parse);
                    return Ok(_task.UpdateTaskStatus(taskids, statusId));
                }
                else
                {
                    _logger.LogError($"Invalid StatusId : {statusId} or Task Ids : {ids} in UpdateTaskStatus");
                    return BadRequest($"Invalid StatusId : {statusId} or Task Ids : {ids}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in UpdateTaskStatus : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Remove the task
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="taskName"></param>
        /// <returns>removeed tasks details</returns>
        [HttpDelete]
        [Route("DeleteTask/{bookId}/{taskName}")]
        public IActionResult DeleteTask(int bookId, string taskName)
        {
            try
            {
                if (bookId > 0 && !string.IsNullOrEmpty(taskName))
                {
                    var rowsDeleted = _task.DeleteTasks(bookId, taskName);
                    return Ok($"{rowsDeleted} Tasks are deleted");
                }
                else
                {
                    _logger.LogError($"Invalid BookId : {bookId} or TaskName : {taskName} in DeleteTask");
                    return BadRequest($"Invalid BookId : {bookId} or TaskName : {taskName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in DeleteTasks : {ex}");
                return StatusCode(500, "Internal server Error");
            }
        }

        /// <summary>
        /// Modify the task using task id
        /// </summary>
        /// <param name="task"></param>
        /// <param name="taskId"></param>
        /// <returns>Success or fail message</returns>
        [HttpPost]
        [Route("UpdateTask/{taskId}")]
        public IActionResult UpdateTask([FromBody] Task task, int taskId)
        {
            try
            {
                if (taskId > 0)
                {
                    var taskUpdated = _task.UpdateTask(taskId, task);
                    if (taskUpdated > 0)
                        return Ok("Task updated successfully");
                    else
                        return Ok("Task updation unsuccessfull");
                }
                else
                {
                    _logger.LogError($"Invalid TaskId : {taskId} in UpdateTask");
                    return BadRequest($"Invalid TaskId : {taskId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in Update Tasks : {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}