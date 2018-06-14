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
        private UserLogRequest _entitytRequest;

        [SetUp]
        public void SetUp()
        {
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AzureRepositories.AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();

            _entitytRequest = _fixture.Build<UserLogRequest>().Create();
            _entity = Mapper.Map<UserLogEntity>(_entitytRequest);

            _storage.Setup(x => x.InsertAsync(_entity)).Returns(Task.CompletedTask);

            _repository = new UserLogRepository(_storage.Object);
        }

        [Test]
        public void WriteTest()
        {
            _repository.WriteAsync(_entitytRequest).Wait();
        }
    }
}
