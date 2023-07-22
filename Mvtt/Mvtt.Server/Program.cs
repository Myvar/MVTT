//the first ship pos: [10, 10, 10]

using Mvtt.Core.Core;
using Mvtt.Core.Demo.Components;
using Mvtt.Core.Ecs;
using Mvtt.Core.StarsWithOutNumber.Components;

var sectorMap = EcsServerEngine.CreateNewEntity(
    new SectorMapComponent()
    {
    });

var ranchMap = EcsServerEngine.CreateNewEntity(
    new RanchComponent()
    {
    });

// var card = EcsServerEngine.CreateNewEntity(
//     new CardComponent()
//     {
//         Front = "Front",
//         Back = "Back",
//     });
//
// var guidA = EcsServerEngine.CreateNewEntity(
//     new TransponderComponent()
//     {
//         ShipName = "Ship A"
//     },
//     new PhysicalComponent()
//     {
//         Position = new Vec3(10),
//         Mass = 10_000 //ship is 10k tons in mass
//     },
//     new FuelTankComponent()
//     {
//         FuelInTons = 4000,
//         CapacityInTons = 4000
//     },
//     new ChemicalEngineComponent()
//     {
//         Thrust = 40_000,
//         FuelUsage = 1
//     },
//     new FlightComputerComponent()
//     {
//         Instructions = new List<FlightInstruction>()
//         {
//             new()
//             {
//                 Method = PropulsionMethod.MainSystem,
//                 BurnTime = 5,
//                 ThrustVector = new Vec3(0, 1, 0)
//             }
//         }
//     },
//     new SensorSweetComponent()
//     {
//         Range = 25
//     }
// );
//
// //the second ship pos: [0, 0, 0]
// var guidB = EcsServerEngine.CreateNewEntity(
//     new TransponderComponent()
//     {
//         ShipName = "Ship B"
//     },
//     new PhysicalComponent()
//     {
//         Position = new Vec3(0),
//         Mass = 10_000 //ship is 10k tons in mass
//     },
//     new FuelTankComponent()
//     {
//         FuelInTons = 4000,
//         CapacityInTons = 4000
//     },
//     new ChemicalEngineComponent()
//     {
//         Thrust = 40_000,
//         FuelUsage = 1
//     },
//     new FlightComputerComponent()
//     {
//         Instructions = new ()
//         {
//             new()
//             {
//                 Method = PropulsionMethod.MainSystem,
//                 BurnTime = 5,
//                 ThrustVector = new Vec3(-1, 0, 0)
//             },
//         }
//     },
//     new SensorSweetComponent()
//     {
//         Range = 25
//     }
// );
//
//
// var guidC = EcsServerEngine.CreateNewEntity(
//     new TransponderComponent() 
//     {
//         ShipName = "Planet"
//     },
//     new PhysicalComponent()
//     {
//         Position = new Vec3(-100, -100, -100),
//         Mass = 1_000_000_000_000 
//     }
// );


EcsServerEngine.Start();