using System.Net.Mime;
using System;
using Xunit;
using Moq;
using Runbook.Services.Interfaces;
using Runbook.Models;
using System.Collections.Generic;
using Runbook.API.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Runbook.Test
{
    public class BookControllerTest
    {
        private readonly Mock<IBookService> bookServiceMoq;
        private readonly Mock<ILogger<BookController>> logger;
        private readonly Mock<IUserService> userService;
        private readonly Mock<IMailService> mailService;
        private readonly Mock<ITaskService> taskService;
        public BookControllerTest()
        {
            bookServiceMoq = new Mock<IBookService>();
            logger = new Mock<ILogger<BookController>>();
            userService = new Mock<IUserService>();
            mailService = new Mock<IMailService>();
            taskService = new Mock<ITaskService>();
        }


        [Fact]
        public void CreateBook_Successfull()
        {
            Book book = new Book
            {

                BookId = 4,
                BookName = "TestBook",
                Description = "Checking X unit test",
                TenantId = 1,

            };
            bookServiceMoq.Setup(c => c.CreateBook(book)).Returns(true);
            // Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);
            var response = controller.CreateBook(book);
            // Assert
            Assert.IsType<OkObjectResult>(response);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Book created successfully", okResult.Value);
            bookServiceMoq.Verify(c => c.CreateBook(book), Times.Once());

        }

        [Fact]
        public void CreateBook_Unsuccessfull()
        {
            // Arrange
            Book book = new Book
            {

                BookId = 4,
                BookName = "TestBook",
                Description = "Checking X unit test",
                TenantId = 1,

            };

            bookServiceMoq.Setup(c => c.CreateBook(book)).Returns(false);
            // Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);
            var response = controller.CreateBook(book);
            // Assert

            var OkResult = response as OkObjectResult;
            Assert.NotNull(OkResult);
            Assert.Equal("Book not created", OkResult.Value);
            bookServiceMoq.Verify(c => c.CreateBook(book), Times.Once());

        }

        [Fact]
        public void GetBook_Returns_Books()
        {
            //Arrange
            int BookId = 1;
            bookServiceMoq.Setup(c => c.GetBook(BookId)).Returns(It.IsAny<Book>());
            //Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);

            var result = controller.GetBook(BookId);
            //assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            bookServiceMoq.Verify(c => c.GetBook(BookId), Times.Once());

        }

        [Fact]
        public void GetBook_Invalid_BookId()
        {
            //Arrange
            int BookId = 0;
            //Act
            bookServiceMoq.Setup(c => c.GetBook(BookId)).Returns(It.IsAny<Book>());
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);
            var result = controller.GetBook(BookId);
            var BadRequest = result.Result as BadRequestObjectResult;
            // assert
            Assert.NotNull(BadRequest);
            Assert.Equal(400, BadRequest.StatusCode);
            Assert.Equal("Book Id is not Valid", BadRequest.Value);


        }

        [Fact]
        public void GetAllBooks_Returns_Books()
        {
            //Arrange
            int BookId = 1;
            int UserId = 1;
            bookServiceMoq.Setup(c => c.GetAllBooks(UserId, BookId)).Returns(It.IsAny<IEnumerable<Book>>());
            //Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);

            var result = controller.GetAllBooks(UserId, BookId);
            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            bookServiceMoq.Verify(c => c.GetAllBooks(UserId, BookId), Times.Once());


        }
        [Fact]
        public void GetAllBooks_Invalid_BookId()
        {
            //Arrange
            int BookId = 1;
            int UserId = 0;
            bookServiceMoq.Setup(c => c.GetAllBooks(UserId, BookId)).Returns(It.IsAny<IEnumerable<Book>>());
            //Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);
            var result = controller.GetAllBooks(UserId, BookId);
            var BadRequest = result.Result as BadRequestObjectResult;
            // assert
            Assert.NotNull(BadRequest);
            Assert.Equal(400, BadRequest.StatusCode);
            Assert.Equal("User Id is not valid", BadRequest.Value);

        }



        [Fact]
        public void GetStatuses_Returns_Statuses()
        {
            //Arrange
            bookServiceMoq.Setup(c => c.GetStatuses()).Returns(It.IsAny<IEnumerable<Status>>());
            //Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);

            var result = controller.GetStatuses();

            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            bookServiceMoq.Verify(c => c.GetStatuses(), Times.Once());
        }

        [Fact]
        public void UpdateBookByEnvironment_Successfull()
        {
            int bookId = 1;
            int envId = 1;
            int statusId = 1;
            bookServiceMoq.Setup(c => c.UpdateBookStatus(bookId, envId, statusId)).Returns(true);
            // Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);
            var response = controller.UpdateBookByEnvironment(bookId, envId, statusId);
            // Assert
            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;
            // assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Book updated successfully", okResult.Value);
            bookServiceMoq.Verify(c => c.UpdateBookStatus(bookId, envId, statusId), Times.Once());

        }

        [Fact]
        public void UpdateBookByEnvironment_InvalidbookId_Unsuccessfull()
        {
            int bookId = 0;
            int envId = 1;
            int statusId = 1;
            bookServiceMoq.Setup(c => c.UpdateBookStatus(bookId, envId, statusId)).Returns(false);
            // Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);
            var response = controller.UpdateBookByEnvironment(bookId, envId, statusId);
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);

            var badResult = response as BadRequestObjectResult;
            // assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.Equal("Invalid BookID : 0 or environmentId : 1 or StatusId : 1", badResult.Value);

        }

        [Fact]
        public void UpdateBookByEnvironment_InvalidenvId_Unsuccessfull()
        {
            int bookId = 1;
            int envId = 0;
            int statusId = 1;
            bookServiceMoq.Setup(c => c.UpdateBookStatus(bookId, envId, statusId)).Returns(false);
            // Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);
            var response = controller.UpdateBookByEnvironment(bookId, envId, statusId);
            // Assert

            Assert.IsType<BadRequestObjectResult>(response);
            var badResult = response as BadRequestObjectResult;
            // assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.Equal("Something went wrong", badResult.Value);
            bookServiceMoq.Verify(c => c.UpdateBookStatus(bookId, envId, statusId), Times.Once());


        }


        [Fact]
        public void UpdateBookByEnvironment_InvalidstatusId_Unsuccessfull()
        {
            int bookId = 1;
            int envId = 1;
            int statusId = 0;
            bookServiceMoq.Setup(c => c.UpdateBookStatus(bookId, envId, statusId)).Returns(false);
            // Act
            // Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);
            var response = controller.UpdateBookByEnvironment(bookId, envId, statusId);
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            var badResult = response as BadRequestObjectResult;
            // assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.Equal("Something went wrong", badResult.Value);
            bookServiceMoq.Verify(c => c.UpdateBookStatus(bookId, envId, statusId), Times.Once());


        }

        [Fact]
        public void UpdateBookByEnvironment_InvalidstatusId_InvalidenvId_Unsuccessfull()
        {
            int bookId = 1;
            int envId = 0;
            int statusId = 0;
            bookServiceMoq.Setup(c => c.UpdateBookStatus(bookId, envId, statusId)).Returns(false);
            // Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);
            var response = controller.UpdateBookByEnvironment(bookId, envId, statusId);
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            var badResult = response as BadRequestObjectResult;
            // assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.Equal("Something went wrong", badResult.Value);
            bookServiceMoq.Verify(c => c.UpdateBookStatus(bookId, envId, statusId), Times.Once());


        }

        [Fact]
        public void UpdateBookByEnvironment_InvalidstatusId_InvalidBookId_Unsuccessfull()
        {
            int bookId = 0;
            int envId = 1;
            int statusId = 0;
            bookServiceMoq.Setup(c => c.UpdateBookStatus(bookId, envId, statusId)).Returns(false);
            // Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);
            var response = controller.UpdateBookByEnvironment(bookId, envId, statusId);
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            var badResult = response as BadRequestObjectResult;
            // assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);


        }

        [Fact]
        public void UpdateBookByEnvironment_InvalidenvId_InvalidBookId_Unsuccessfull()
        {
            int bookId = 0;
            int envId = 0;
            int statusId = 1;
            bookServiceMoq.Setup(c => c.UpdateBookStatus(bookId, envId, statusId)).Returns(false);
            // Act
            var controller = new BookController(logger.Object, bookServiceMoq.Object, userService.Object, mailService.Object, taskService.Object);
            var response = controller.UpdateBookByEnvironment(bookId, envId, statusId);
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            var badResult = response as BadRequestObjectResult;
            // assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.Equal("Invalid BookID : 0 or environmentId : 0 or StatusId : 1", badResult.Value);


        }



    }

}