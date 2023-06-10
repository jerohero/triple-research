using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ardalis.Result;
using Moq;
using NUnit.Framework;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Models;
using RealtimeCv.Core.Models.Dto;

namespace RealtimeCv.UnitTests.Core.Functions.Services;

[TestFixture]
public class StreamPollServiceTests : StreamPollServiceTestsBase
{
    [Test]
    public void StartPollStreams_WhenCalled_ShouldSendMessageForEachChunk()
    {
        // Arrange
        var visionSets = new List<VisionSet>
        {
            new() { Id = 1, Sources = Enumerable.Range(1, 15).Select(x => $"source{x}").ToList() },
            new() { Id = 2, Sources = Enumerable.Range(16, 20).Select(x => $"source{x}").ToList() }
        };
        _mockVisionSetRepository.Setup(x => x.ListAsync(CancellationToken.None)).ReturnsAsync(visionSets);

        // Act
        var pollStreams = _streamPollService.StartPollStreams();

        // Assert
        _mockQueue.Verify(x => 
                x.SendMessage("stream-poll-chunk", It.IsAny<StreamPollChunkMessage>()), Times.Exactly(4)
        );
    }

    [Test]
    public void StartSessionsForActiveStreams_WhenCalled_ShouldStartSessionForActiveStreams()
    {
        // Arrange
        var message = new StreamPollChunkMessage
        {
            VisionSetId = 1,
            Sources = Enumerable.Range(1, 10).Select(x => $"source{x}").ToList()
        };
        _mockStreamReceiver.Setup(x => 
            x.CheckConnection(It.IsAny<string>())).Returns(true
        );
        _mockSessionService.Setup(x => 
            x.GetActiveVisionSetSessionsBySource(It.IsAny<int>(),It.IsAny<string>())
        ).ReturnsAsync(Result<List<Session>>.Success(new List<Session>()));

        // Act
        var result = _streamPollService.StartSessionsForActiveStreams(message);

        // Assert
        Assert.AreEqual(10, result.Result.Count);
        _mockSessionService.Verify(x => x.StartSession(It.IsAny<SessionStartDto>()), Times.Exactly(10));
    }
}
