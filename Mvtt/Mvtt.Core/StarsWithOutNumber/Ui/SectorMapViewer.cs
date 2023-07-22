using ImGuiNET;
using Mvtt.Core.Demo.Components;
using Mvtt.Core.Ecs;
using System.Numerics;
using Mvtt.Core.StarsWithOutNumber.Components;

namespace Mvtt.Core.StarsWithOutNumber.Ui;

[System]
public static class SectorMapSystem
{
    [SystemUiMethod]
    public static void DrawSectorMap(SectorMapComponent cc)
    {
        ImGui.Begin("SectorMap");

        var size = ImGui.GetWindowSize();
        ImGui.Image(new IntPtr(EcsClientEngine.SectorMap.TextureId), new Vector2(EcsClientEngine.SectorMap.Width, EcsClientEngine.SectorMap.Height));

        ImGui.End();
    }
}