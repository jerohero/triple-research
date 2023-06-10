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
    protected Mock<IPubSub> _pubSubMock;

    [SetUp]
    public void Setup()
    {
        var mocker = new AutoMocker();
        
        _context?.Database.EnsureDeleted();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("TestDb").Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        var loggerMock = mocker.GetMock<ILoggerAdapter<SessionService>>();
        var kubernetesMock = mocker.GetMock<IKubernetesService>();
        _pubSubMock = mocker.GetMock<IPubSub>();
        
        var mapperConfig = new MapperConfiguration(cfg => 
            cfg.AddProfile(new AutomapperMaps())
        );
        var mapper = new Mapper(mapperConfig);

        _service = new SessionService(
            loggerMock.Object, mapper,
            new SessionRepository(_context), new VisionSetRepository(_context),
            kubernetesMock.Object, _pubSubMock.Object
        );
        
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
            TrainedModelId = 1,
        };
        
        _context.VisionSet.Add(fakeVisionSet);
        _context.SaveChanges();

        for (var i = 0; i < count; i++)
        {
            fakeSessions.Add(new Session
            {
                VisionSetId = fakeVisionSet.Id,
                Source = "rtsp://test.com",
                Pod = "cv-test-1",
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            });
        }
        
        _context.Session.AddRange(fakeSessions);
        _context.SaveChanges();
    }
}
