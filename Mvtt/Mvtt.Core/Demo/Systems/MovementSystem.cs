using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Mvtt.Core.Core;
using Mvtt.Core.Demo.Components;
using Mvtt.Core.Ecs;

namespace Mvtt.Core.Demo.Systems;

[System]
public class MovementSystem
{
    /*
     * We will impl newtons laws here
     */

    [SystemMethod]
    public static void HandelAcceleration(PhysicalComponent pc)
    {
        // add acceleration to velocity
        pc.Velocity += pc.Acceleration;
        pc.Acceleration = Vec3.Zero;
    }

    [SystemMethod]
    public static void HandelVelocity(PhysicalComponent pc)
    {
        // add velocity to position
        pc.Position += pc.Velocity;

        pc.HasChanged = true;
    }


    private static Dictionary<string, List<Vector2>> _paths = new();

    [SystemUiMethod]
    public static void SystemDisplay(PhysicalComponent pc, TransponderComponent t)
    {
        ImGui.Begin("System Display");
        var displaySize = ImGui.GetWindowSize() / 2;
        var windowLoc = ImGui.GetWindowPos();

        var scale = new Vector2(3);


        if (!_paths.ContainsKey(t.ShipName))
        {
            _paths.Add(t.ShipName, new List<Vector2>());
        }

        //dont add no meany dot that are so clsoe to one another
        _paths[t.ShipName].Add(new Vector2(pc.Position.X, pc.Position.Y));
        if (_paths[t.ShipName].Count > 1000)
        {
            _paths[t.ShipName].RemoveAt(0);
        }

        var drawList = ImGui.GetWindowDrawList();

        var circleColor = 0xFF0000FF;
        var textColor = 0xFFFFFFFF;


        var shipLocation = (new Vector2((pc.Position.X), (pc.Position.Y)) / scale) + displaySize + windowLoc;

        float normalize(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        for (var i = 1; i < _paths[t.ShipName].Count; i++)
        {
            var comp = (int)(255 * normalize(i, 0, _paths[t.ShipName].Count));
            var c = (uint)(((comp << 24) | (255 << 16) | (255 << 8) | 255) & 0xffffffffL);
            var bp = (_paths[t.ShipName][i - 1] / scale) + displaySize + windowLoc;
            var p = (_paths[t.ShipName][i] / scale) + displaySize + windowLoc;
            drawList.AddLine(bp, p, c);
        }

        drawList.AddCircleFilled(shipLocation, 5, circleColor);
        drawList.AddText(shipLocation, textColor, t.ShipName);

        ImGui.End();
    }
}