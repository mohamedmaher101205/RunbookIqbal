using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
using Runbook.Services;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runbook.API.Controllers
{
    /// <summary>
    /// This BookController class have methods to performing create a book,select particular book, 
    /// get all books,get book Statuses,modify Book by environment
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _book;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        private readonly ITaskService _taskService;

        /// <summary>
        /// This contructor is to inject object using dependency injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="book"></param>
        /// <param name="userService"></param>
        /// <param name="mailService"></param>
        /// <param name="taskService"></param>
        public BookController(ILogger<BookController> logger, IBookService book, IUserService userService, IMailService mailService, ITaskService taskService)
        {
            _logger = logger;
            _book = book;
            _userService = userService;
            _mailService = mailService;
            _taskService = taskService;
        }

        /// <summary>
        /// create new book
        /// </summary>
        /// <param name="book"></param>
        /// <returns>Success message</returns>
        [HttpPost]
        [Route("CreateBook")]
        public IActionResult CreateBook([FromBody] Book book)
        {
            try
            {
                var res = _book.CreateBook(book);

                if (res)
                {
                    return Ok("Book created successfully");
                }
                else
                {
                    return Ok("Book not created");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went Wrong in CreateBook : {ex}");
                return StatusCode(500, "Internal server error:Something went wrong");
            }
        }

        /// <summary>
        /// Get particular book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>book details</returns>
        [HttpGet]
        [Route("GetBookById/{id}")]
        public ActionResult<Book> GetBook(int id)
        {
            try
            {
                if (id > 0)
                {
                    return Ok(_book.GetBook(id));
                }
                else
                {
                    _logger.LogError($"Book id is not valid : {id}");
                    return BadRequest("Book Id is not Valid");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong in GetBook : {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all books
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tenantId"></param>
        /// <returns>List of books</returns>
        [HttpGet]
        [Route("GetBooks/{userid}/{tenantId}")]
        public ActionResult<IEnumerable<Book>> GetAllBooks(int userId, int tenantId)
        {
            try
            {
                if (userId > 0)
                {
                    return Ok(_book.GetAllBooks(userId, tenantId));
                }
                else
                {
                    _logger.LogError($"User id is not valid : {userId} in GetAllBooks");
                    return BadRequest("User Id is not valid");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong, Internal server error : {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// status of books
        /// </summary>
        /// <returns>Book status</returns>
        [HttpGet]
        [Route("GetStatuses")]
        public ActionResult<IEnumerable<Status>> GetStatuses()
        {
            try
            {
                return Ok(_book.GetStatuses());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in GetStatus : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Modify book details
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="envId"></param>
        /// <param name="statusId"></param>
        /// <returns>Success message</returns>
        [HttpPut]
        [Route("UpdateBook/{bookId}/{envId}/{statusId}")]
        public IActionResult UpdateBookByEnvironment(int bookId, int envId, int statusId)
        {
            try
            {
                if (bookId > 0 && envId >= 0 && statusId >= 0)
                {
                    var isUpdated = _book.UpdateBookStatus(bookId, envId, statusId);
                    //Runbook level notification to stack holders
                    Book book = _book.GetBook(bookId);
                    List<InviteUsers> inviteUsers = _userService.GetInviteUsers(book.TenantId);
                    var list = (from i in inviteUsers select i.InviteUserEmailId).ToList<string>();
                    var subject = "Notification  Runbook Updates:" + book.BookName + ":-";
                    IEnumerable<Task> tasks = _taskService.GetAllTasksByBookID(bookId);
                    var bodystart = @"<section>
                                        <p>Hi,</p> 
                                        <p>Runbook:" + book.BookName + @" status is Listed below.</p>
                                        <p><b>Task list:</b></p>";
                    foreach (Task t in tasks)
                    {
                        bodystart += $"<p> <h5> Task Name - { t.TaskName } </h5> </p>";
                    }

                    bodystart += "</section>";

                    _mailService.SendEmail(list, subject, bodystart);
                    //End Runbook level  Notifications.

                    if (isUpdated)
                    {
                        return Ok("Book updated successfully");
                    }
                    else
                    {
                        return BadRequest("Something went wrong");
                    }
                }
                else
                {
                    _logger.LogError($"Invalid BookID : {bookId} or environmentId : {envId} or StatusId : {statusId} in UpdateBookByEnvironment");
                    return BadRequest($"Invalid BookID : {bookId} or environmentId : {envId} or StatusId : {statusId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in UpdateBookByEnvironment : {ex}");
                return StatusCode(500, "Internal server Error");
            }
        }
    }
}