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
        public void WriteTest()
        {
            _service.WriteAsync(_entitytRequest).Wait();
        }

        private IUserLogService MockValidUserLogService()
        {
            var userLogRepository = MockValidUserLogRepository();

            return  new UserLogService(userLogRepository);
        }

        private IUserLogRepository MockValidUserLogRepository()
        {
            var repo = new Mock<IUserLogRepository>();

            repo.Setup(x => x.WriteAsync(It.IsAny<UserLogRequest>())).Returns(Task.CompletedTask);

            return repo.Object;
        }
    }
}
