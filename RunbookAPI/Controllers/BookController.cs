using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
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
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _book;
        private readonly ILogger _logger;

        /// <summary>
        /// This contructor is to inject object using dependency injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="book"></param>
        public BookController(ILogger<BookController> logger, IBookService book)
        {
            _logger = logger;
            _book = book;
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