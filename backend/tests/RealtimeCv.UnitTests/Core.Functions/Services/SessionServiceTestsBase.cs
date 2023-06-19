using System;
using System.Collections.Generic;
using System.Diagnostics;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Functions.Config;
using RealtimeCv.Core.Functions.Services;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Infrastructure.Data;
using RealtimeCv.Infrastructure.Data.Repositories;

namespace RealtimeCv.UnitTests.Core.Functions.Services;

public class SessionServiceTestsBase
{
    protected SessionService _service;
    protected AppDbContext _context;
    protected VisionSet _visionSet;
    protected Mock<IPubSub> _pubSubMock;
    protected Mock<IKubernetesService> _kubernetesMock;

    [SetUp]
    public void Setup()
    {
        var mocker = new AutoMocker();
        
        _context?.Database.EnsureDeleted();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("TestDb").Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        var loggerMock = mocker.GetMock<ILoggerAdapter<SessionService>>();
        _kubernetesMock = mocker.GetMock<IKubernetesService>();
        _pubSubMock = mocker.GetMock<IPubSub>();
        
        var mapperConfig = new MapperConfiguration(cfg => 
            cfg.AddProfile(new AutomapperMaps())
        );
        var mapper = new Mapper(mapperConfig);

        _service = new SessionService(
            loggerMock.Object, mapper,
            new SessionRepository(_context), new VisionSetRepository(_context),
            _kubernetesMock.Object, _pubSubMock.Object
        );
        
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    protected void SetupSessions(int count)
    {
        var fakeProject = new Project
        {
            Id = 1,
            Name = "test"
        };
        
        _context.Project.Add(fakeProject);
        _context.SaveChanges();
        
        var fakeSessions = new List<Session>();
        _visionSet = new VisionSet
        {
            Name = "test",
            ContainerImage = "test/image:latest",
            Sources = new List<string> { "rtsp://test.com" },
            ProjectId = 1,
            Project = fakeProject,
            TrainedModelId = 1,
        };
        
        _context.VisionSet.Add(_visionSet);
        _context.SaveChanges();

        for (var i = 0; i < count; i++)
        {
            fakeSessions.Add(new Session
            {
                VisionSetId = _visionSet.Id,
                Source = "rtsp://test.com",
                Pod = "cv-test-1",
                CreatedAt = DateTime.UtcNow
            });
        }
        
        _context.Session.AddRange(fakeSessions);
        _context.SaveChanges();
    }
}
