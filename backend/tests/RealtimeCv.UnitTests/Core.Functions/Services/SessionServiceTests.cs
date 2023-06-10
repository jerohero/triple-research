using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Result;
using Moq;
using NUnit.Framework;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Models.Dto;

namespace RealtimeCv.UnitTests.Core.Functions.Services;

[TestFixture]
public class SessionServiceTests : SessionServiceTestsBase
{
    [Test]
    public void GetSessionById_WhenSessionFound_ItShouldBeReturned()
    {
        // Arrange
        SetupSessions(2);
        const int expected = 1;

        // Act
        var result = _service.GetSessionById(1);
        
        // Assert
        Assert.That(result.Result.Value.Id == expected);
    }
    
    [Test]
    public void GetSessionById_WhenSessionFound_ItShouldReturnResultStatusOk()
    {
        // Arrange
        SetupSessions(2);
        const ResultStatus expected = ResultStatus.Ok;

        // Act
        var result = _service.GetSessionById(1);

        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void GetSessionById_WhenSessionFound_ItShouldReturnTypeOfSessionDto()
    {
        // Arrange
        SetupSessions(2);
        var expected = typeof(SessionDto);

        // Act
        var result = _service.GetSessionById(1);

        // Assert
        Assert.That(result.Result.ValueType == expected);
    }

    [Test]
    public void GetSessionById_WhenSessionNotFound_ItShouldReturnResultStatusNotFound()
    {
        // Arrange
        SetupSessions(2);
        const ResultStatus expected = ResultStatus.NotFound;

        // Act
        var result = _service.GetSessionById(3);

        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void GetSessionsByVisionSet_WhenSessionsFound_ItShouldReturnSessions()
    {
        // Arrange
        SetupSessions(2);
        const int visionSetId = 1;
        const int expected = 2;

        // Act
        var result = _service.GetSessionsByVisionSet(visionSetId);

        // Assert
        Assert.That(result.Result.Value.Count == expected);
    }
    
    [Test]
    public void GetSessionsByVisionSet_WhenSessionsFound_ItShouldReturnTypeOfSessionDtoList()
    {
        // Arrange
        SetupSessions(2);
        const int visionSetId = 1;
        var expected = typeof(List<SessionDto>);

        // Act
        var result = _service.GetSessionsByVisionSet(visionSetId);

        // Assert
        Assert.That(result.Result.ValueType == expected);
    }
    
    [Test]
    public void GetSessionsByVisionSet_WhenSessionsNotFound_ItShouldReturnNoSessions()
    {
        // Arrange
        const int visionSetId = 1;
        const int expected = 0;

        // Act
        var result = _service.GetSessionsByVisionSet(visionSetId);

        // Assert
        Assert.That(result.Result.Value.Count == expected);
    }
    
    [Test]
    public void GetSessionsByVisionSet_WhenSessionsNotFound_ItShouldReturnResultStatusOk()
    {
        // Arrange
        const int visionSetId = 1;
        const ResultStatus expected = ResultStatus.Ok;

        // Act
        var result = _service.GetSessionsByVisionSet(visionSetId);

        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void GetActiveVisionSetSessionsBySource_WhenSessionsFound_ItShouldReturnActiveSessions()
    {
        // Arrange
        SetupSessions(2);
        const int expected = 2;
        const int visionSetId = 1;
        const string source = "rtsp://test.com";
        foreach (var session in _context.Session)
        {
            session.IsActive = true;
            _context.Session.Update(session);
        }
        _context.SaveChanges();

        // Act
        var result = _service.GetActiveVisionSetSessionsBySource(visionSetId, source);

        // Assert
        Assert.That(result.Result.Value.Count == expected);
    }

    [Test]
    public void GetActiveVisionSetSessionsBySource_WhenSessionsFound_ItShouldReturnTypeOfSessionList()
    {
        // Arrange
        SetupSessions(2);
        var expected = typeof(List<Session>);
        const int visionSetId = 1;
        const string source = "rtsp://test.com";

        // Act
        var result = _service.GetActiveVisionSetSessionsBySource(visionSetId, source);

        // Assert
        Assert.That(result.Result.ValueType == expected);
    }

    [Test]
    public void GetActiveVisionSetSessionsBySource_WhenNoActiveSessions_ItShouldReturnNoSessions()
    {
        // Arrange
        SetupSessions(2);
        const int expected = 0;
        const int visionSetId = 1;
        const string source = "rtsp://test.com";
        
        // Act
        var result = _service.GetActiveVisionSetSessionsBySource(visionSetId, source);

        // Assert
        Assert.That(result.Result.Value.Count == expected);
    }

    [Test]
    public void GetActiveVisionSetSessionsBySource_WhenNoActiveSessionsForVisionSet_ItShouldReturnNoSessions()
    {
        // Arrange
        SetupSessions(2);
        const int expected = 0;
        const int visionSetId = 2;
        const string source = "rtsp://test.com";

        // Act
        var result = _service.GetActiveVisionSetSessionsBySource(visionSetId, source);

        // Assert
        Assert.That(result.Result.Value.Count == expected);
    }

    [Test]
    public void GetActiveVisionSetSessionsBySource_WhenNoSessionsForSource_ItShouldReturnNoSessions()
    {
        // Arrange
        SetupSessions(2);
        const int expected = 0;
        const int visionSetId = 1;
        const string source = "rtsp://test2.com";

        // Act
        var result = _service.GetActiveVisionSetSessionsBySource(visionSetId, source);

        // Assert
        Assert.That(result.Result.Value.Count == expected);
    }

    [Test]
    public void StartSession_WhenInputValid_ItShouldReturnResultStatusOk()
    {
        // Arrange
        SetupSessions(1);
        const ResultStatus expected = ResultStatus.Ok;
        const int visionSetId = 1;
        const string source = "https://stream.com";
        var createDto = new SessionStartDto(visionSetId, source);
    
        // Act
        var result = _service.StartSession(createDto);
    
        // Assert
        Assert.That(result.Result.Status == expected);
    }

    [Test]
    public void StartSession_WhenInputValid_ItShouldIncreaseCount()
    {
        // Arrange
        SetupSessions(1);
        var expected = 2;
        const int visionSetId = 1;
        const string source = "https://stream.com";
        var createDto = new SessionStartDto(visionSetId, source);
    
        // Act
        var result = _service.StartSession(createDto);

        // Assert
        Assert.That(_context.Session.Count() == expected);
    }

    [Test]
    public void CreateSession_WhenSessionCreated_ItShouldReturnTypeOfSessionDto()
    {
        // Arrange
        SetupSessions(2);
        var expected = typeof(SessionDto);
        const int visionSetId = 1;
        const string source = "https://stream.com";
        var createDto = new SessionStartDto(visionSetId, source);
    
        // Act
        var result = _service.StartSession(createDto);

        // Assert
        Assert.That(result.Result.ValueType == expected);
    }

    [Test]
    public void UpdateSession_WhenInputValid_ItShouldReturnResultStatusOk()
    {
        // Arrange
        SetupSessions(2);
        const int visionSetId = 1;
        const string source = "https://stream.com";
        var dto = new SessionDto(1, visionSetId, source, "cv-test-1", true, DateTime.UtcNow);
        const ResultStatus expected = ResultStatus.Ok;
        
        // Act
        var result = _service.UpdateSession(dto);
        
        // Assert
        Assert.That(result.Result.Status == expected);
    }

    [Test]
    public void UpdateSession_WhenInputValid_ItShouldUpdate()
    {
        // Arrange
        SetupSessions(2);
        const int visionSetId = 1;
        const string source = "https://stream.com";
        var expected = new SessionDto(1, visionSetId, source, "cv-test-1", true, DateTime.UtcNow);

        // Act
        var result = _service.UpdateSession(expected);

        // Assert
        Assert.That(result.Result.Value.Id == expected.Id);
    }

    [Test]
    public void UpdateSession_WhenSessionUpdated_ItShouldReturnTypeOfSessionDto()
    {
        // Arrange
        SetupSessions(2);
        var expected = typeof(SessionDto);
        const int visionSetId = 1;
        const string source = "https://stream.com";
        var dto = new SessionDto(1, visionSetId, source, "cv-test-1", true, DateTime.UtcNow);
        
        // Act
        var result = _service.UpdateSession(dto);
        
        // Assert
        Assert.That(result.Result.ValueType == expected);
    }

    [Test]
    public void DeleteSession_WhenSessionDeleted_ItShouldDecreaseCount()
    {
        // Arrange
        SetupSessions(2);
        const int expected = 1;

        // Act
        var result = _service.DeleteSession(1);

        // Assert
        Assert.That(_context.Session.Count() == expected);
    }

    [Test]
    public void DeleteSession_WhenSessionDeleted_ItShouldReturnResultStatusOk()
    {
        // Arrange
        SetupSessions(2);
        const ResultStatus expected = ResultStatus.Ok;

        // Act
        var result = _service.DeleteSession(1);

        // Assert
        Assert.That(result.Result.Status.Equals(expected));
    }

    [Test]
    public void DeleteSession_WhenSessionNotFound_ItShouldReturnResultStatusNotFound()
    {
        // Arrange
        const ResultStatus expected = ResultStatus.NotFound;

        // Act
        var result = _service.DeleteSession(1);

        // Assert
        Assert.That(result.Result.Status.Equals(expected));
    }
    
    [Test]
    public void NegotiateSession_WhenSessionExists_ItShouldReturnResultStatusOk()
    {
        // Arrange
        SetupSessions(1);
        const int sessionId = 1;
        const ResultStatus expected = ResultStatus.Ok;
        var uri = new Uri("http://pubsub");
        _pubSubMock.Setup(p => p
                .Negotiate(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(uri);
        // Act
        var result = _service.NegotiateSession(sessionId);

        // Assert
        Assert.That(result.Result.Status == expected);
    }

    [Test]
    public void NegotiateSession_WhenSessionExists_ItShouldReturnTypeOfSessionNegotiateDto()
    {
        // Arrange
        SetupSessions(1);
        const int sessionId = 1;
        var expected = typeof(SessionNegotiateDto);
        var uri = new Uri("http://pubsub");
        _pubSubMock.Setup(p => p
            .Negotiate(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(uri);

        // Act
        var result = _service.NegotiateSession(sessionId);

        // Assert
        Assert.That(result.Result.ValueType == expected);
    }

    [Test]
    public void NegotiateSession_WhenSessionDoesNotExist_ItShouldReturnResultStatusNotFound()
    {
        // Arrange
        const int sessionId = 1;
        const ResultStatus expected = ResultStatus.NotFound;

        // Act
        var result = _service.NegotiateSession(sessionId);

        // Assert
        Assert.That(result.Result.Status == expected);
    }
}
