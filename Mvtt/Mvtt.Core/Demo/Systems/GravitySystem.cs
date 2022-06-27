using Mvtt.Core.Core;
using Mvtt.Core.Demo.Components;
using Mvtt.Core.Ecs;

namespace Mvtt.Core.Demo.Systems;

[System]
public class GravitySystem
{
    public const double G = 6.6720e-08;

    [SystemMethod]
    public static void ApplyGravity(
        PhysicalComponent pc,
        [Query] PhysicalComponent[] targets)
    {
        
        foreach (var target in targets)
        {
            if (target.Guid != pc.Guid) //we dont want to detect our self
            {
                // F = G * ((m1 * m2)/(r ^ 2))
                // a = f / m    [f = ma]

                //first we get the direction that gravity should be applied in
                var targetVector = pc.Position - target.Position;
                var targetDir = targetVector.Normalized();

                //now calc the force of gravity
                var r = targetVector.Length();
                var f = (float)G * ((pc.Mass * target.Mass) / (MathF.Pow(r, 2)));

                //now calc acceleration due to gravity
                var a = f / pc.Mass;

                //now we apply the acceleration on the direction vector
                pc.Acceleration += new Vec3(a) * -targetDir;
            }
        }
    }
}