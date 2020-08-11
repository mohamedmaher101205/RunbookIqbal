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
    public class ApplicationControllerTest
    {
        private readonly Mock<IApplicationService> applicationServiceMoq;
        private readonly Mock<ILogger<ApplicationController>> logger;

        public ApplicationControllerTest()
        {
            applicationServiceMoq = new Mock<IApplicationService>();
            logger = new Mock<ILogger<ApplicationController>>();
        }

        [Fact]
        public void CreateApplication_Successfull()
        {
            // Arrange
            Application app = new Application{
                ApplicationName = "Test",
                Description = "Xunit test",
                BookId = 3,
                AppTypeName = "Applicaion server"
            };
            int tenantId = 1;
            applicationServiceMoq.Setup(c => c.CreateApplication(app, tenantId)).Returns(true);

            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.CreateApplication(app,tenantId) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Application created successfully",response.Value);
            applicationServiceMoq.Verify(c => c.CreateApplication(app,tenantId),Times.Once);
        }

        [Fact]
        public void CreateApplication_Unsuccessfull()
        {
            // Arrange
            Application app = new Application{
                ApplicationName = "Test",
                Description = "Xunit test",
                BookId = 3,
                AppTypeName = "Applicaion server"
            };
            int tenantId = 1;
            applicationServiceMoq.Setup(c => c.CreateApplication(app, tenantId)).Returns(false);
            
            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.CreateApplication(app,tenantId) as BadRequestObjectResult;
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Application Not created",response.Value);
            applicationServiceMoq.Verify(c => c.CreateApplication(app,tenantId),Times.Once);
        }

        [Fact]
        public void CreateApplication_EmptyApplicationName_ReturnsBadRequest()
        {
            // Arrange
            Application app = new Application{
                ApplicationName = "",
                Description = "Xunit test",
                BookId = 3,
                AppTypeName = "Applicaion server"
            };
            int tenantId = 1;

            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.CreateApplication(app,tenantId);
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public void GetAllApplication_Returns_Applications()
        {
            //Arrange
            int tenantId = 1;
            applicationServiceMoq.Setup(c => c.GetAllApplications(tenantId)).Returns(It.IsAny<IEnumerable<Application>>());

            //Act
            var controller = new ApplicationController(logger.Object ,applicationServiceMoq.Object);
            var result = controller.GetAllApplications(tenantId) as OkObjectResult;

            //Assert
            Assert.IsType<OkObjectResult>(result);
            applicationServiceMoq.Verify(c => c.GetAllApplications(tenantId),Times.Once);
        }

        [Fact]
        public void GetAllApplication_Invalid_TenantId()
        {
            //Arrange
            int tenantId = 0;

            //Act
            var controller = new ApplicationController(logger.Object ,applicationServiceMoq.Object);
            var response = controller.GetAllApplications(tenantId) as BadRequestObjectResult;

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid TenantId : {tenantId}",response.Value);
        }

        [Fact]
        public void GetApplicationTypes_Success()
        {
            // Arrange
            int tenantId = 1;
            applicationServiceMoq.Setup(c => c.GetApplicationTypes(tenantId)).Returns(It.IsAny<IEnumerable<ApplicationType>>);
            
            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.GetApplicationTypes(tenantId) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            applicationServiceMoq.Verify(c => c.GetApplicationTypes(tenantId),Times.Once);
        }

        [Fact]
        public void GetApplicationTypes_Invalid_TenantId()
        {
            // Arrange
            int tenantId = 0;
            
            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.GetApplicationTypes(tenantId) as BadRequestObjectResult;
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid TenantId : ${tenantId}",response.Value);
        }

        [Fact]
        public void AddApplicationsToBook_Successfull()
        {
            // Arrange
            int bookId = 1;
            string applicationIds = "1,2,3";
            int rowsInserted = 2;
            int[] appIds = Array.ConvertAll(applicationIds.Split(','),int.Parse);
            applicationServiceMoq.Setup(c => c.AddApplications(bookId,appIds)).Returns(rowsInserted);
            
            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.AddApplicationToBook(bookId,applicationIds) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal($"Inserted ${rowsInserted} rows",response.Value);
            applicationServiceMoq.Verify(c => c.AddApplications(bookId,appIds),Times.Once);
        }

        [Fact]
        public void AddApplicationsToBook_Unsuccessfull()
        {
            // Arrange
            int bookId = 1;
            string applicationIds = "1,2,3";
            int rowsInserted = 0;
            int[] appIds = Array.ConvertAll(applicationIds.Split(','),int.Parse);
            applicationServiceMoq.Setup(c => c.AddApplications(bookId,appIds)).Returns(rowsInserted);
            
            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.AddApplicationToBook(bookId,applicationIds) as NotFoundObjectResult;
            
            // Assert
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal("Failed to insert",response.Value);
           applicationServiceMoq.Verify(c => c.AddApplications(bookId,appIds),Times.Once);
        }

        [Fact]
        public void AddApplicationsToBook_Invalid_BookId()
        {
            // Arrange
            int bookId = 0;
            string applicationIds = "1,2,3";
            
            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.AddApplicationToBook(bookId,applicationIds) as BadRequestObjectResult;
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid BookId : {bookId} or ApplicationIds : {applicationIds}",response.Value);
        }

        [Fact]
        public void GetApplicationByBookId_Success()
        {
            // Arrange
            int bookId = 1;
            applicationServiceMoq.Setup(c => c.GetApplicationByBookId(bookId)).Returns(It.IsAny<IEnumerable<Application>>);
            
            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.GetApplicationByBookId(bookId) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            applicationServiceMoq.Verify(c => c.GetApplicationByBookId(bookId),Times.Once);
        }

        [Fact]
        public void GetApplicationByBookId_Invalid_BookId()
        {
            // Arrange
            int bookId = 0;
            
            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.GetApplicationByBookId(bookId) as BadRequestObjectResult;
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid BookId in : {bookId}",response.Value);
        }

        [Fact]
        public void CreateApplicationType_Success()
        {
            // Arrange
            ApplicationType applicationType = new ApplicationType{
                AppTypeName = "Server"
            };
            int tenantId = 1;
            int appCreatedInt = 1;
            applicationServiceMoq.Setup(c => c.CreateCustomApplicationType(applicationType,tenantId)).Returns(appCreatedInt);
            
            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.CreateApplicationType(applicationType,tenantId) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Application Type created successfully",response.Value);
            applicationServiceMoq.Verify(c => c.CreateCustomApplicationType(applicationType,tenantId),Times.Once);
        }

        [Fact]
        public void CreateApplicationType_Failure()
        {
            // Arrange
            ApplicationType applicationType = new ApplicationType{
                AppTypeName = "Server"
            };
            int tenantId = 1;
            int appCreatedInt = 0;
            applicationServiceMoq.Setup(c => c.CreateCustomApplicationType(applicationType,tenantId)).Returns(appCreatedInt);
            
            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.CreateApplicationType(applicationType,tenantId) as BadRequestObjectResult;
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Application Type Not created",response.Value);
            applicationServiceMoq.Verify(c => c.CreateCustomApplicationType(applicationType,tenantId),Times.Once);
        }

        [Fact]
        public void CreateApplicationType_Invalid_TenantId_Or_AppName()
        {
            // Arrange
            ApplicationType applicationType = new ApplicationType{
                AppTypeName = ""
            };
            int tenantId = 0;
            
            // Act
            var controller = new ApplicationController(logger.Object,applicationServiceMoq.Object);
            var response = controller.CreateApplicationType(applicationType,tenantId) as BadRequestObjectResult;
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Empty Application name : {applicationType} Or invalid tenantId : {tenantId}",response.Value);
        }
    }
}
