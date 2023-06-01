using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ardalis.Result;
using Newtonsoft.Json;
using NUnit.Framework;
using RealtimeCv.Core.Models;
using RealtimeCv.Core.Models.Dto;
using RealtimeCv.UnitTests.Functions.Services;

namespace CleanArchitecture.UnitTests.Functions.Services;

public class ProjectServiceTests : ProjectServiceTestsBase
{
    [Test]
    public void GetProjectById_WhenProjectFound_ItShouldBeReturned()
    {
        // Arrange
        SetupProjects(2);
        const int expected = 1;

        // Act
        var result = _service.GetProjectById(1);
        
        // Assert
        Assert.That(result.Result.Value.Id == expected);
    }
    
    [Test]
    public void GetProjectById_WhenProjectFound_ItShouldReturnResultStatusOk()
    {
        // Arrange
        SetupProjects(2);
        const ResultStatus expected = ResultStatus.Ok;

        // Act
        var result = _service.GetProjectById(1);

        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void GetProjectById_WhenProjectFound_ItShouldReturnTypeOfProjectDto()
    {
        // Arrange
        SetupProjects(2);
        var expected = typeof(ProjectDto);

        // Act
        var result = _service.GetProjectById(1);

        // Assert
        Assert.That(result.Result.ValueType == expected);
    }

    [Test]
    public void GetProjectById_WhenProjectNotFound_ItShouldReturnResultStatusNotFound()
    {
        // Arrange
        SetupProjects(2);
        const ResultStatus expected = ResultStatus.NotFound;

        // Act
        var result = _service.GetProjectById(3);

        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void GetProjects_WhenProjectsFound_ItShouldReturnProjects()
    {
        // Arrange
        SetupProjects(2);
        const int expected = 2;

        // Act
        var result = _service.GetProjects();

        // Assert
        Assert.That(result.Result.Value.Count == expected);
    }
    
    [Test]
    public void GetProjects_WhenProjectsFound_ItShouldReturnTypeOfProjectsDtoList()
    {
        // Arrange
        SetupProjects(2);
        var expected = typeof(List<ProjectsDto>);

        // Act
        var result = _service.GetProjects();

        // Assert
        Assert.That(result.Result.ValueType == expected);
    }
    
    [Test]
    public void GetProjects_WhenProjectsNotFound_ItShouldReturnNoProjects()
    {
        // Arrange
        const int expected = 0;

        // Act
        var result = _service.GetProjects();

        // Assert
        Assert.That(result.Result.Value.Count == expected);
    }
    
    [Test]
    public void GetProjects_WhenProjectsNotFound_ItShouldReturnResultStatusOk()
    {
        // Arrange
        const ResultStatus expected = ResultStatus.Ok;

        // Act
        var result = _service.GetProjects();

        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void CreateProject_WhenInputValid_ItShouldReturnResultStatusOk()
    {
        // Arrange
        const ResultStatus expected = ResultStatus.Ok;
        var createDto = new ProjectCreateDto("Project1");
        
        // Act
        var result = _service.CreateProject(createDto);
        
        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void CreateProject_WhenInputValid_ItShouldIncreaseCount()
    {
        // Arrange
        var expected = 1;
        var createDto = new ProjectCreateDto("Project1");

        // Act
        var result = _service.CreateProject(createDto);

        // Assert
        Assert.That(_context.Project.Count() == expected);
    }
    
    [Test]
    public void CreateProject_WhenNameEmpty_ItShouldReturnResultStatusInvalid()
    {
        // Arrange
        var expected = ResultStatus.Invalid;
        var createDto = new ProjectCreateDto("");

        // Act
        var result = _service.CreateProject(createDto);

        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void CreateProject_WhenNameLongerThan100_ItShouldReturnResultStatusInvalid()
    {
        // Arrange
        var expected = ResultStatus.Invalid;
        var createDto = new ProjectCreateDto(new string ('x', 101));

        // Act
        var result = _service.CreateProject(createDto);

        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void CreateProject_WhenProjectCreated_ItShouldReturnTypeOfProjectDto()
    {
        // Arrange
        var expected = typeof(ProjectDto);
        var createDto = new ProjectCreateDto("Project1");

        // Act
        var result = _service.CreateProject(createDto);

        // Assert
        Assert.That(result.Result.ValueType == expected);
    }

    [Test]
    public void UpdateProject_WhenInputValid_ItShouldReturnResultStatusOk()
    {
        // Arrange
        SetupProjects(2);
        var dto = new ProjectUpdateDto(1, "UpdatedProject1");
        const ResultStatus expected = ResultStatus.Ok;
        
        // Act
        var result = _service.UpdateProject(dto);
        
        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void UpdateProject_WhenInputValid_ItShouldUpdate()
    {
        // Arrange
        SetupProjects(2);
        var expected = new ProjectUpdateDto(1, "UpdatedProject1");

        // Act
        var result = _service.UpdateProject(expected);

        // Assert
        Assert.That(result.Result.Value.Id == expected.Id);
    }
    
    [Test]
    public void UpdateProject_WhenNameEmpty_ItShouldReturnResultStatusInvalid()
    {
        // Arrange
        SetupProjects(2);
        var expected = ResultStatus.Invalid;
        var dto = new ProjectUpdateDto(1, "");
    
        // Act
        var result = _service.UpdateProject(dto);
    
        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void UpdateProject_WhenNameLongerThan100_ItShouldReturnResultStatusInvalid()
    {
        // Arrange
        SetupProjects(2);
        var expected = ResultStatus.Invalid;
        var dto = new ProjectUpdateDto(1, new string('x', 101));
    
        // Act
        var result = _service.UpdateProject(dto);
    
        // Assert
        Assert.That(result.Result.Status == expected);
    }
    
    [Test]
    public void UpdateProject_WhenProjectUpdated_ItShouldReturnTypeOfProjectDto()
    {
        // Arrange
        SetupProjects(2);
        var expected = typeof(ProjectDto);
        var dto = new ProjectUpdateDto(1, "UpdatedProject1");
    
        // Act
        var result = _service.UpdateProject(dto);
    
        // Assert
        Assert.That(result.Result.ValueType == expected);
    }

    [Test]
    public void DeleteProject_WhenProjectDeleted_ItShouldDecreaseCount()
    {
        // Arrange
        SetupProjects(2);
        var expected = 1;

        // Act
        var result = _service.DeleteProject(1);

        // Assert
        Assert.That(_context.Project.Count() == expected);
    }

    [Test]
    public void DeleteProject_WhenProjectDeleted_ItShouldReturnResultStatusOk()
    {
        // Arrange
        SetupProjects(2);
        const ResultStatus expected = ResultStatus.Ok;

        // Act
        var result = _service.DeleteProject(1);

        // Assert
        Assert.That(result.Result.Status.Equals(expected));
    }
    
    [Test]
    public void DeleteProject_WhenProjectNotFound_ItShouldReturnResultStatusNotFound()
    {
        // Arrange
        const ResultStatus expected = ResultStatus.NotFound;

        // Act
        var result = _service.DeleteProject(1);

        // Assert
        Assert.That(result.Result.Status.Equals(expected));
    }
}
