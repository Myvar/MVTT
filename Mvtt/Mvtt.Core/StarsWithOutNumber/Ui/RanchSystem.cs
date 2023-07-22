using System.Numerics;
using ImGuiNET;
using Mvtt.Core.Core;
using Mvtt.Core.Ecs;
using Mvtt.Core.StarsWithOutNumber.Components;

namespace Mvtt.Core.StarsWithOutNumber.Ui;

[System]
public static class RanchSystem
{
    public static Vector2 MyPos = Vector2.Zero;

    [SystemUiMethod]
    public static void DrawRanch(RanchComponent ranchComponent)
    {
        ImGui.Begin("Ranch");

        var io = ImGui.GetIO();

        if (io.KeyCtrl)
        {
            ranchComponent.Zoom += io.MouseWheel * 0.01f;
        }


        if (ImGui.BeginTabBar("dsds"))
        {
            if (ImGui.BeginTabItem("Map"))
            {
                ImGui.Image(new IntPtr(EcsClientEngine.RanchMap.TextureId),
                    new Vector2(EcsClientEngine.RanchMap.Width * ranchComponent.Zoom,
                        EcsClientEngine.RanchMap.Height * ranchComponent.Zoom));

                var canvas = ImGui.GetForegroundDrawList();
                var mousePos = ImGui.GetMousePos();
                var scroll = new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());

                var r = 10;
                
                // var pos = (-scroll);
                var zoom = new Vector2(ranchComponent.Zoom, ranchComponent.Zoom);
                foreach (var token in ranchComponent.Tokens)
                {
                    var p = ((token.Active ? MyPos : token.Pos) *
                             zoom) + -scroll;
                    canvas.AddCircleFilled(p, r, token.Color);
                    canvas.AddText(p - new Vector2(r, 30), 0xFFFFFFFF, token.Name);

                    var x = new Vec2(p.X, p.Y);
                    var distanceTo = new Vec2(mousePos.X, mousePos.Y).DistanceTo(x);

                    if (distanceTo < r)
                    {
                        if (ImGui.IsMouseDragging(ImGuiMouseButton.Left) && ranchComponent.Tokens.All(x => !x.Active))
                        {
                            MyPos = token.Pos;
                            token.Active = true;
                        }


                        token.Color = 0xFF0000FF;
                    }
                    else
                    {
                        token.Color = 0xFFFFFFFF;
                    }

                    if (ImGui.IsMouseDragging(ImGuiMouseButton.Left) && token.Active)
                    {
                        MyPos += (ImGui.GetMouseDragDelta() * 1 / zoom);
                        if (DateTime.Now.Millisecond % 2 == 0)
                            token.Pos = MyPos;
                        ImGui.ResetMouseDragDelta();
                    }
                    else
                    {
                        if (token.Active)
                        {
                            token.Pos = MyPos;
                            token.Active = false;
                        }
                    }
                }

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Tokens"))
            {
                if (ImGui.Button("Add Token"))
                {
                    ranchComponent.Tokens.Add(new Token()
                    {
                        Name = "Unnamed",
                        Color = 0xFFFFFFFF,
                        Pos = new Vector2(100, 100)
                    });
                }

                ImGui.Separator();

                for (var i = 0; i < ranchComponent.Tokens.Count; i++)
                {
                    var token = ranchComponent.Tokens[i];
                    var x = token.Name;
                    ImGui.InputText("Name#" + i, ref x, 255);
                    token.Name = x;

                    if (ImGui.Button("Reset Pos"))
                    {
                        token.Pos = new Vector2(100, 100);
                    }

                    ImGui.Separator();
                }

                ImGui.EndTabItem();
            }

            ImGui.End();
        }


        ImGui.End();
    }
}