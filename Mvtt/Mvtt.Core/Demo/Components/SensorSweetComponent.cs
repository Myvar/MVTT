using Mvtt.Core.Ecs;

namespace Mvtt.Core.Demo.Components;

public class SensorSweetComponent : Component
{
    public float Range { get; set; }

    public List<Guid> EntitiesInRange { get; set; } = new();
}