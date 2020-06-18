using System.Linq.Expressions;
using System.Collections;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RunbookAPI.Models;
using RunbookAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using RunbookAPI.Filters;

namespace RunbookAPI.Controllers
{
    [Authorize]
    [ApiController]
    [RefreshToken]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private IDataService _data;
        private ILogger _logger;

        public BookController(ILogger<BookController> logger,IDataService data)
        {
            _logger = logger;
            _data = data;
        }

        [HttpPost]
        [Route("createbook")]
        public IActionResult CreateBook([FromBody] Book book)
        {
            try{        
                var currentUser = HttpContext.User;
                int userId=int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
                int tenantId=int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "TenantId").Value);

                var res = _data.CreateBook(book,userId,tenantId);

                if(res){
                    return Ok("Book created successfully");
                }
                else{
                    return Ok("Book not created");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Something went Wrong in CreateBook : {ex}");
                return StatusCode(500,"Internal server error:Something went wrong");
            }
        }

        [HttpGet]
        [Route("bookbyid/{id}")]
        public ActionResult<Book> GetBook(int id)
        {
            try{
                if(id > 0){
                    return Ok(_data.GetBook(id));
                }
                else{
                    _logger.LogError($"Book id is not valid : {id}");
                    return BadRequest("Book Id is not Valid");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Something went wrong in GetBook : {ex}");
                return StatusCode(500,"Internal server error");
            }
        }

        [HttpGet]
        [Route("books/{userid}/{tenantId}")]
        public ActionResult<IEnumerable<Book>> GetAllBooks(int userId,int tenantId)
        {
            try{
                if(userId > 0){
                    return Ok(_data.GetAllBooks(userId,tenantId));
                }
                else{
                    _logger.LogError($"User id is not valid : {userId} in GetAllBooks");
                    return BadRequest("User Id is not valid");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Something went wrong, Internal server error : {ex}");
                return StatusCode(500,"Internal server error");
            }
        }

        [HttpPost]
        [Route("stage/{bookId}")]
        public IActionResult CreateStage(Stage stage,int bookId)
        {
            try{
                if(bookId > 0 && !string.IsNullOrEmpty(stage.StageName)){

                    bool res = _data.CreateStage(stage,bookId);

                    if(res){
                        return Ok("Stage created successfully");
                    }
                    else{
                        return Ok("Unsuccessful while creating stage");
                    }
                }
                else{
                    _logger.LogError($"Invalid BookID : {bookId} or Stage : {stage} in CreateStage");
                    return BadRequest("Invalid BookId or Stage Name");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Something went wrong, Internal server error : {ex}");
                return StatusCode(500,"Internal server error");
            }
        }

        [HttpGet]
        [Route("stages/{BookId}/{EnvId}")]
        public ActionResult<IEnumerable<Stage>> GetAllStages(int bookId,int envId)
        {
            try{
                if(bookId > 0 && envId > 0){
                    return Ok(_data.GetAllStages(bookId,envId));
                }
                else{
                    _logger.LogError($"Invalid Book Id : {bookId} or Environment Id : {envId} in GetAllStages");
                    return BadRequest($"Invalid Book Id : {bookId} or Environment Id : {envId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal server error : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpPost]
        [Route("task/{bookId}")]
        public IActionResult CreateTask([FromBody] RunbookAPI.Models.Task task,int bookId)
        {
            Console.WriteLine("Inside create task => Task name =>"+task.TaskName);
            try{
                string stageName = task.StageName;
                if(!string.IsNullOrEmpty(stageName) && !string.IsNullOrEmpty(task.TaskName)){

                    bool res = _data.CreateTask(task,stageName,bookId);

                    if(res){
                        return Ok("Task created successfully");
                    }
                    else{
                        return Ok("Unsuccessful while creating Task");
                    }
                }
                else{
                        _logger.LogError($"Invalid Stage : {stageName} or Task : {task} in CreateTask");
                        return BadRequest($"Invalid Stage : {stageName} or Task : {task}");
                    }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error : {ex} in CreateTask");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpGet]
        [Route("tasks/{StageId}")]
        public ActionResult<IEnumerable<RunbookAPI.Models.Task>> GetAllTasks(int stageId)
        {
            try{
                if(stageId > 0){
                    return Ok(_data.GetAllTasks(stageId));
                }
                else{
                    _logger.LogError($"Invalid stageId : {stageId} in GetAllTasks");
                    return BadRequest($"Invalid stageId : {stageId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal server Error in GetAllTasks : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpGet]
        [Route("statuses")]
        public ActionResult<IEnumerable<Status>> GetStatuses()
        {
            try{
                return Ok(_data.GetStatuses());
            }
            catch(Exception ex){
                _logger.LogError($"Internal server error in GetStatus : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpPut]
        [Route("updatetasks/{ids}/{statusId}")]
        public ActionResult<bool> UpdateTaskStatus(string ids,int statusId)
        {
            try{
                if(statusId > 0 && !string.IsNullOrEmpty(ids)){
                    int[] taskids = Array.ConvertAll(ids.Split(','), int.Parse);
                    return Ok(_data.UpdateTaskStatus(taskids,statusId));
                }
                else{
                    _logger.LogError($"Invalid StatusId : {statusId} or Task Ids : {ids} in UpdateTaskStatus");
                    return BadRequest($"Invalid StatusId : {statusId} or Task Ids : {ids}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in UpdateTaskStatus : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpPut]
        [Route("updateStage/{stageId}/{nextStageId}/{statusId}")]
        public ActionResult<bool> UpdateStageStatus(int stageId,int nextStageId,int statusId)
        {
            try{
                if(stageId > 0 && nextStageId >= 0 && statusId >= 0){
                    return Ok(_data.UpdateStageStatus(stageId,nextStageId,statusId));
                }
                else{
                    _logger.LogError($"Invalid stageId : {stageId} or nextStageId : {nextStageId} or StatusId : {statusId} in UpdateStageStatus");
                    return BadRequest($"Invalid stageId : {stageId} or nextStageId : {nextStageId} or StatusId : {statusId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in UpdateStageStatus : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpGet]
        [Route("environments/{tenantId}")]
        public ActionResult<IEnumerable<Environments>> GetAllEnvironments(int tenantId)
        {
            try{
                return Ok(_data.GetAllEnvironments(tenantId));
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in GetAllEnvironments : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpGet]
        [Route("updatebook/{bookId}/{envId}/{statusId}")]
        public IActionResult UpdateBookByEnvironment(int bookId,int envId,int statusId)
        {
            try{
                if(bookId > 0 && envId >= 0 && statusId >= 0){
                    var isUpdated = _data.UpdateBookStatus(bookId,envId,statusId);
                    if(isUpdated){
                        return Ok("Book updated successfully");
                    }else{
                        return BadRequest("Something went wrong");
                    }
                }
                else{
                    _logger.LogError($"Invalid BookID : {bookId} or environmentId : {envId} or StatusId : {statusId} in UpdateBookByEnvironment");
                    return BadRequest($"Invalid BookID : {bookId} or environmentId : {envId} or StatusId : {statusId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in UpdateBookByEnvironment : {ex}");
                return StatusCode(500,"Internal server Error");
            }
        }

        [HttpPut]
        [Route("deletetask/{bookId}/{taskName}")]
        public IActionResult DeleteTask(int bookId,string taskName)
        {
            try{
                if(bookId > 0 && !string.IsNullOrEmpty(taskName)){
                    var rowsDeleted = _data.DeleteTasks(bookId,taskName);
                    return Ok($"{rowsDeleted} Tasks are deleted");
                }
                else{
                    _logger.LogError($"Invalid BookId : {bookId} or TaskName : {taskName} in DeleteTask");
                    return BadRequest($"Invalid BookId : {bookId} or TaskName : {taskName}");
                }
            }catch(Exception ex){
                _logger.LogError($"Internal Server Error in DeleteTasks : {ex}");
                return StatusCode(500,"Internal server Error");
            }
        }

        [HttpGet]
        [Route("users")]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            try{
                var response = _data.GetAllUsers();
                if(response != null){
                    return Ok(response);
                }
                else{
                    _logger.LogError("Empty response in GetAllUsers");
                    return Ok(null);
                }
            }catch(Exception ex){
                _logger.LogError($"Internal Server Error in GetAllUsers : {ex}");
                return StatusCode(500,"Internal server Error");
            }
        }
    }
}
