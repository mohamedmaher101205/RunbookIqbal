using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System;
using Xunit;
using Moq;
using Runbook.Services.Interfaces;
using Runbook.Models;
using Runbook.API.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Runbook.Test
{
    public class GroupControllerTest
    {
        private readonly Mock<IGroupService> groupServiceMoq;
        private readonly Mock<ILogger<GroupController>> logger;

        public GroupControllerTest()
        {
            groupServiceMoq = new Mock<IGroupService>();
            logger = new Mock<ILogger<GroupController>>();
        }

        [Fact]
        public void CreateGroup_Success()
        {
            //Given
            int tenantId = 1, groupCreated = 1;
            Group group = new Group()
            {
                GroupName = "Group 1",
                Description = "Group"
            };
            groupServiceMoq.Setup(g => g.CreateGroup(tenantId,group)).Returns(groupCreated);
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.CreateGroup(group,tenantId) as OkObjectResult;
            
            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Group created successfully",response.Value);
            groupServiceMoq.Verify(g => g.CreateGroup(tenantId,group),Times.Once);
        }

        [Fact]
        public void CreateGroup_UnSuccessfull()
        {
            //Given
            int tenantId = 1, groupCreated = 0;
            Group group = new Group()
            {
                GroupName = "Group 1",
                Description = "Group"
            };
            groupServiceMoq.Setup(g => g.CreateGroup(tenantId,group)).Returns(groupCreated);
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.CreateGroup(group,tenantId) as NotFoundObjectResult;
            
            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal("Group Creation unsuccessfull",response.Value);
            groupServiceMoq.Verify(g => g.CreateGroup(tenantId,group),Times.Once);
        }

        [Theory]
        [InlineData("",1)]
        [InlineData("Group 2",0)]
        public void CreateGroup_Invalid_TenantId_or_GroupName(string groupName, int tenantId)
        {
            //Given
            Group group = new Group()
            {
                GroupName = groupName,
                Description = "Group"
            };
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.CreateGroup(group,tenantId) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid tenantId : {tenantId} or GroupName : {group.GroupName}",response.Value);
        }

        [Fact]
        public void GetPermissions_Successfull()
        {
            //Given
            Permissions permission = new Permissions() { Permission = "Read" , PermissionId = 1};
            var permissions = new List<Permissions>() {permission};
            groupServiceMoq.Setup(g => g.GetPermissions()).Returns(permissions);
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.GetPermissions() as OkObjectResult;
            var responseObj = response.Value as List<Permissions>;
            
            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(response.Value);
            Assert.True(responseObj.Count > 0);
            groupServiceMoq.Verify(g => g.GetPermissions(),Times.Once);
        }

        [Fact]
        public void GetPermissions_NoPermissions_Found()
        {
            //Given
            IEnumerable<Permissions> permissions = null;
            groupServiceMoq.Setup(g => g.GetPermissions()).Returns(permissions);
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.GetPermissions() as NotFoundObjectResult;
            
            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"No Permissions found",response.Value);
            groupServiceMoq.Verify(g => g.GetPermissions(),Times.Once);
        }

        [Fact]
        public void GetTenantGroups_Successfull()
        {
            //Given
            int tenantId = 1;
            Group group = new Group() {GroupName = "Group 1", Description = "G1"};
            var groups = new List<Group>() {group};
            groupServiceMoq.Setup(g => g.GetTenantGroups(tenantId)).Returns(groups);
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.GetTenantGroups(tenantId) as OkObjectResult;
            var responseObj = response.Value as List<Group>;
            
            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(response.Value);
            Assert.True(responseObj.Count > 0);
            groupServiceMoq.Verify(g => g.GetTenantGroups(tenantId),Times.Once);
        }

        [Fact]
        public void GetTenantGroups_Groups_Not_Avialable()
        {
            //Given
            int tenantId = 1000;
            IEnumerable<Group> groups = null;
            groupServiceMoq.Setup(g => g.GetTenantGroups(tenantId)).Returns(groups);
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.GetTenantGroups(tenantId) as NotFoundObjectResult;
            
            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"No Groups found for TenantId : {tenantId}",response.Value);
            groupServiceMoq.Verify(g => g.GetTenantGroups(tenantId),Times.Once);
        }

        [Fact]
        public void GetTenantGroups_Invalid_TenantId()
        {
            //Given
            int tenantId = 0;
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.GetTenantGroups(tenantId) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid TenantId : {tenantId}",response.Value);
        }

        [Fact]
        public void AddUsersToGroup_Successfull()
        {
            //Given
            int groupId = 1,usersLinkedToGroup = 4;
            string userIds = "1,2,3,4";
            int[] usersIds = Array.ConvertAll(userIds.Split(","),int.Parse);
            groupServiceMoq.Setup(g => g.AddUsersToGroup(groupId,usersIds)).Returns(usersLinkedToGroup);
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.AddUsersToGroup(groupId,userIds) as OkObjectResult;
            
            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal($"{usersLinkedToGroup} rows inserted",response.Value);
            groupServiceMoq.Verify(g => g.AddUsersToGroup(groupId,usersIds),Times.Once);
        }

        [Fact]
        public void AddUsersToGroup_Failure()
        {
            //Given
            int groupId = 1,usersLinkedToGroup = 0;
            string userIds = "1,2,3,4";
            int[] usersIds = Array.ConvertAll(userIds.Split(","),int.Parse);
            groupServiceMoq.Setup(g => g.AddUsersToGroup(groupId,usersIds)).Returns(usersLinkedToGroup);
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.AddUsersToGroup(groupId,userIds) as NotFoundObjectResult;
            
            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"rows failed to insert",response.Value);
            groupServiceMoq.Verify(g => g.AddUsersToGroup(groupId,usersIds),Times.Once);
        }

        [Theory]
        [InlineData(1,null)]
        [InlineData(0,"1,2,3")]
        public void AddUsersToGroup_Invalid_GroupId_Or_UserIds(int GroupId,string UserIds)
        {
            //Given
            int groupId = GroupId;
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.AddUsersToGroup(groupId,UserIds) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid GroupId : {groupId} or Users ids : {UserIds}",response.Value);
        }

        [Fact]
        public void GetGroupUsers_Successfull()
        {
            //Given
            int groupId = 1;
            User user = new User() {FirstName = "V",LastName = "K"};
            var users = new List<User>() {user};
            groupServiceMoq.Setup(g => g.GetGroupUsers(groupId)).Returns(users);
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.GetGroupUsers(groupId) as OkObjectResult;
            var responseObj = response.Value as List<User>;
            
            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(response.Value);
            Assert.True(responseObj.Count > 0);
            groupServiceMoq.Verify(g => g.GetGroupUsers(groupId),Times.Once);
        }

        [Fact]
        public void GetGroupUsers_No_Users_Found()
        {
            //Given
            int groupId = 5;
            IEnumerable<User> users = null;
            groupServiceMoq.Setup(g => g.GetGroupUsers(groupId)).Returns(users);
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.GetGroupUsers(groupId) as NotFoundObjectResult;
            
            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"No users found for this Group : {groupId}",response.Value);
            groupServiceMoq.Verify(g => g.GetGroupUsers(groupId),Times.Once);
        }

        [Fact]
        public void GetGroupUsers_Invalid_GroupId()
        {
            //Given
            int groupId = 0;
            
            //When
            var controller = new GroupController(logger.Object,groupServiceMoq.Object);
            var response = controller.GetGroupUsers(groupId) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid GroupId : {groupId}",response.Value);
        }
    }
}
