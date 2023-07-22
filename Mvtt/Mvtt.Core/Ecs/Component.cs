namespace Mvtt.Core.Ecs;

public class Component
{
    public Guid Guid { get; set; }
    public bool HasChanged { get; set; }
}