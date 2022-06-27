using Mvtt.Core.Core;
using Mvtt.Core.Demo.Components;
using Mvtt.Core.Ecs;

namespace Mvtt.Core.Demo.Systems;

[System]
public class PropulsionSystem
{
    [SystemMethod]
    public static void ChemicalEngine(
        PhysicalComponent pc,
        ChemicalEngineComponent engine,
        FuelTankComponent fuelTank,
        FlightComputerComponent fc)
    {
        if (engine.Activated) // is the engine on
        {
            //is there enuf fuel in the tanks
            if (fuelTank.FuelInTons - engine.FuelUsage > 0)
            {
                //consume the fule
                fuelTank.FuelInTons -= engine.FuelUsage;
                    
                //now apply acceleration
                //f = ma -> a = f / m
                    
                //NOTE: we are working in tons
                var a = new Vec3(engine.Thrust) / new Vec3(pc.Mass);
                    
                //now we need to apply the thrust vector
                var vector = fc.CurrentInstruction.ThrustVector.Normalized();

                //apply direction to acceleration
                pc.Acceleration += vector * a;
            }
        }
    }
        
    [SystemMethod]
    public static void ColdJetEngine(
        PhysicalComponent pc,
        ColdJetsEngineComponent engine,
        FuelTankComponent fuelTank,
        FlightComputerComponent fc)
    {
        if (engine.Activated) // is the engine on
        {
            //is there enuf fuel in the tanks
            if (fuelTank.FuelInTons - engine.FuelUsage > 0)
            {
                //consume the fule
                fuelTank.FuelInTons -= engine.FuelUsage;
                    
                //now apply acceleration
                //f = ma -> a = f / m
                    
                //NOTE: we are working in tons
                var a = new Vec3(engine.Thrust) / new Vec3(pc.Mass);
                    
                //now we need to apply the thrust vector
                var vector = fc.CurrentInstruction.ThrustVector.Normalized();

                //apply direction to acceleration
                pc.Acceleration += vector * a;
            }
        }
    }
}