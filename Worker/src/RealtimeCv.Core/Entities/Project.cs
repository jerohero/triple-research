using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Ardalis.GuardClauses;

namespace RealtimeCv.Core.Entities;

public class Project : BaseEntity
{
  public string Name { get; set; }
  
  public Project(string name)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
  }
  
  public void UpdateName(string newName)
  {
    Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
  }
}
