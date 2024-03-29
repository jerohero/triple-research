﻿using System;
using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IPubSub
{
    Task Send(object message, string group, string hub);
    Task<Uri> Negotiate(string hub, string group);
}
