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
    public class TeamControllerTest
    {
        private readonly Mock<ITeamService> teamServiceMoq;
        private readonly Mock<ILogger<TeamController>> logger;

        public TeamControllerTest()
        {
            teamServiceMoq = new Mock<ITeamService>();
            logger = new Mock<ILogger<TeamController>>();
        }

        [Fact]
        public async void CreateTeam_Successful()
        {
            //Given
            Team team = new Team()
            {
                TeamName = "Quinnox",
                Description = "Test team",
                TenantId = 1
            };
            int teamCreatedCount = 1;
            teamServiceMoq.Setup(t => t.CreateTeam(team)).ReturnsAsync(teamCreatedCount);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.CreateTeam(team) as OkObjectResult;
            
            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Team created successfully",response.Value);
            teamServiceMoq.Verify(e => e.CreateTeam(team),Times.Once);
        }

        [Fact]
        public async void CreateTeam_UnSuccessful()
        {
            //Given
            Team team = new Team()
            {
                TeamName = "Quinnox",
                Description = "Test team",
                TenantId = 1
            };
            int teamCreatedCount = 0;
            teamServiceMoq.Setup(e => e.CreateTeam(team)).ReturnsAsync(teamCreatedCount);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.CreateTeam(team) as NotFoundObjectResult;
            
            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal("Unsuccessfull while creating the Team",response.Value);
            teamServiceMoq.Verify(e => e.CreateTeam(team),Times.Once);
        }

        [Fact]
        public async void CreateTeam_Invalid_TeamName_Or_TenantId()
        {
            //Given
            Team team = new Team()
            {
                TeamName = null,
                Description = "Test team",
                TenantId = 0
            };
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.CreateTeam(team) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid Team Name : {team.TeamName} or TenantId : {team.TenantId}",response.Value);
        }

        [Fact]
        public async void CreateTeam_Duplicate_TeamName()
        {
            //Given
            Team team = new Team()
            {
                TeamName = "Quinnox",
                Description = "Test team",
                TenantId = 1
            };
            teamServiceMoq.Setup(e => e.CreateTeam(team)).ThrowsAsync(new Exception("Team with same name exist"));
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.CreateTeam(team);
            
            //Then
            Assert.IsType<ConflictObjectResult>(response);
            //Assert.Equal($"Team {team.TeamName} already exist for the Tenant",response.Value);
        }

        [Fact]
        public async void GetAllTeams_Successfull()
        {
            //Given
            int tenantId = 1;
            var team = new Team() {TeamName = "Quinnox" , TeamId = 1};
            var teams = new List<Team>() {team};
            teamServiceMoq.Setup(e => e.GetAllTeams(tenantId)).ReturnsAsync(teams);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.GetAllTeams(tenantId);

            //Then
            Assert.IsType<OkObjectResult>(response);
           // Assert.NotNull(response.Value);
            teamServiceMoq.Verify(e => e.GetAllTeams(tenantId),Times.Once);
        }

        [Fact]
        public async void GetAllEnvironments_Invalid_TenantId()
        {
            //Given
            int tenantId = 0;
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.GetAllTeams(tenantId) as BadRequestObjectResult;

            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid TenantId : {tenantId}",response.Value);
        }

        [Fact]
        public async void GetAllEnvironments_NotFound()
        {
            //Given
            int tenantId = 100;
            IEnumerable<Team> teams = null;
            teamServiceMoq.Setup(e => e.GetAllTeams(tenantId)).ReturnsAsync(teams);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.GetAllTeams(tenantId) as NotFoundObjectResult;

            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"No teams found for the tenantId : {tenantId}",response.Value);
        }
    }
}
