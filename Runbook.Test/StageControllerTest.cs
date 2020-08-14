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
    public class StageControllerTest
    {
        private readonly Mock<IStageService> stageServiceMoq;
        private readonly Mock<ILogger<StageController>> logger;

        public StageControllerTest()
        {
            stageServiceMoq = new Mock<IStageService>();
            logger = new Mock<ILogger<StageController>>();
        }

        [Fact]
        public void CreateStage_Successfull()
        {
            Stage stage = new Stage { BookId = 1, StageId = 3, Description = "desc", EnvId = 2, StageName = "prod", StatusId = 2 };
            int bookId = 1;
            stageServiceMoq.Setup(c => c.CreateStage(stage,bookId)).Returns(true);

            // Act
            var controller = new StageController(logger.Object, stageServiceMoq.Object);
            var response = controller.CreateStage(stage, bookId) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Stage created successfully", response.Value);
            stageServiceMoq.Verify(c => c.CreateStage(stage, bookId),Times.Once);
        }

        [Fact]
        public void CreateStage_UnSuccessfull()
        {
            Stage stage = new Stage { BookId = 1, StageId = 3, Description = "desc", EnvId = 2, StageName = "prod", StatusId = 2 };
            int bookId = 1;
            stageServiceMoq.Setup(c => c.CreateStage(stage, bookId)).Returns(false);

            // Act
            var controller = new StageController(logger.Object, stageServiceMoq.Object);
            var response = controller.CreateStage(stage, bookId) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Unsuccessful while creating stage", response.Value);
            stageServiceMoq.Verify(c => c.CreateStage(stage, bookId), Times.Once);
        }

        [Fact]
        public void Createstage_Invalid_BookIdOrStageName_ReturnsBadRequest()
        {
            // Arrange
            Stage stage = new Stage { BookId = 1, StageId = 3, Description = "desc", EnvId = 2, StageName = null, StatusId = 2 };
            int bookId = 0;

            // Act
            var controller = new StageController(logger.Object, stageServiceMoq.Object);
            var response = controller.CreateStage(stage, bookId) as BadRequestObjectResult;

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Invalid BookId or Stage Name", response.Value);
        }

        [Fact]
        public void GetAllStages_Returns_tasks()
        {
            IEnumerable<Stage> objstageList = new List<Stage> { new Stage { StageId =1, }, new Stage { StageId =2 } };
            //Arrange
            int bookId = 1;
            int envId = 1;
            stageServiceMoq.Setup(c => c.GetAllStages(bookId,envId)).Returns(objstageList);

            //Act
            var controller = new StageController(logger.Object, stageServiceMoq.Object);
            var result = controller.GetAllStages(bookId, envId);
            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(objstageList, okResult.Value);
            stageServiceMoq.Verify(c => c.GetAllStages(bookId, envId), Times.Once());
        }

        [Fact]
        public void GetAllTask_Invalid_StageIdId()
        {
            //Arrange
            int bookId = 0;
            int envId = 0;

            //Act
            var controller = new StageController(logger.Object, stageServiceMoq.Object);
            var result = controller.GetAllStages(bookId, envId);
            var BadRequest = result.Result as BadRequestObjectResult;
            // assert
            Assert.NotNull(BadRequest);
            Assert.Equal(400, BadRequest.StatusCode);
            Assert.Equal($"Invalid Book Id : {bookId} or Environment Id : {envId}", BadRequest.Value);
        }

        [Fact]
        public void UpdateStageStatus_ReturnsSuccess()
        {
            //Arrange 
            int stageId = 1;
            int nextStageId = 1;
            int statusId = 1;
            stageServiceMoq.Setup(c => c.UpdateStageStatus(stageId, nextStageId, statusId)).Returns(true);

            //Act
            var controller = new StageController(logger.Object, stageServiceMoq.Object);
            var result = controller.UpdateStageStatus(stageId, nextStageId, statusId);

            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(true, okResult.Value);
            stageServiceMoq.Verify(c => c.UpdateStageStatus(stageId, nextStageId, statusId), Times.Once);
        }
        [Fact]
        public void UpdateStageStatus_Failure_ReturnsFailure()
        {
            //Arrange
            int stageId = 0;
            int nextStageId = 0;
            int statusId = 0;
            stageServiceMoq.Setup(c => c.UpdateStageStatus(stageId, nextStageId, statusId)).Returns(false);

            //Act
            var controller = new StageController(logger.Object, stageServiceMoq.Object);
            var result = controller.UpdateStageStatus(stageId, nextStageId, statusId);

            // assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var badResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.Equal($"Invalid stageId : {stageId} or nextStageId : {nextStageId} or StatusId : {statusId}", badResult.Value);
        }
    }
}
