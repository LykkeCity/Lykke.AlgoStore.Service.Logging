using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using AzureStorage;
using Lykke.AlgoStore.Service.Logging.AzureRepositories;
using Lykke.AlgoStore.Service.Logging.AzureRepositories.Entitites;
using Lykke.AlgoStore.Service.Logging.Core.Repositories;
using Lykke.AlgoStore.Service.Logging.Requests;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.Logging.Tests.Unit
{
    [TestFixture]
    public class UserLogRepositoryTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<INoSQLTableStorage<UserLogEntity>> _storage =
            new Mock<INoSQLTableStorage<UserLogEntity>>();

        private IUserLogRepository _repository;

        private UserLogEntity _entity;
        private UserLogRequest _entityRequest;

        [SetUp]
        public void SetUp()
        {
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AzureRepositories.AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();

            _entityRequest = _fixture.Build<UserLogRequest>().Create();
            _entity = Mapper.Map<UserLogEntity>(_entityRequest);

            _storage.Setup(x => x.InsertAsync(_entity)).Returns(Task.CompletedTask);

            _repository = new UserLogRepository(_storage.Object);
        }

        [Test]
        public void WriteUserLogDataTest()
        {
            _repository.WriteAsync(_entityRequest).Wait();
        }

        [Test]
        public void WriteUserLogWithInstanceIdAndMessageTest()
        {
            _repository.WriteAsync("12345", "Message for 12345").Wait();
        }

        [Test]
        public void WriteUserLogWithInstanceIdAndExceptionTest()
        {
            _repository.WriteAsync("12345", new Exception("Exception for 12345")).Wait();
        }
    }
}
