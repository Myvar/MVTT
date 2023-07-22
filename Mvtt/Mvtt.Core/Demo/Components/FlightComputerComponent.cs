using Mvtt.Core.Core;
using Mvtt.Core.Ecs;

namespace Mvtt.Core.Demo.Components;

public enum PropulsionMethod
{
    MainSystem,
    ColdSystem
}

public class FlightInstruction
{
    public Vec3 ThrustVector { get; set; } = new();
    public float BurnTime { get; set; }
    public PropulsionMethod Method { get; set; }
}

public class FlightComputerComponent : Component
{
    public List<FlightInstruction> Instructions { get; set; } = new();
    public FlightInstruction CurrentInstruction => Instructions.FirstOrDefault();
}