using Ardalis.GuardClauses;

namespace RealtimeCv.Infrastructure.Entities;

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
