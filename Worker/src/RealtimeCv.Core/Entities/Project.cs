using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RealtimeCv.Core.Entities;

public class Project : BaseEntity
{
  public string Name { get; set; }
}
