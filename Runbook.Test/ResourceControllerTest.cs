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
    public class ResourceControllerTest
    {
        private readonly Mock<IResourceService> resourceServiceMoq;
        private readonly Mock<ILogger<ResourceController>> logger;

        public ResourceControllerTest()
        {
            resourceServiceMoq = new Mock<IResourceService>();
            logger = new Mock<ILogger<ResourceController>>();
        }

        [Fact]
        public void CreateResourceType_Successfull()
        {
            //Given
            ResourceType resourceType = new ResourceType()
            {
                ResourceTypeName = "VM"
            };
            int tenantId = 1, resourceTypeCreated = 1;
            resourceServiceMoq.Setup(r => r.CreateResourceType(resourceType,tenantId)).Returns(resourceTypeCreated);
            
            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.CreateResourceType(resourceType,tenantId) as OkObjectResult;
            
            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Resource Type created successfully",response.Value);
            resourceServiceMoq.Verify(r => r.CreateResourceType(resourceType,tenantId),Times.Once);
        }

        [Fact]
        public void CreateResourceType_Failure()
        {
            //Given
            ResourceType resourceType = new ResourceType()
            {
                ResourceTypeName = "VM"
            };
            int tenantId = 1, resourceTypeCreated = 0;
            resourceServiceMoq.Setup(r => r.CreateResourceType(resourceType,tenantId)).Returns(resourceTypeCreated);
            
            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.CreateResourceType(resourceType,tenantId) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Resource Type Not created",response.Value);
            resourceServiceMoq.Verify(r => r.CreateResourceType(resourceType,tenantId),Times.Once);
        }

        [Theory]
        [InlineData("Virtual machine",0)]
        [InlineData(null,1)]
        public void CreateResourceType_Invalid_ResourceName_Or_TenantId(string resourceName,int tenantId)
        {
            //Given
            ResourceType resourceType = new ResourceType()
            {
                ResourceTypeName = resourceName
            };
            
            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.CreateResourceType(resourceType,tenantId) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Empty ResourceType name : {resourceType.ResourceTypeName} Or invalid tenantId : {tenantId}",response.Value);
        }

        [Fact]
        public void GetAllResourceTypes_Success()
        {
            //Given
            ResourceType resourceType = new ResourceType() { ResourceTypeName = "VM" };
            var resourceTypes = new List<ResourceType>() {resourceType};
            int tenantId = 1;
            resourceServiceMoq.Setup(r => r.GetResourceTypes(tenantId)).Returns(resourceTypes);
            
            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.GetAllResourceTypes(tenantId) as OkObjectResult;
            
            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(response.Value);
            resourceServiceMoq.Verify(r => r.GetResourceTypes(tenantId),Times.Once);
        }

        [Fact]
        public void GetAllResourceTypes_NotFound()
        {
            //Given
            IEnumerable<ResourceType> resourceTypes = null;
            int tenantId = 40;
            resourceServiceMoq.Setup(r => r.GetResourceTypes(tenantId)).Returns(resourceTypes);
            
            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.GetAllResourceTypes(tenantId) as NotFoundObjectResult;
            
            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"There are no resource types found for tenantId : {tenantId}",response.Value);
            resourceServiceMoq.Verify(r => r.GetResourceTypes(tenantId),Times.Once);
        }

        [Fact]
        public void GetAllResourceTypes_Invalid_TenantId()
        {
            //Given
            int tenantId = 0;
            
            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.GetAllResourceTypes(tenantId) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid TenantId : {tenantId}",response.Value);
        }

        [Fact]
        public void CreateResource_Successfull()
        {
            //Given
            Resource resource = new Resource()
            {
                ResourceName = "Database server"
            };
            int tenantId = 1, resourceCreated = 1;
            resourceServiceMoq.Setup(r => r.CreateResource(resource,tenantId)).Returns(resourceCreated);
            
            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.CreateResource(resource,tenantId) as OkObjectResult;
            
            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Resource created successfully",response.Value);
            resourceServiceMoq.Verify(r => r.CreateResource(resource,tenantId),Times.Once);
        }

        [Fact]
        public void CreateResource_Failure()
        {
            //Given
            Resource resource = new Resource()
            {
                ResourceName = "Database server"
            };
            int tenantId = 1, resourceCreated = 0;
            resourceServiceMoq.Setup(r => r.CreateResource(resource,tenantId)).Returns(resourceCreated);
            
            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.CreateResource(resource,tenantId) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Resource Not created",response.Value);
            resourceServiceMoq.Verify(r => r.CreateResource(resource,tenantId),Times.Once);
        }

        [Theory]
        [InlineData("Virtual machine",0)]
        [InlineData(null,1)]
        public void CreateResource_Invalid_ResourceName_Or_TenantId(string resourceName, int tenantId)
        {
            //Given
            Resource resource = new Resource()
            {
                ResourceName = resourceName
            };

            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.CreateResource(resource,tenantId) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Empty Resource name : {resource.ResourceName} or Invalid TenantId : {tenantId}",response.Value);
        }

        [Fact]
        public void GetAllResources_Success()
        {
            //Given
            Resource resource = new Resource() { ResourceName = "VM" };
            var resources = new List<Resource>() {resource};
            int tenantId = 1;
            resourceServiceMoq.Setup(r => r.GetAllResources(tenantId)).Returns(resources);
            
            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.GetAllResources(tenantId) as OkObjectResult;
            
            //Then
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(response.Value);
            resourceServiceMoq.Verify(r => r.GetAllResources(tenantId),Times.Once);
        }

        [Fact]
        public void GetAllResources_NotFound()
        {
            //Given
            IEnumerable<Resource> resources = null;
            int tenantId = 500;
            resourceServiceMoq.Setup(r => r.GetAllResources(tenantId)).Returns(resources);
            
            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.GetAllResources(tenantId) as NotFoundObjectResult;
            
            //Then
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal($"No Resources found for the TenantId : {tenantId}",response.Value);
            resourceServiceMoq.Verify(r => r.GetAllResources(tenantId),Times.Once);
        }

        [Fact]
        public void GetAllResources_Invalid_TenantId()
        {
            //Given
            int tenantId = 0;
            
            //When
            var controller = new ResourceController(logger.Object,resourceServiceMoq.Object);
            var response = controller.GetAllResources(tenantId) as BadRequestObjectResult;
            
            //Then
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal($"Invalid TenantId : {tenantId}",response.Value);
        }
    }
}
