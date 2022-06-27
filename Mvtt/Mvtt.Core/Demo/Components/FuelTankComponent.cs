using Mvtt.Core.Ecs;

namespace Mvtt.Core.Demo.Components;

public class FuelTankComponent : Component
{
    public uint CapacityInTons { get; set; }
    public uint FuelInTons { get; set; }
}