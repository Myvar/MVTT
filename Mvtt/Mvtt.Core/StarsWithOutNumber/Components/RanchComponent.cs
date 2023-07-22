using System.Numerics;
using Mvtt.Core.Ecs;

namespace Mvtt.Core.StarsWithOutNumber.Components;

public class Token
{
    public string Name { get; set; }
    public uint Color { get; set; }
    public Vector2 Pos { get; set; }
    public bool Active { get; set; }
}

public class RanchComponent : Component
{
    public float Zoom { get; set; } = 1;

    public List<Token> Tokens { get; set; } = new();
}