using System;
using Xunit;
using Moq;
using Runbook.Services.Interfaces;
using Runbook.Models;
using System.Collections.Generic;
using Runbook.API.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Runbook.Test
{
    public class UserControllerTest
    {
        private readonly Mock<IUserService> userServiceMoq;
        private readonly Mock<ILogger<UserController>> logger;
        private readonly Mock<IMailService> mailService;

        public UserControllerTest()
        {
            userServiceMoq = new Mock<IUserService>();
            logger = new Mock<ILogger<UserController>>();
            mailService = new Mock<IMailService>();
        }

        [Fact]
        public void GetTenant_Successfull()
        {
            int tenantId = 1;
            Tenant tenant = new Tenant { TenantId = 2, Domain = "domain1", TenantName ="quinnox"};
            userServiceMoq.Setup(c => c.GetTenant(tenantId)).Returns(tenant);
            // Act
            var controller = new UserController(logger.Object, userServiceMoq.Object, mailService.Object);
            var result = controller.GetTenant(tenantId);

            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            userServiceMoq.Verify(c => c.GetTenant(tenantId), Times.Once());
        }

        [Fact]
        public void GetTenant_Successfull_201statuscheck()
        {
            int tenantId = 1;
            Tenant tenant = null;
            userServiceMoq.Setup(c => c.GetTenant(tenantId)).Returns(tenant);
            // Act
            var controller = new UserController(logger.Object, userServiceMoq.Object, mailService.Object);
            var result = controller.GetTenant(tenantId);

            // assert
            var statusResult = result.Result as ObjectResult;
            Assert.NotNull(statusResult);
            Assert.Equal(201, statusResult.StatusCode);
            Assert.Equal("No Tenants for this userId", statusResult.Value);
            userServiceMoq.Verify(c => c.GetTenant(tenantId), Times.Once());
        }

        [Fact]
        public void GetTenant_badrequest()
        {
            int tenantId = 0;
           
            // Act
            var controller = new UserController(logger.Object, userServiceMoq.Object, mailService.Object);
            var result = controller.GetTenant(tenantId);

            // assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var badResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.Equal($"Invalid UserId : {tenantId}", badResult.Value);
        }

        [Fact]
        public void GetAllUsers_Returns_Users()
        {
            //Arrange
            IEnumerable<User> objUserlist = new List<User>() { new User { UserId = 1 }, new User { UserId = 2 } };
            int tenantId = 1;
            userServiceMoq.Setup(c => c.GetAllUsers(tenantId)).Returns(objUserlist);

            //Act
            var controller = new UserController(logger.Object, userServiceMoq.Object,mailService.Object);
            var result = controller.GetAllUsers(tenantId);
            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(objUserlist, okResult.Value);
            userServiceMoq.Verify(c => c.GetAllUsers(tenantId), Times.Once());
        }
        [Fact]
        public void GetAllUsers_ReturnsNull_Users()
        {
            //Arrange
            IEnumerable<User> retVal = null;
            int tenantId = 1;
            userServiceMoq.Setup(c => c.GetAllUsers(tenantId)).Returns(retVal);

            //Act
            var controller = new UserController(logger.Object, userServiceMoq.Object, mailService.Object);
            var result = controller.GetAllUsers(tenantId);
            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(retVal, okResult.Value);
            userServiceMoq.Verify(c => c.GetAllUsers(tenantId), Times.Once());
        }

        [Fact]
        public void GetAllTask_Invalid_StageIdId()
        {
            //Arrange
            int tenantId = 0;

            //Act
            var controller = new UserController(logger.Object, userServiceMoq.Object, mailService.Object);
            var result = controller.GetAllUsers(tenantId);


            var BadRequest = result.Result as BadRequestObjectResult;
            // assert
            Assert.NotNull(BadRequest);
            Assert.Equal(400, BadRequest.StatusCode);
            Assert.Equal($"Invalid tenantId : {tenantId}", BadRequest.Value);
        }
        [Fact]
        public void SendEmail() 
        {

            string email = "quinnox@test.com";
            string subject = "salary"; 
            string body = "salary for may";
            mailService.Setup(c => c.SendEmail(email, subject, body));
            //Act
            var controller = new UserController(logger.Object, userServiceMoq.Object, mailService.Object);
            var result = controller.SendEMail(email);
            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Email sent successfully", okResult.Value);
        }
    }
}
