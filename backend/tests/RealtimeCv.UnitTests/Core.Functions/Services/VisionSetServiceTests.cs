using System;
using Ardalis.Result;
using NUnit.Framework;
using RealtimeCv.Core.Models.Dto;

namespace RealtimeCv.UnitTests.Core.Functions.Services;

[TestFixture]
public class VisionSetServiceTests : VisionSetServiceTestsBase
{
    [Test]
    public void GetVisionSetById_WhenVisionSetFound_ItShouldReturnVisionSet()
    {
        // Arrange
        SetupVisionSets(2);
        const int expected = 1;

        // Act
        var result = _service.GetVisionSetById(1).Result;

        // Assert
        Assert.That(result.Value.Id == expected);
    }

    [Test]
    public void GetVisionSetById_WhenVisionSetNotFound_ItShouldReturnResultStatusNotFound()
    {
        // Arrange
        SetupVisionSets(2);

        // Act
        var result = _service.GetVisionSetById(3).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.NotFound);
    }

    [Test]
    public void GetVisionSetsByProject_WhenVisionSetsFound_ItShouldReturnVisionSets()
    {
        // Arrange
        SetupVisionSets(2);
        const int expected = 2;

        // Act
        var result = _service.GetVisionSetsByProject(1).Result;

        // Assert
        Assert.That(result.Value.Count == expected);
    }

    [Test]
    public void GetVisionSetsByProject_WhenNoVisionSetsFound_ItShouldReturnEmptyList()
    {
        // Arrange

        // Act
        var result = _service.GetVisionSetsByProject(1).Result;

        // Assert
        Assert.That(result.Value.Count == 0);
    }

    [Test]
    public void CreateVisionSet_WhenInputValid_ItShouldReturnResultStatusOk()
    {
        // Arrange
        SetupVisionSets(1);
        var createDto = new VisionSetCreateDto(
            1,
            "visionSet1",
            new[] { "rtsp://test.com" },
            "test/image:latest",
            1
        );

        // Act
        var result = _service.CreateVisionSet(createDto).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.Ok);
    }

    [Test]
    public void CreateVisionSet_WhenNameEmpty_ItShouldReturnResultStatusInvalid()
    {
        // Arrange
        var createDto = new VisionSetCreateDto(
            1,
            "",
            new[] { "rtsp://test.com" },
            "test/image:latest",
            1
        );

        // Act
        var result = _service.CreateVisionSet(createDto).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.Invalid);
    }
    
    [Test]
    public void CreateVisionSet_WhenSourcesEmpty_ItShouldReturnResultStatusInvalid()
    {
        // Arrange
        var createDto = new VisionSetCreateDto(
            1,
            "visionSet1",
            Array.Empty<string>(),
            "test/image:latest",
            1
        );

        // Act
        var result = _service.CreateVisionSet(createDto).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.Invalid);
    }
    
    [Test]
    public void CreateVisionSet_WhenContainerImageEmpty_ItShouldReturnResultStatusInvalid()
    {
        // Arrange
        var createDto = new VisionSetCreateDto(
            1,
            "visionSet1",
            Array.Empty<string>(),
            "",
            1
        );

        // Act
        var result = _service.CreateVisionSet(createDto).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.Invalid);
    }

    [Test]
    public void CreateVisionSet_WhenTrainedModelNotFound_ItShouldReturnResultStatusNotFound()
    {
        // Arrange
        var createDto = new VisionSetCreateDto(
            1,
            "visionSet1",
            new[] { "rtsp://test.com" },
            "test/image:latest",
            99
        );

        // Act
        var result = _service.CreateVisionSet(createDto).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.NotFound);
    }
    
    [Test]
    public void CreateVisionSet_WhenProjectNotFound_ItShouldReturnResultStatusNotFound()
    {
        // Arrange
        var createDto = new VisionSetCreateDto(
            99,
            "visionSet1",
            new[] { "rtsp://test.com" },
            "test/image:latest",
            1
        );

        // Act
        var result = _service.CreateVisionSet(createDto).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.NotFound);
    }

    [Test]
    public void UpdateVisionSet_WhenInputValid_ItShouldReturnResultStatusOk()
    {
        // Arrange
        SetupVisionSets(2);
        var dto = new VisionSetUpdateDto(
            1,
            "UpdatedVisionSet1",
            new[] { "rtsp://test.com" },
            "test/image:latest",
            1
        );

        // Act
        var result = _service.UpdateVisionSet(dto).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.Ok);
    }
    
        [Test]
    public void UpdateVisionSet_WhenInputInvalid_ItShouldReturnResultStatusInvalid()
    {
        // Arrange
        SetupVisionSets(2);
        var dto = new VisionSetUpdateDto(
            1,
            "",
            new[] { "rtsp://test.com" },
            "test/image:latest",
            1
        );

        // Act
        var result = _service.UpdateVisionSet(dto).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.Invalid);
    }

    [Test]
    public void UpdateVisionSet_WhenVisionSetNotFound_ItShouldReturnResultStatusNotFound()
    {
        // Arrange
        SetupVisionSets(2);
        var dto = new VisionSetUpdateDto(
            99,
            "UpdatedVisionSet1",
            new[] { "rtsp://test.com" },
            "test/image:latest",
            1
        );

        // Act
        var result = _service.UpdateVisionSet(dto).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.NotFound);
    }
    
    [Test]
    public void UpdateVisionSet_WhenTrainedModelNotFound_ItShouldReturnResultStatusNotFound()
    {
        // Arrange
        SetupVisionSets(2);
        var dto = new VisionSetUpdateDto(
            1,
            "UpdatedVisionSet1",
            new[] { "rtsp://test.com" },
            "test/image:latest",
            99
        );

        // Act
        var result = _service.UpdateVisionSet(dto).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.NotFound);
    }

    [Test]
    public void DeleteVisionSet_WhenVisionSetDeleted_ItShouldReturnResultStatusOk()
    {
        // Arrange
        SetupVisionSets(2);

        // Act
        var result = _service.DeleteVisionSet(1).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.Ok);
    }

    [Test]
    public void DeleteVisionSet_WhenVisionSetNotFound_ItShouldReturnResultStatusNotFound()
    {
        // Arrange
        SetupVisionSets(2);

        // Act
        var result = _service.DeleteVisionSet(99).Result;

        // Assert
        Assert.That(result.Status == ResultStatus.NotFound);
    }
}
