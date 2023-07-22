using Mvtt.Core.Ecs;

namespace Mvtt.Core.Demo.Components;

public class ChemicalEngineComponent : Component
{
    /// <summary>
    /// Is this engine Active
    /// </summary>
    public bool Activated { get; set; }

    /// <summary>
    /// liter per second
    /// </summary>
    public uint FuelUsage { get; set; }

    /// <summary>
    /// Tons
    /// </summary>
    public float Thrust { get; set; }

}