using System;
using Microsoft.Extensions.DependencyInjection;

namespace RealtimeCv.Core.Interfaces;

public interface IServiceLocator : IDisposable
{
    IServiceScope CreateScope();
    T Get<T>();
}
