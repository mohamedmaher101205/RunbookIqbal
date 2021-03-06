using System.Net;
using System;
using Xunit;
using Moq;
using Runbook.Services.Interfaces;
using Runbook.Models;
using Runbook.API.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Runbook.Test
{
    public class AuthControllerTest
    {
        private readonly Mock<IAuthService> authServiceMoq;
        private readonly Mock<ILogger<AuthController>> logger;

         private readonly Mock<IMailService> mailService;

        public AuthControllerTest()
        {
            authServiceMoq = new Mock<IAuthService>();
            logger = new Mock<ILogger<AuthController>>();
             mailService = new Mock<IMailService>();
        }

        [Fact]
        public void Login_Success_For_App_users()
        {
            // Arrange
            User user = new User{
                UserEmail = "Xunit@test.com",
                Password = "Xunit"
            };
            AuthRequest token = new AuthRequest{
                Token = "some.test.token",
                ExpiresIn = DateTime.Now.AddHours(1)
            };
            authServiceMoq.Setup(c => c.AuthenticateUser(user)).Returns(token);
            
            // Act
            var controller = new AuthController(authServiceMoq.Object,logger.Object,mailService.Object);
            var response = controller.Login(user) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(response.Value);
            authServiceMoq.Verify(c => c.AuthenticateUser(user),Times.Once);
        }

        [Fact]
        public void Login_Success_For_OpenId_users()
        {
            // Arrange
            User user = new User{
                UserEmail = "Xunit@test.com",
                Password = ""
            };
            AuthRequest token = new AuthRequest{
                Token = "some.test.token",
                ExpiresIn = DateTime.Now.AddHours(1)
            };
            authServiceMoq.Setup(c => c.OpenIdAuthenticateUser(user)).Returns(token);
            
            // Act
            var controller = new AuthController(authServiceMoq.Object,logger.Object,mailService.Object);
            var response = controller.Login(user) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(response.Value);
            authServiceMoq.Verify(c => c.OpenIdAuthenticateUser(user),Times.Once);
        }

        [Fact]
        public void Login_Unauthorized()
        {
            // Arrange
            User user = new User{
                UserEmail = "Xunit@test.com",
                Password = "WrongPassword"
            };
            AuthRequest token = new AuthRequest{
                Token = null,
            };
            authServiceMoq.Setup(c => c.AuthenticateUser(user)).Returns(token);
            
            // Act
            var controller = new AuthController(authServiceMoq.Object,logger.Object,mailService.Object);
            var response = controller.Login(user);
            
            // Assert
            Assert.IsType<UnauthorizedResult>(response);
            authServiceMoq.Verify(c => c.AuthenticateUser(user),Times.Once);
        }

        [Fact]
        public void Login_Empty_EmailId()
        {
            // Arrange
            User user = new User{
                UserEmail = "",
                Password = "WrongPassword"
            };
              var expectedValue = "User exist";
            // Act
            var controller = new AuthController(authServiceMoq.Object,logger.Object,mailService.Object);
            var response = controller.Login(user) as BadRequestObjectResult;
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Email should not be empty",response.Value);
        }

            [Fact]
        public void Register_Successful()
        {
            // Arrange
            User user = new User{
                UserEmail = "Xunit@test.com",
                Password = "Password",
                FirstName = "X",
                LastName = "Unit",
            };
            var expectedValue = "successfull";
            authServiceMoq.Setup(c => c.RegisterUser(user,out expectedValue)).Returns(It.IsAny<IEnumerable<InviteUsers>>());
            
            // Act
            var controller = new AuthController(authServiceMoq.Object,logger.Object,mailService.Object);
            var response = controller.RegisterUser(user) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200,response.StatusCode);
            authServiceMoq.Verify(c => c.RegisterUser(user,out expectedValue),Times.Once);
        }
        
        [Fact]
        public void Register_UserWithSameEMail_Exist()
        {
            // Arrange
            User user = new User{
                UserEmail = "Xunit@test.com",
                Password = "Password",
                FirstName = "X",
                LastName = "Unit",
            };
            var expectedValue = "User exist";
            authServiceMoq.Setup(c => c.RegisterUser(user,out expectedValue)).Returns(It.IsAny<IEnumerable<InviteUsers>>());
            
            
            // Act
            var controller = new AuthController(authServiceMoq.Object,logger.Object,mailService.Object);
            var response = controller.RegisterUser(user) as ConflictObjectResult;
            
            // Assert
            Assert.IsType<ConflictObjectResult>(response);
            Assert.Equal("User with same email already exist",response.Value);
            authServiceMoq.Verify(c => c.RegisterUser(user,out expectedValue),Times.Once);
        }

        [Fact]
        public void Register_UserEMailOrPassword_ShouldNotBeEmpty()
        {
            // Arrange
            User user = new User{
                UserEmail = "",
                Password = "",
                FirstName = "X",
                LastName = "Unit",
            };
            
            // Act
            var controller = new AuthController(authServiceMoq.Object,logger.Object,mailService.Object);
            var response = controller.RegisterUser(user) as BadRequestObjectResult;
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("User email or password should not be empty",response.Value);
        }

         [Fact]
         public void ForgotPassword_Successful()
        {
            // Arrange
            User user = new User{
                UserEmail = "Xunit@test.com",
                Password = "",
                FirstName = "",
                LastName = "",
            };
            authServiceMoq.Setup(c => c.checkExistingUser(user)).Returns(true);
            
            // Act
            var controller = new AuthController(authServiceMoq.Object,logger.Object,mailService.Object);
            var response = controller.ForgotPasswordSendOTP(user) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200,response.StatusCode);
            authServiceMoq.Verify(c => c.checkExistingUser(user),Times.Once);
        }

        [Fact]
         public void ForgotPassword_UnSuccessful()
        {
            // Arrange
            User user = new User{
                UserEmail = "Xunit@test.com",
                Password = "",
                FirstName = "",
                LastName = "",
            };
            authServiceMoq.Setup(c => c.checkExistingUser(user)).Returns(false);
            
            // Act
             var controller = new AuthController(authServiceMoq.Object,logger.Object,mailService.Object);
            var response = controller.ForgotPasswordSendOTP(user) as BadRequestObjectResult;
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("User email doesn't exist",response.Value);
        }

        [Fact]
          public void ResetPassword_Successful()
        {
            // Arrange
            User user = new User{
                UserEmail = "Xunit@test.com",
                Password = "",
                FirstName = "",
                LastName = "",
            };
            authServiceMoq.Setup(c => c.ResetPassword(user)).Returns("successfull");
            
            // Act
            var controller = new AuthController(authServiceMoq.Object,logger.Object,mailService.Object);
            var response = controller.ResetPassword(user) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200,response.StatusCode);
            authServiceMoq.Verify(c => c.ResetPassword(user),Times.Once);
        }

        [Fact]
         public void ResetPassword_UnSuccessful()
        {
            // Arrange
            User user = new User{
                UserEmail = "Xunit@test.com",
                Password = "",
                FirstName = "",
                LastName = "",
            };
              authServiceMoq.Setup(c => c.ResetPassword(user)).Returns("UserNotExist");
            
            // Act
             var controller = new AuthController(authServiceMoq.Object,logger.Object,mailService.Object);
                var response = controller.ResetPassword(user) as BadRequestObjectResult;
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("User Not exist",response.Value);
        }
        
    }
}
