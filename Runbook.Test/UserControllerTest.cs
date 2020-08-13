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
        public void GetTenant_badrequest()
        {
            int tenantId = 0;
           
            // Act
            var controller = new UserController(logger.Object, userServiceMoq.Object, mailService.Object);
            var result = controller.GetTenant(tenantId);

            // assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var okResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public void GetAllUsers_Returns_Userss()
        {
            //Arrange
            int tenantId = 1;
            userServiceMoq.Setup(c => c.GetAllUsers(tenantId)).Returns(It.IsAny<IEnumerable<User>>());

            //Act
            var controller = new UserController(logger.Object, userServiceMoq.Object,mailService.Object);
            var result = controller.GetAllUsers(tenantId);
            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
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
