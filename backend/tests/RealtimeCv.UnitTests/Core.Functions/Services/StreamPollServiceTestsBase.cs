using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Moq;
using NUnit.Framework;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Functions.Services;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Models;
using RealtimeCv.Core.Models.Dto;

namespace RealtimeCv.UnitTests.Core.Functions.Services;

public class StreamPollServiceTestsBase
{
    protected Mock<ILoggerAdapter<StreamPollService>> _mockLogger;
    protected Mock<IStreamReceiver> _mockStreamReceiver;
    protected Mock<IVisionSetRepository> _mockVisionSetRepository;
    protected Mock<ISessionService> _mockSessionService;
    protected Mock<IQueue> _mockQueue;
    protected StreamPollService _streamPollService;

    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILoggerAdapter<StreamPollService>>();
        _mockStreamReceiver = new Mock<IStreamReceiver>();
        _mockVisionSetRepository = new Mock<IVisionSetRepository>();
        _mockSessionService = new Mock<ISessionService>();
        _mockQueue = new Mock<IQueue>();

        _streamPollService = new StreamPollService(
            _mockLogger.Object,
            _mockStreamReceiver.Object,
            _mockVisionSetRepository.Object,
            _mockSessionService.Object,
            _mockQueue.Object);
    }
}
