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
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _book;
        private readonly ILogger _logger;

        public BookController(ILogger<BookController> logger, IBookService book)
        {
            _logger = logger;
            _book = book;
        }

        [HttpPost]
        [Route("CreateBook")]
        public IActionResult CreateBook([FromBody] Book book)
        {
            try
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
                int tenantId = int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "TenantId").Value);

                var res = _book.CreateBook(book, userId, tenantId);

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