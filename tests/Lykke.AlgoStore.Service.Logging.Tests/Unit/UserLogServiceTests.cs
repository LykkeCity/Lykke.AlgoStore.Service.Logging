﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Service.Logging.Core.Repositories;
using Lykke.AlgoStore.Service.Logging.Core.Services;
using Lykke.AlgoStore.Service.Logging.Requests;
using Lykke.AlgoStore.Service.Logging.Services;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.Logging.Tests.Unit
{
    [TestFixture]
    public class UserLogServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private IUserLogService _service;

        private UserLogRequest _entitytRequest;

        [SetUp]
        public void SetUp()
        {
            _service = MockValidUserLogService();
            _entitytRequest = _fixture.Build<UserLogRequest>().Create();
        }

        [Test]
        public void WriteUserLogDataTest()
        {
            _service.WriteAsync(_entitytRequest).Wait();
        }

        [Test]
        public void WriteUserLogDataAsNullWillThrowExceptionTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.WriteAsync(null));
        }

        [Test]
        public void WriteUserLogWithInstanceIdAndMessageTest()
        {
            _service.WriteAsync("12345", "Message for 12345").Wait();
        }

        [Test]
        public void WriteUserLogWithInvalidInstanceIdWillThrowExceptionTest()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(string.Empty, "Message for 12345"));
        }

        [Test]
        public void WriteUserLogWithNullAsInstanceIdWillThrowExceptionTest()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(null, "Message for 12345"));
        }

        [Test]
        public void WriteUserLogWithInvalidMessageWillThrowExceptionTest()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync("12345", string.Empty));
        }

        [Test]
        public void WriteUserLogWithNullAsMessageWillThrowExceptionTest()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync("12345", message: null));
        }

        [Test]
        public void WriteUserLogWithInstanceIdAndExceptionTest()
        {
            _service.WriteAsync("12345", new Exception("Exception for 12345")).Wait();
        }

        [Test]
        public void WriteUserLogWithInvalidExceptionWillThrowExceptionTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.WriteAsync("12345", ex: null));
        }

        private static IUserLogService MockValidUserLogService()
        {
            var userLogRepository = MockValidUserLogRepository();

            return new UserLogService(userLogRepository);
        }

        private static IUserLogRepository MockValidUserLogRepository()
        {
            var repo = new Mock<IUserLogRepository>();

            repo.Setup(x => x.WriteAsync(It.IsAny<UserLogRequest>())).Returns(Task.CompletedTask);
            repo.Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            repo.Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<Exception>())).Returns(Task.CompletedTask);

            return repo.Object;
        }
    }
}
