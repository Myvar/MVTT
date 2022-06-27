using ImGuiNET;
using Mvtt.Core.Components;
using Mvtt.Core.Demo.Components;
using Mvtt.Core.Ecs;

namespace Mvtt.Core.Systems;

[System]
public class PlayerSystem
{
    [SystemUiMethod]
    public static void PlayerList(PlayerComponent pc)
    {
        ImGui.Begin($"Players");

        if (ImGui.CollapsingHeader(pc.Username))
        {
            ImGui.Text($"Name: {pc.Username}");    
            ImGui.Text($"PlayerType: {pc.PlayerType}");    
        }
        
        ImGui.End();
    }
}