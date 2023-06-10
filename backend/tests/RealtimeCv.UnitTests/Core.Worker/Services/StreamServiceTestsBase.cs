using System;
using Moq;
using NUnit.Framework;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Worker.Services;

namespace RealtimeCv.UnitTests.Core.Worker.Services;

public class StreamServiceTestsBase
{
    protected StreamService _streamService;
    protected Mock<IStreamReceiver> _mockStreamReceiver;
    protected Mock<IStreamSender> _mockStreamSender;
    protected Mock<IPubSub> _mockPubSub;

    [SetUp]
    public void Setup()
    {
        _mockStreamReceiver = new Mock<IStreamReceiver>();
        _mockStreamSender = new Mock<IStreamSender>();
        _mockPubSub = new Mock<IPubSub>();
        _streamService = new StreamService(_mockStreamReceiver.Object, _mockStreamSender.Object, _mockPubSub.Object);
    }
}
