using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Lykke.AlgoStore.Service.Logging.Controllers;
using Lykke.AlgoStore.Service.Logging.Core.Domain;
using Lykke.AlgoStore.Service.Logging.Core.Services;
using Lykke.AlgoStore.Service.Logging.Requests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.Logging.Tests.Unit
{
    [TestFixture]
    public class UserLogControllerTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IUserLogService> _serviceMock;
        private UserLogController _controller;
        private UserLogRequest _userLogRequest;
        private List<UserLogRequest> _userLogsRequest;

        [SetUp]
        public void SetUp()
        {
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AzureRepositories.AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();

            _serviceMock = new Mock<IUserLogService>();

            _serviceMock.Setup(x => x.WriteAsync(It.IsAny<IUserLog>())).Returns(Task.CompletedTask);
            _serviceMock.Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            _userLogRequest = _fixture.Build<UserLogRequest>().Create();

            _controller = new UserLogController(_serviceMock.Object);
        }

        [Test]
        public void WriteLogWillReturnCorrectResult()
        {
            var result = _controller.WriteLog(_userLogRequest).Result;

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void WriteMessageWillReturnCorrectResult()
        {
            var instanceId = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();

            var result = _controller.WriteMessage(instanceId, message).Result;

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void WriteLogsWillReturnCorrectResult()
        {
            _userLogsRequest = _fixture.Build<UserLogRequest>().With(x => x.InstanceId, "TEST").CreateMany().ToList();

            var result = _controller.WriteLogs(_userLogsRequest).Result;

            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
