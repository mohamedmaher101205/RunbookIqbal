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
    public class TaskControllerTest
    {
        private readonly Mock<ITaskService> taskServiceMoq;
        private readonly Mock<ILogger<TaskController>> logger;

        public TaskControllerTest()
        {
            taskServiceMoq = new Mock<ITaskService>();
            logger = new Mock<ILogger<TaskController>>();
        }

        [Fact]
        public void CreateTask_Successfull()
        {            
            Task task = new Task
            { 
                TaskId = 8,
                TaskName = "Scripts deployment",
                StageId = 5,
                Description = "Database scripts has to be deployed",
                CompletedByDate = DateTime.Now,
                AssignedTo = "Mohammed",
                StatusId = 1,
                ReleaseNote = "release on 12.9.2020", 
                StageName = "Pre-requisite"
            };
            int bookId = 1;
            string stageName = "Pre-requisite";
            taskServiceMoq.Setup(c => c.CreateTask(task, stageName, bookId)).Returns(true);

            // Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var response = controller.CreateTask(task, bookId) as OkObjectResult;
            
            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Task created successfully", response.Value);
            taskServiceMoq.Verify(c => c.CreateTask(task, stageName, bookId),Times.Once);
        }

        [Fact]
        public void CreateTask_UnSuccessfull()
        {
            Task task = new Task
            {
                TaskId = 8,
                TaskName = "Scripts deployment",
                StageId = 5,
                Description = "Database scripts has to be deployed",
                CompletedByDate = DateTime.Now,
                AssignedTo = "Mohammed",
                StatusId = 1,
                ReleaseNote = "release on 12.9.2020",
                StageName = "Pre-requisite"
            };
            int bookId = 1;
            string stageName = "Pre-requisite";
            taskServiceMoq.Setup(c => c.CreateTask(task, stageName, bookId)).Returns(false);

            // Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var response = controller.CreateTask(task, bookId) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Unsuccessful while creating Task", response.Value);
            taskServiceMoq.Verify(c => c.CreateTask(task, stageName, bookId), Times.Once);
        }


        [Fact]
        public void CreateTask_EmptyTaskName_ReturnsBadRequest()
        {
            // Arrange
            Task task = new Task
            {
                TaskId = 8,
                TaskName = "",
                StageId = 5,
                Description = "Database scripts has to be deployed",
                CompletedByDate = DateTime.Now,
                AssignedTo = "Mohammed",
                StatusId = 1,
                ReleaseNote = "release on 12.9.2020",
                StageName = "Pre-requisite"
            };
            int bookId = 1;

            // Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var response = controller.CreateTask(task, bookId);           
        
            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public void GetAlltasks_Returns_tasks()
        {
            //Arrange
            int stageId = 1;
            taskServiceMoq.Setup(c => c.GetAllTasks(stageId)).Returns(It.IsAny<IEnumerable<Task>>());

            //Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var result = controller.GetAllTasks(stageId);
            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            taskServiceMoq.Verify(c => c.GetAllTasks(stageId), Times.Once());
        }

        [Fact]
        public void GetAllTask_Invalid_StageIdId()
        {
            //Arrange
            int stageId = 0;

            //Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var result = controller.GetAllTasks(stageId);

  
            var BadRequest = result.Result as BadRequestObjectResult;
            // assert
            Assert.NotNull(BadRequest);
            Assert.Equal(400, BadRequest.StatusCode);
            Assert.Equal("Invalid stageId : 0", BadRequest.Value);
        }

        [Fact]
        public void UpdateTaskStatus_ReturnsSuccess()
        {
            //Arrange
            int statusId = 5;
            string ids = "1,5,7";
            int[] taskids = new int[] { 1,5,7 };
            taskServiceMoq.Setup(c => c.UpdateTaskStatus(taskids, statusId)).Returns(true);

            //Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var result = controller.UpdateTaskStatus(ids, statusId);

            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            taskServiceMoq.Verify(c => c.UpdateTaskStatus(taskids, statusId),Times.Once);
        }
        [Fact]
        public void UpdateTaskStatus_Failure_ReturnsFailure()
        {
            //Arrange
            int statusId = 0;
            string ids = "1,5,7";
            int[] taskids = new int[] { 1, 5, 7 };
            taskServiceMoq.Setup(c => c.UpdateTaskStatus(taskids, statusId)).Returns(true);

            //Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var result = controller.UpdateTaskStatus(ids, statusId);

            // assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var badResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.Equal("Invalid StatusId : 0 or Task Ids : 1,5,7", badResult.Value);
        }

        [Fact]
        public void subscribeTask_ReturnsSuccess()
        {
            //Arrange
            int taskId =1; string emailId = "mdicpal@gmail.com";
            bool success = true;
            taskServiceMoq.Setup(c => c.subscribeTask(taskId, emailId)).Returns(success);

            //Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var result = controller.subscribeTask(taskId, emailId);

            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(success, okResult.Value);
            taskServiceMoq.Verify(c => c.subscribeTask(taskId, emailId), Times.Once);
        }
        [Fact]
        public void subscribeTask_Failure_ReturnsFailure()
        {
            //Arrange
            int taskId = 0; string emailId = "";
            bool fail = false;
            taskServiceMoq.Setup(c => c.subscribeTask(taskId, emailId)).Returns(fail);

            //Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var result = controller.subscribeTask(taskId, emailId);
            // assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var badResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.Equal($"Invalid taskid : {taskId} or email Ids : {emailId}", badResult.Value);
        }
        [Fact]
        public void DeleteTask_Successfull()
        {
            int bookId = 1;
            string taskName = "Scripts deployment";
            int rowsDeleted = 10;
            taskServiceMoq.Setup(c => c.DeleteTasks(bookId,taskName)).Returns(rowsDeleted);

            // Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var response = controller.DeleteTask(bookId, taskName) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal($"{rowsDeleted} Tasks are deleted", response.Value);
            taskServiceMoq.Verify(c => c.DeleteTasks(bookId, taskName), Times.Once);
        }

        [Fact]
        public void DeleteTask_UnSuccessfull()
        {
            int bookId = 0;
            string taskName = "Scripts deployment";
            int rowsDeleted = 10;
            taskServiceMoq.Setup(c => c.DeleteTasks(bookId, taskName)).Returns(rowsDeleted);

            // Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var response = controller.DeleteTask(bookId, taskName) as BadRequestObjectResult;

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid BookId : {bookId} or TaskName : {taskName}", response.Value);
        }

        [Fact]
        public void UpdateTask_Successfull()
        {
            Task task = new Task
            {
                TaskId = 8,
                TaskName = "Scripts deployment",
                StageId = 5,
                Description = "Database scripts has to be deployed",
                CompletedByDate = DateTime.Now,
                AssignedTo = "Mohammed",
                StatusId = 1,
                ReleaseNote = "release on 12.9.2020",
                StageName = "Pre-requisite",
                Subscribers = "smanjunath@quinnox.com,ramkumarm@quinnox.com,anishas@quinnox.com"
            };
            int taskId = 1;
            int taskUpdated = 10;
            taskServiceMoq.Setup(c => c.UpdateTask(taskId, task)).Returns(taskUpdated);

            // Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var response = controller.UpdateTask(task, taskId) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Task updated successfully", response.Value);
            taskServiceMoq.Verify(c => c.UpdateTask(taskId, task), Times.Once);
        }

        [Fact]
        public void UpdateTask_Successfull_withNotUpdated()
        {
            Task task = new Task
            {
                TaskId = 8,
                TaskName = "Scripts deployment",
                StageId = 5,
                Description = "Database scripts has to be deployed",
                CompletedByDate = DateTime.Now,
                AssignedTo = "Mohammed",
                StatusId = 1,
                ReleaseNote = "release on 12.9.2020",
                StageName = "Pre-requisite", Subscribers  = "mohammeds@quinnox.com"
            };
            int taskId = 1;
            int taskUpdated = 0;
            taskServiceMoq.Setup(c => c.UpdateTask(taskId, task)).Returns(taskUpdated);

            // Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var response = controller.UpdateTask(task, taskId) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Task updation unsuccessfull", response.Value);
            taskServiceMoq.Verify(c => c.UpdateTask(taskId, task), Times.Once);
        }
        [Fact]
        public void UpdateTask_UnSuccessfull()
        {
            Task task = new Task
            {
                TaskId = 8,
                TaskName = "Scripts deployment",
                StageId = 5,
                Description = "Database scripts has to be deployed",
                CompletedByDate = DateTime.Now,
                AssignedTo = "Mohammed",
                StatusId = 1,
                ReleaseNote = "release on 12.9.2020",
                StageName = "Pre-requisite",
                Subscribers = "mohammeds@quinnox.com"
            };
            int taskId = 0;
            int taskUpdated = 10;
            taskServiceMoq.Setup(c => c.UpdateTask(taskId, task)).Returns(taskUpdated);

            // Act
            var controller = new TaskController(logger.Object, taskServiceMoq.Object);
            var response = controller.UpdateTask(task, taskId) as BadRequestObjectResult;

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid TaskId : {taskId}", response.Value);
        }

    }
}
