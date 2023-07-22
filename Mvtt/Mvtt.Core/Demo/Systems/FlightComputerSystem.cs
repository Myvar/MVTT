using ImGuiNET;
using Mvtt.Core.Core;
using Mvtt.Core.Demo.Components;
using Mvtt.Core.Ecs;
using Vector3 = System.Numerics.Vector3;

namespace Mvtt.Core.Demo.Systems;

[System]
public class FlightComputerSystem
{
    [SystemUiMethod]
    public static void TransponderUi(TransponderComponent t)
    {
        ImGui.Begin($"{t.ShipName}");

        ImGui.End();
    }

    [SystemUiMethod]
    public static void PhysicalUi(PhysicalComponent pc, TransponderComponent t)
    {
        ImGui.Begin($"{t.ShipName}");
        if (ImGui.CollapsingHeader("Vectors"))
        {
            {
                var z = pc.Position;
                var v = new Vector3(z.X, z.Y, z.Z);
                ImGui.DragFloat3("Position", ref v);
                pc.Position = new Vec3(v.X, v.Y, v.Z);
            }

            {
                var z = pc.Velocity;
                var v = new Vector3(z.X, z.Y, z.Z);
                ImGui.DragFloat3("Velocity", ref v);
                pc.Velocity = new Vec3(v.X, v.Y, v.Z);
            }

            {
                var v = pc.Mass;
                ImGui.DragFloat("Mass", ref v);
                pc.Mass = v;
            }
        }

        ImGui.End();
    }

    [SystemUiMethod]
    public static void FuleTankUi(FuelTankComponent ft, TransponderComponent t)
    {
        ImGui.Begin($"{t.ShipName}");
        if (ImGui.CollapsingHeader("Fuel Tank"))
        {
            ImGui.Text($"Fule: {ft.FuelInTons} / {ft.CapacityInTons}");
        }


        ImGui.End();
    }


    private static Vec3 ThrustVector = new Vec3(0);
    private static float BurnTime = 1f;

    [SystemUiMethod]
    public static void FlightComputerUi(PhysicalComponent pc, FlightComputerComponent fcs, TransponderComponent t)
    {
        ImGui.Begin($"{t.ShipName}");


        if (ImGui.CollapsingHeader("Flight Computer"))
        {
            ImGui.Separator();
            ImGui.Text("Add Instruction");
            ImGui.Separator();
            {
                var v = new Vector3(ThrustVector.X, ThrustVector.Y, ThrustVector.Z);
                ImGui.DragFloat3("ThrustVector", ref v);
                ThrustVector = new Vec3(v.X, v.Y, v.Z);

                ImGui.DragFloat("BurnTime", ref BurnTime);

                if (ImGui.Button("Add"))
                {
                    fcs.Instructions.Add(new FlightInstruction()
                    {
                        Method = PropulsionMethod.MainSystem,
                        BurnTime = BurnTime,
                        ThrustVector = ThrustVector
                    });
                }
            }

            ImGui.Separator();
            ImGui.Text("Instructions");
            ImGui.Separator();


            foreach (var instruction in fcs.Instructions)
            {
                ImGui.Text($"Method: {instruction.Method}");
                ImGui.Text($"BurnTime: {instruction.BurnTime}");
                ImGui.Text($"ThrustVector: {instruction.ThrustVector}");
                var v = new Vector3(instruction.ThrustVector.X, instruction.ThrustVector.Y, instruction.ThrustVector.Z);
                ImGui.DragFloat3("ThrustVector", ref v);
                instruction.ThrustVector = new Vec3(v.X, v.Y, v.Z);
                ImGui.Separator();
            }
        }

        ImGui.End();
    }

    [SystemMethod]
    public static void HandelMainEngines(
        FlightComputerComponent fcs,
        ChemicalEngineComponent engine)
    {
        if (fcs.CurrentInstruction != null
            && fcs.CurrentInstruction.Method == PropulsionMethod.MainSystem)
        {
            if (fcs.CurrentInstruction.BurnTime > 0)
            {
                engine.Activated = true;
                fcs.CurrentInstruction.BurnTime -= 1;
            }
            else
            {
                fcs.Instructions.RemoveAt(0);
                engine.Activated = false;
            }
        }
    }

    [SystemMethod]
    public static void HandelColdEngines(
        FlightComputerComponent fcs,
        ColdJetsEngineComponent engine)
    {
        if (fcs.CurrentInstruction != null
            && fcs.CurrentInstruction.Method == PropulsionMethod.MainSystem)
        {
            if (fcs.CurrentInstruction.BurnTime > 0)
            {
                engine.Activated = true;
                fcs.CurrentInstruction.BurnTime -= 1;
            }
            else
            {
                fcs.Instructions.RemoveAt(0);
                engine.Activated = false;
            }
        }
    }
}