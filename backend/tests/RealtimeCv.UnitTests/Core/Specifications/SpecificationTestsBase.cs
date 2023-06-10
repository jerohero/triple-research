using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Infrastructure.Data;
using RealtimeCv.Infrastructure.Data.Repositories;

namespace RealtimeCv.UnitTests.Core.Specifications;

public class SpecificationTestsBase
{
    protected AppDbContext _context;
    protected IVisionSetRepository _mockVisionSetRepository;
    protected ISessionRepository _mockSessionRepository;
    protected ITrainedModelRepository _mockTrainedModelRepository;
    protected IProjectRepository _mockProjectRepository;

    [SetUp]
    public void Setup()
    {
        _context?.Database.EnsureDeleted();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("TestDb").Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        
        _mockVisionSetRepository = new VisionSetRepository(_context);
        _mockSessionRepository =new SessionRepository(_context);
        _mockTrainedModelRepository = new TrainedModelRepository(_context);
        _mockProjectRepository = new ProjectRepository(_context);
        
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    protected void SetupSessions(int count)
    {
        var fakeSessions = new List<Session>();

        var fakeVisionSet = new VisionSet
        {
            Name = "test",
            ContainerImage = "test/image:latest",
            Sources = new List<string> { "rtsp://test.com" },
            ProjectId = 1,
            TrainedModelId = 1
        };

        for (var i = 0; i < count; i++)
        {
            fakeSessions.Add(new Session
            {
                VisionSetId = 1,
                Source = "rtsp://test.com",
                Pod = $"cv-test-{i}",
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        _context.VisionSet.Add(fakeVisionSet);
        _context.Session.AddRange(fakeSessions);
        _context.SaveChanges();
    }

    protected void SetupTrainedModels(int count)
    {
        var fakeModels = new List<TrainedModel>();
        var fakeProject = new Project
        {
            Name = "Project"
        };
        var fakeVisionSet = new VisionSet
        {
            Name = "test",
            ContainerImage = "test/image:latest",
            Sources = new List<string> { "rtsp://test.com" },
            ProjectId = 1,
            TrainedModelId = 1
        };
        var fakeSession = new Session
        {
            VisionSetId = 1,
            Source = "rtsp://test.com",
            Pod = "cv-test-1",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };
        
        for (var i = 0; i < count; i++)
        {
            fakeModels.Add(new TrainedModel
            {
                Id = i + 1,
                Name = "test",
                ProjectId = 1,
            });
        }

        _context.VisionSet.Add(fakeVisionSet);
        _context.Session.Add(fakeSession);
        _context.Project.Add(fakeProject);
        _context.TrainedModel.AddRange(fakeModels);
        _context.SaveChanges();
    }

    protected void SetupVisionSets(int count)
    {
        var fakeSets = new List<VisionSet>();
        
        var fakeTrainedModel = new TrainedModel
        {
            Id = 1,
            Name = "test.pt",
            IsUploadFinished = true,
            ProjectId = 1
        };

        var fakeSession = new Session
        {
            VisionSetId = 1,
            Source = "rtsp://test.com",
            Pod = $"cv-test-1",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        for (var i = 0; i < count; i++)
        {
            fakeSets.Add(new VisionSet
            {
                Name = "test",
                ContainerImage = "test/image:latest",
                Sources = new List<string> { "rtsp://test.com" },
                ProjectId = 1,
                TrainedModelId = 1
            });
        }

        _context.Session.Add(fakeSession);
        _context.TrainedModel.Add(fakeTrainedModel);
        _context.VisionSet.AddRange(fakeSets);
        _context.SaveChanges();
    }
    
    protected void SetupProjects(int count)
    {
        var fakeProjects = new List<Project>();

        for (var i = 0; i < count; i++)
        {
            fakeProjects.Add(new Project
            {
                Id = i + 1,
                Name = $"Project{i+1}",
                TrainedModels = new List<TrainedModel> { new TrainedModel { Id = i + 1, Name = $"model{i + 1}" } }
            });
        }
    
        _context.Project.AddRange(fakeProjects);
        _context.SaveChanges();
    }
}
