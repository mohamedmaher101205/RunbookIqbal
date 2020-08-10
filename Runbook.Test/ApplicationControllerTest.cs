using System.Net.Mime;
using System;
using Xunit;
using Moq;
using Runbook.Services.Interfaces;
using Runbook.Models;
using System.Collections.Generic;
using Runbook.API.Controllers;
using Microsoft.Extensions.Logging;

namespace Runbook.Test
{
    public class ApplicationControllerTest
    {
        [Fact]
        public void GetAllApplication_Returns_Applications()
        {
            //Arrange
            var moq = new Mock<IApplicationService>();
            var logger = new Mock<ILogger<ApplicationController>>();
            int tenantId = 1;
            moq.Setup(c => c.GetAllApplications(tenantId)).Returns(It.IsAny<IEnumerable<Application>>());

            //Act
            var controller = new ApplicationController(logger.Object ,moq.Object);
            var result = controller.GetAllApplications(tenantId);

            //Assert
            //Assert.IsType<IEnumerable<Application>>(result.Value);
        }
    }
}
