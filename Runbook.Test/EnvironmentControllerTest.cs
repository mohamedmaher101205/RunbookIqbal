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
    public class EnvironmentControllerTest
    {
        private readonly Mock<IEnvironmentService> environmentServiceMoq;
        private readonly Mock<ILogger<EnvironmentController>> logger;

        public EnvironmentControllerTest()
        {
            environmentServiceMoq = new Mock<IEnvironmentService>();
            logger = new Mock<ILogger<EnvironmentController>>();
        }

        [Fact]
        public void CreateEnvironment_Successful()
        {
            //Given
            Environments env = new Environments{
                Environment = "Custom Dev"
            };
            int tenantId = 1, envCreatedCount = 1;
            environmentServiceMoq.Setup(e => e.CreateEnvironment(env,tenantId)).Returns(envCreatedCount);
            
            //When
            var controller = new EnvironmentController(logger.Object,environmentServiceMoq.Object);
            var response = controller.CreateCustomEnvironment(env,tenantId) as OkObjectResult;
            
            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal($"{envCreatedCount} Environments inserted",response.Value);
            environmentServiceMoq.Verify(e => e.CreateEnvironment(env,tenantId),Times.Once);
        }

        [Fact]
        public void CreateEnvironment_UnSuccessful()
        {
            //Given
            Environments env = new Environments{
                Environment = "Custom Dev"
            };
            int tenantId = 1, envCreatedCount = 0;
            environmentServiceMoq.Setup(e => e.CreateEnvironment(env,tenantId)).Returns(envCreatedCount);
            
            //When
            var controller = new EnvironmentController(logger.Object,environmentServiceMoq.Object);
            var response = controller.CreateCustomEnvironment(env,tenantId) as NotFoundObjectResult;
            
            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"Environments failed to insert",response.Value);
            environmentServiceMoq.Verify(e => e.CreateEnvironment(env,tenantId),Times.Once);
        }

        [Theory]
        [InlineData("",1)]
        [InlineData("Custom dev",0)]
        public void CreateEnvironment_Invalid_EnvironmentName_Or_TenantId(string environmentName,int tenantId)
        {
            //Given
            Environments env = new Environments{
                Environment = environmentName
            };
            
            //When
            var controller = new EnvironmentController(logger.Object,environmentServiceMoq.Object);
            var response = controller.CreateCustomEnvironment(env,tenantId) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid TenantId : {tenantId} or environment name : {env.Environment}",response.Value);
        }

        [Fact]
        public void GetAllEnvironments_Successfull()
        {
            //Given
            int tenantId = 1;
            var environment = new Environments() {Environment = "Devlopment" , EnvId = 1};
            var envs = new List<Environments>() {environment};
            environmentServiceMoq.Setup(e => e.GetAllEnvironments(tenantId)).Returns(envs);
            
            //When
            var controller = new EnvironmentController(logger.Object,environmentServiceMoq.Object);
            var response = controller.GetAllEnvironments(tenantId) as OkObjectResult;

            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(response.Value);
            environmentServiceMoq.Verify(e => e.GetAllEnvironments(tenantId),Times.Once);
        }

        [Fact]
        public void GetAllEnvironments_Invalid_TenantId()
        {
            //Given
            int tenantId = 0;
            
            //When
            var controller = new EnvironmentController(logger.Object,environmentServiceMoq.Object);
            var response = controller.GetAllEnvironments(tenantId) as NotFoundObjectResult;

            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"Invalid tenantId : {tenantId}",response.Value);
        }
    }
}
