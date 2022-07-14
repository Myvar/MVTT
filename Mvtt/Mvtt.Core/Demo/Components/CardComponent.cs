using System.Numerics;
using ImGuiNET;
using Mvtt.Core.Assets;
using Mvtt.Core.Ecs;

namespace Mvtt.Core.Demo.Components;

public class CardComponent : Component
{
    public string Front { get; set; }
    public string Back { get; set; }
    public bool IsFrontUp { get; set; }
}

[System]
public static class CardSystem
{
    [SystemUiMethod]
    public static void DrawCards(CardComponent cc)
    {
        ImGui.Begin("Cards");

        if (ImGui.ImageButton(
                !cc.IsFrontUp
                    ? new IntPtr(EcsClientEngine.Front.TextureId)
                    : new IntPtr(EcsClientEngine.Back.TextureId), new Vector2(100, 250)))
        {
            cc.IsFrontUp = !cc.IsFrontUp;
        }


        ImGui.End();
    }
    
}