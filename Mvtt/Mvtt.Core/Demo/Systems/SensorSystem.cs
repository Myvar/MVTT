using Mvtt.Core.Demo.Components;
using Mvtt.Core.Ecs;

namespace Mvtt.Core.Demo.Systems;

[System]
public class SensorSystem
{
    [SystemMethod]
    public static void UpdateSensors(
        PhysicalComponent pc,
        SensorSweetComponent sweet,
        // the query tag passes all the Components of al entities of that type to the arg
        //later il add a string argument to the atribute to add real querys
        [Query] PhysicalComponent[] targets)
    {
        sweet.EntitiesInRange.Clear();

        foreach (var target in targets)
        {
            if (target.Guid != pc.Guid) //we dont want to detect out self
            {
                //now check range
                var targetVector = pc.Position - target.Position;
                var distanceToTarget = targetVector.Length();

                if (distanceToTarget < sweet.Range) //target is in range
                {
                    sweet.EntitiesInRange.Add(target.Guid);
                }
            }
        }
    }
}