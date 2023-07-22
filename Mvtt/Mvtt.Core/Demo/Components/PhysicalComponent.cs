using Mvtt.Core.Core;
using Mvtt.Core.Ecs;

namespace Mvtt.Core.Demo.Components;

public class PhysicalComponent : Component
{
    public Vec3 Position { get; set; } = new(0);
    public Vec3 Velocity { get; set; } = new(0);
    public Vec3 Acceleration { get; set; } = new(0);
        
    /// <summary>
    /// Tons
    /// </summary>
    public float Mass { get; set; } = 0;
}