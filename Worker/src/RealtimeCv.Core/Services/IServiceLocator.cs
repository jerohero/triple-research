using System;
using Microsoft.Extensions.DependencyInjection;

namespace RealtimeCv.Core.Services;

public interface IServiceLocator : IDisposable
{
    IServiceScope CreateScope();
    T Get<T>();
}
