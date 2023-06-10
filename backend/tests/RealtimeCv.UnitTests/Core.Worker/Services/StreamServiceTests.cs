using System;
using Moq;
using NUnit.Framework;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.UnitTests.Core.Worker.Services;

public class StreamServiceTests : StreamServiceTestsBase
{
    [Test]
    public void HandleStream_WhenCalled_ShouldCallPrepareTargetAndConnectStreamBySource()
    {
        // Arrange
        var session = new Session { Source = "source" };
        var modelUri = "https://test.com/blob";
        var targetUrl = "http://localhost:5000";

        // Act
        _streamService.HandleStream(session, modelUri, targetUrl);

        // Assert
        _mockStreamSender.Verify(x => x.PrepareTarget(
                It.IsAny<string>(), modelUri, 180), Times.Once
        );
        _mockStreamReceiver.Verify(x => x.ConnectStreamBySource(session.Source, 15), Times.Once);
    }

    [Test]
    public void HandleStream_WhenSessionSourceIsNull_ShouldThrowException()
    {
        // Arrange
        var session = new Session { Source = null };
        var modelUri = "https://test.com/blob";
        var targetUrl = "http://localhost:5000";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _streamService.HandleStream(session, modelUri, targetUrl));
    }

    [Test]
    public void HandleStream_WhenTargetUrlIsEmpty_ShouldThrowException()
    {
        // Arrange
        var session = new Session { Source = "source" };
        var modelUri = "https://test.com/blob";
        var targetUrl = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _streamService.HandleStream(session, modelUri, targetUrl));
    }

    [Test]
    public void HandleStream_WhenOnConnectionEstablished_ShouldCallSendStreamToEndpoint()
    {
        // Arrange
        var session = new Session { Source = "source" };
        var modelUri = "https://test.com/blob";
        var targetUrl = "http://localhost:5000";

        // Act
        _streamService.HandleStream(session, modelUri, targetUrl);
        _mockStreamReceiver.Raise(m => m.OnConnectionEstablished += null);

        // Assert
        _mockStreamSender.Verify(x => x.SendStreamToEndpoint(
                _mockStreamReceiver.Object, It.IsAny<string>()), Times.Once
        );
    }

    [Test]
    public void HandleStream_WhenOnConnectionTimeout_ShouldInvokeOnStreamEndedAndDisposeStreamReceiverAndSender()
    {
        // Arrange
        var session = new Session { Source = "source" };
        var modelUri = "https://test.com/blob";
        var targetUrl = "http://localhost:5000";
        var eventInvoked = false;
        _streamService.OnStreamEnded += () => eventInvoked = true;

        // Act
        _streamService.HandleStream(session, modelUri, targetUrl);
        _mockStreamReceiver.Raise(m => m.OnConnectionTimeout += null);

        // Assert
        Assert.IsTrue(eventInvoked);
        _mockStreamReceiver.Verify(x => x.Dispose(), Times.Once);
        _mockStreamSender.Verify(x => x.Dispose(), Times.Once);
    }

    [Test]
    public void HandleStream_WhenOnPredictionResult_ShouldCallSendOnPubSub()
    {
        // Arrange
        var session = new Session { Source = "source" };
        var modelUri = "https://test.com/blob";
        var targetUrl = "http://localhost:5000";
        var predictionResult = new object();

        // Act
        _streamService.HandleStream(session, modelUri, targetUrl);
        _mockStreamSender.Raise(m => m.OnPredictionResult += null, predictionResult);

        // Assert
        _mockPubSub.Verify(x => x.Send(predictionResult, session.Pod, "predictions"), Times.Once);
    }
}
