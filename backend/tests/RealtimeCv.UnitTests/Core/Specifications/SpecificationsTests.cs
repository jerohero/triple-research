using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using RealtimeCv.Core.Specifications;

namespace RealtimeCv.UnitTests.Core.Specifications;

public class SpecificationsTests : SpecificationTestsBase
{
    [Test]
    public void ActiveVisionSetSessionsBySourceSpec_WhenSessionsActive_ShouldFilterSessions()
    {
        // Arrange
        SetupSessions(2);
        const int expected = 2;
        foreach (var session in _context.Session)
        {
            session.IsActive = true;
            _context.Session.Update(session);
        }
        _context.SaveChanges();
        const string source = "rtsp://test.com";
        var spec = new ActiveVisionSetSessionsBySourceSpec(1, source);

        // Act
        var result = _mockSessionRepository.ListAsync(spec);

        // Assert
        Assert.That(result.Result.Count == expected);
    }
    
    [Test]
    public void ActiveVisionSetSessionsBySourceSpec_WhenSessionsInactive_ShouldFilterEmpty()
    {
        // Arrange
        SetupSessions(2);
        const int expected = 0;
        const string source = "rtsp://test.com";
        var spec = new ActiveVisionSetSessionsBySourceSpec(1, source);

        // Act
        var result = _mockSessionRepository.ListAsync(spec);

        // Assert
        Assert.That(result.Result.Count == expected);
    }

    [Test]
    public void ProjectSpec_WhenProjectWithIdExists_ShouldFilterProject()
    {
        // Arrange
        SetupProjects(2);
        const int expected = 1;
        var spec = new ProjectSpec(expected);

        // Act
        var result = _mockProjectRepository.SingleOrDefaultAsync(spec);

        // Assert
        Assert.That(result.Result.Id == expected);
    }

    [Test]
    public void SessionsByVisionSet_WhenSessionsByVisionSetExist_ShouldFilterSessions()
    {
        // Arrange
        SetupSessions(2);
        const int expected = 2;
        var spec = new SessionsByVisionSet(1);

        // Act
        var result = _mockSessionRepository.ListAsync(spec);

        // Assert
        Assert.That(result.Result.Count == expected);
    }

    [Test]
    public void SessionWithVisionSetSpec_WhenSessionAndVisionSetExists_ShouldFilterSessionWithVisionSet()
    {
        // Arrange
        SetupSessions(2);
        const int expected = 1;
        var spec = new SessionWithVisionSetSpec(1);

        // Act
        var result = _mockSessionRepository.SingleOrDefaultAsync(spec);

        // Assert
        Assert.That(result.Result.VisionSet.Id == expected);
    }

    [Test]
    public void TrainedModelByNameSpec_WhenTrainedModelExists_ShouldFilterTrainedModel()
    {
        // Arrange
        SetupTrainedModels(2);
        const int expected = 1;
        var spec = new TrainedModelByNameSpec("test");

        // Act
        var result = _mockTrainedModelRepository.SingleOrDefaultAsync(spec);

        // Assert
        Assert.That(result.Result.Id == expected);
    }

    [Test]
    public void TrainedModelBySessionSpec_WhenTrainedModelExists_ShouldFilterTrainedModel()
    {
        // Arrange
        SetupTrainedModels(2);
        const int expected = 1;
        var spec = new TrainedModelBySessionSpec(expected);

        // Act
        var result = _mockTrainedModelRepository.SingleOrDefaultAsync(spec);

        // Assert
        Assert.That(result.Result.Id == expected);
    }

    [Test]
    public void TrainedModelsByProject_WhenTrainedModelsExists_ShouldFilterTrainedModels()
    {
        // Arrange
        SetupTrainedModels(2);
        const int expected = 2;
        var spec = new TrainedModelsByProject(1);

        // Act
        var result = _mockTrainedModelRepository.ListAsync(spec);

        // Assert
        Assert.That(result.Result.Count == expected);
    }

    [Test]
    public void VisionSetBySession_WhenVisionSetExists_ShouldFilterVisionSet()
    {
        // Arrange
        SetupVisionSets(2);
        var spec = new VisionSetBySession(1);

        // Act
        var result = _mockVisionSetRepository.SingleOrDefaultAsync(spec);

        // Assert
        Assert.NotNull(result.Result);
    }

    [Test]
    public void VisionSetWithTrainedModelSpec_WhenVisionSetAndTrainedModelExists_ShouldFilterVisionSetWithTrainedModel()
    {
        // Arrange
        SetupVisionSets(2);
        const int expected = 1;
        var spec = new VisionSetWithTrainedModelSpec(1);

        // Act
        var result = _mockVisionSetRepository.SingleOrDefaultAsync(spec);

        // Assert
        Assert.That(result.Result.TrainedModel.Id == expected);
    }
}
