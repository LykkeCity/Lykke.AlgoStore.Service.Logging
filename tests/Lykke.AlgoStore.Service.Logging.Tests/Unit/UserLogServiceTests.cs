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
using Lykke.AlgoStore.Service.Logging.Services.Strings;
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
        public void Write_UserLog_Test()
        {
            _service.WriteAsync(_entityRequest).Wait();
        }

        [Test]
        public void Write_UserLog_AsNull_WillThrowException_Test()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _service.WriteAsync(userLog: null));

            Assert.That(ex.Message, Is.EqualTo($"Value cannot be null.{Environment.NewLine}Parameter name: userLog"));
        }

        [Test]
        public void Write_UserLog_WithInstanceIdAndMessage_Test()
        {
            _service.WriteAsync("12345", "Message for 12345").Wait();
        }

        [Test]
        public void Write_UserLog_WithInvalidInstanceId_WillThrowException_Test()
        {
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(string.Empty, "Message for 12345"));

            Assert.That(ex.Message, Is.EqualTo(Phrases.InstanceIdCannotBeEmpty));
        }

        [Test]
        public void Write_UserLog_WithNullAsInstanceId_WillThrowException_Test()
        {
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(null, "Message for 12345"));

            Assert.That(ex.Message, Is.EqualTo(Phrases.InstanceIdCannotBeEmpty));
        }

        [Test]
        public void Write_UserLog_WithInvalidMessage_WillThrowExceptionTest()
        {
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync("12345", string.Empty));

            Assert.That(ex.Message, Is.EqualTo(Phrases.MessageCannotBeEmpty));
        }

        [Test]
        public void Write_UserLog_WithNullAsMessage_WillThrowException_Test()
        {
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync("12345", message: null));

            Assert.That(ex.Message, Is.EqualTo(Phrases.MessageCannotBeEmpty));
        }

        [Test]
        public void Write_UserLog_WithInstanceIdAndException_Test()
        {
            _service.WriteAsync("12345", new Exception("Exception for 12345")).Wait();
        }

        [Test]
        public void Write_UserLog_WithInvalidException_WillThrowExceptionTest()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _service.WriteAsync("12345", ex: null));

            Assert.That(ex.Message, Is.EqualTo($"Value cannot be null.{Environment.NewLine}Parameter name: exception"));
        }

        [Test]
        public void Write_UserLogs_AsNull_WillThrowException_Test()
        {
            _entitiesRequest = null;

            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _service.WriteAsync(_entitiesRequest));

            Assert.That(ex.Message, Is.EqualTo($"Value cannot be null.{Environment.NewLine}Parameter name: source"));
        }

        [Test]
        public void Write_UserLogs_WithDifferentInstanceIds_WillThrowException_Test()
        {
            _entitiesRequest = _fixture.Build<UserLogRequest>().CreateMany().ToList();

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_entitiesRequest));

            Assert.That(ex.Message, Is.EqualTo(Phrases.InstanceIdMustBeSameForAllLogs));
        }

        [Test]
        public void Write_UserLogs_WithAnyMessageEmpty_WillThrowException_Test()
        {
            _entitiesRequest = _fixture.Build<UserLogRequest>().With(x => x.InstanceId, "TEST").CreateMany().ToList();
            _entitiesRequest[0].Message = string.Empty;

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_entitiesRequest));

            Assert.That(ex.Message, Is.EqualTo(Phrases.AnyMessageCanNotBeEmpty));
        }

        [Test]
        public void Write_MoreThenHundredUserLogs_WillThrowException_Test()
        {
            _entitiesRequest = _fixture.Build<UserLogRequest>().With(x => x.InstanceId, "TEST").CreateMany(101).ToList();

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_entitiesRequest));

            Assert.That(ex.Message, Is.EqualTo(Phrases.MaxNumberOfLogsPerBatchReached));
        }

        [Test]
        public void Write_UserLogs_Test()
        {
            _entitiesRequest = _fixture.Build<UserLogRequest>().With(x => x.InstanceId, "TEST").CreateMany().ToList();
            _service.WriteAsync(_entitiesRequest).Wait();
        }

        [Test]
        public void GetTailLog_Test()
        {
            var result = _service.GetTailLog(10, "TEST").Result;

            Assert.IsNotEmpty(result);
        }

        [Test]
        public void GetTailLog_WithInvalidLimit_WillThrowException_Test()
        {
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.GetTailLog(0, "TEST"));

            Assert.That(ex.Message, Is.EqualTo(Phrases.LogNumberOfReturnedRecordsLimitReached));
        }

        [Test]
        public void GetTailLog_WithInvalidInstanceId_WillThrowExceptionTest()
        {
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.GetTailLog(10, ""));

            Assert.That(ex.Message, Is.EqualTo(Phrases.InstanceIdCannotBeEmpty));
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
