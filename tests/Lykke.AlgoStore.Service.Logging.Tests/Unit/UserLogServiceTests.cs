using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Lykke.AlgoStore.Service.Logging.AzureRepositories.DTOs;
using Lykke.AlgoStore.Service.Logging.Core;
using Lykke.AlgoStore.Service.Logging.Core.Domain;
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

        private UserLogRequest _entityRequest;
        private List<UserLogRequest> _entitiesRequest;
        private List<IUserLog> _data;

        [SetUp]
        public void SetUp()
        {
            _data = new List<IUserLog>();
            var dtoData = _fixture.Build<UserLogDto>().With(x => x.InstanceId, "TEST").CreateMany();
            _data.AddRange(dtoData);

            _entityRequest = _fixture.Build<UserLogRequest>().Create();

            _service = MockValidUserLogService();
        }

        [Test]
        public void WriteUserLogDataTest()
        {
            _service.WriteAsync(_entityRequest).Wait();
        }

        [Test]
        public void WriteUserLogDataAsNullWillThrowExceptionTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.WriteAsync(userLog: null));
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

        [Test]
        public void WriteUserLogsAsNullWillThrowExceptionTest()
        {
            _entitiesRequest = null;
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.WriteAsync(_entitiesRequest));
        }

        [Test]
        public void WriteUserLogsWithDifferentInstanceIdsWillThrowExceptionTest()
        {
            _entitiesRequest = _fixture.Build<UserLogRequest>().CreateMany().ToList();

            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_entitiesRequest));
        }

        [Test]
        public void WriteUserLogsWithAnyMessageEmptyWillThrowExceptionTest()
        {
            _entitiesRequest = _fixture.Build<UserLogRequest>().With(x => x.InstanceId, "TEST").CreateMany().ToList();
            _entitiesRequest[0].Message = string.Empty;

            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_entitiesRequest));
        }

        [Test]
        public void WriteMoreThenHundredUserLogsWillThrowExceptionTest()
        {
            _entitiesRequest = _fixture.Build<UserLogRequest>().With(x => x.InstanceId, "TEST").CreateMany(101).ToList();

            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_entitiesRequest));
        }

        [Test]
        public void WriteUserLogsTest()
        {
            _entitiesRequest = _fixture.Build<UserLogRequest>().With(x => x.InstanceId, "TEST").CreateMany().ToList();
            _service.WriteAsync(_entitiesRequest).Wait();
        }

        [Test]
        public void GetTailLogTest()
        {
            var result = _service.GetTailLog(10, "TEST").Result;

            for (int i = 0; i < _data.Count; i++)
            {
                Assert.AreEqual($"[{_data[i].Date.ToString(Constants.CustomDateTimeFormat)}] {_data[i].Message}",
                    result[i]);
            }
        }

        [Test]
        public void GetTailLogWithInvalidLimitWillThrowExceptionTest()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.GetTailLog(0, "TEST"));
        }

        [Test]
        public void GetTailLogWithInvalidInstanceIdWillThrowExceptionTest()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.GetTailLog(10, ""));
        }

        private IUserLogService MockValidUserLogService()
        {
            var userLogRepository = MockValidUserLogRepository();

            return new UserLogService(userLogRepository);
        }

        private IUserLogRepository MockValidUserLogRepository()
        {
            var repo = new Mock<IUserLogRepository>();

            repo.Setup(x => x.WriteAsync(It.IsAny<UserLogRequest>())).Returns(Task.CompletedTask);
            repo.Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            repo.Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<Exception>())).Returns(Task.CompletedTask);
            repo.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.FromResult(_data.AsEnumerable()));

            return repo.Object;
        }
    }
}
