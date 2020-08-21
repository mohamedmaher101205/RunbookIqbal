using System.Collections;
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
            var response = await controller.CreateTeam(team) as ConflictObjectResult;
            
            //Then
            Assert.IsType<ConflictObjectResult>(response);
            Assert.Equal($"Team {team.TeamName} already exist for the Tenant",response.Value);
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
            var response = await controller.GetAllTeams(tenantId) as OkObjectResult;
            var responseObj = response.Value as List<Team>;

            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.True(responseObj.Count > 0);
            teamServiceMoq.Verify(e => e.GetAllTeams(tenantId),Times.Once);
        }

        [Fact]
        public async void GetAllTeams_Invalid_TenantId()
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
        public async void GetAllTeams_NotFound()
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

        [Fact]
        public async void GetTeam_Successfull()
        {
            //Given
            int teamId = 1;
            Team team = new Team(){
                TeamId = 1,
                TeamName = "Avengers"
            };
            teamServiceMoq.Setup(e => e.GetTeam(teamId)).ReturnsAsync(team);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.GetTeam(teamId) as OkObjectResult;
            var responseObj = response.Value as Team;

            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(responseObj);
            teamServiceMoq.Verify(e => e.GetTeam(teamId),Times.Once);
        }

        [Fact]
        public async void GetTeam_UnSuccessfull()
        {
            //Given
            int teamId = 1;
            Team team = null;
            teamServiceMoq.Setup(e => e.GetTeam(teamId)).ReturnsAsync(team);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.GetTeam(teamId) as NotFoundObjectResult;

            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"No team found for the teamId : {teamId}",response.Value);
            teamServiceMoq.Verify(e => e.GetTeam(teamId),Times.Once);
        }

        [Fact]
        public async void GetTeam_Invalid_TeamId()
        {
            //Given
            int teamId = 0;
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.GetTeam(teamId) as BadRequestObjectResult;

            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid teamId : {teamId}",response.Value);
        }

        [Fact]
        public async void AddMembersToTeam_Successfull()
        {
            //Given
            int teamId = 1,usersAddedToTeam = 1;
            User user = new User(){UserId = 1, FirstName = "John"};
            List<User> users = new List<User>(){user};
            teamServiceMoq.Setup(e => e.AddMembersToTeam(users,teamId)).ReturnsAsync(usersAddedToTeam);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.AddMembersToTeam(users,teamId) as OkObjectResult;

            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Members Added successfully",response.Value);
            teamServiceMoq.Verify(e => e.AddMembersToTeam(users,teamId),Times.Once);
        }

        [Fact]
        public async void AddMembersToTeam_UnSuccessfull()
        {
            //Given
            int teamId = 1,usersAddedToTeam = 0;
            User user = new User(){UserId = 1, FirstName = "John"};
            List<User> users = new List<User>(){user};
            teamServiceMoq.Setup(e => e.AddMembersToTeam(users,teamId)).ReturnsAsync(usersAddedToTeam);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.AddMembersToTeam(users,teamId) as NotFoundObjectResult;

            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal("Unsuccessfull while adding members to team ",response.Value);
            teamServiceMoq.Verify(e => e.AddMembersToTeam(users,teamId),Times.Once);
        }

        [Fact]
        public async void AddMembersToTeam_Invalid_UserId_Or_TeamId()
        {
            //Given
            int teamId = 0;
            List<User> users = new List<User>();
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.AddMembersToTeam(users,teamId) as BadRequestObjectResult;

            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid Users : {users} or TeamId : {teamId}",response.Value);
        }

        [Fact]
        public async void GetTeamMembers_Successfull()
        {
            //Given
            int teamId = 1;
            var user = new User() {UserId = 1 , FirstName = "John"};
            var users = new List<User>() {user};
            teamServiceMoq.Setup(e => e.GetTeamMembers(teamId)).ReturnsAsync(users);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.GetTeamUsers(teamId) as OkObjectResult;
            var responseObj = response.Value as List<User>;

            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.True(responseObj.Count > 0);
            teamServiceMoq.Verify(e => e.GetTeamMembers(teamId),Times.Once);
        }

        [Fact]
        public async void AddMembersToTeam_NotFound()
        {
            //Given
            int teamId = 10;
            List<User> users = null;
            teamServiceMoq.Setup(e => e.GetTeamMembers(teamId)).ReturnsAsync(users);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.GetTeamUsers(teamId) as NotFoundObjectResult;
            var responseObj = response.Value as List<User>;

            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"No team users found for the teamId : {teamId}",response.Value);
            teamServiceMoq.Verify(e => e.GetTeamMembers(teamId),Times.Once);
        }

        [Fact]
        public async void GetTeamMembers_Invalid_TeamId()
        {
            //Given
            int teamId = 0;
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.GetTeamUsers(teamId) as BadRequestObjectResult;

            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid teamId : {teamId}",response.Value);
        }

        [Fact]
        public async void RemoveTeamUsers_Successfull()
        {
            //Given
            int teamId = 1, userId = 1;
            bool isUserRemoved = true;
            teamServiceMoq.Setup(e => e.RemoveUserFromTeam(teamId,userId)).ReturnsAsync(isUserRemoved);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.RemoveTeamUsers(teamId,userId) as OkObjectResult;

            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("User removed",response.Value);
            teamServiceMoq.Verify(e => e.RemoveUserFromTeam(teamId,userId),Times.Once);
        }

        [Fact]
        public async void RemoveTeamUsers_NotFound()
        {
            //Given
            int teamId = 10, userId = 1;
            bool isUserRemoved = false;
            teamServiceMoq.Setup(e => e.RemoveUserFromTeam(teamId,userId)).ReturnsAsync(isUserRemoved);
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.RemoveTeamUsers(teamId,userId) as NotFoundObjectResult;

            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"No User found for the teamId : {teamId} to remove",response.Value);
            teamServiceMoq.Verify(e => e.RemoveUserFromTeam(teamId,userId),Times.Once);
        }

        [Theory]
        [InlineData(0,1)]
        [InlineData(1,0)]
        public async void RemoveTeamusers_Invalid_TeamId(int teamId,int userId)
        {
            //Given
            
            //When
            var controller = new TeamController(logger.Object,teamServiceMoq.Object);
            var response = await controller.RemoveTeamUsers(teamId,userId) as BadRequestObjectResult;

            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid teamId : {teamId} or UserId : {userId}",response.Value);
        }

    }
}
