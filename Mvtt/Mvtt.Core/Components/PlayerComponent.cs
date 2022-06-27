using Mvtt.Core.Ecs;

namespace Mvtt.Core.Components;

public class PlayerComponent : Component
{
    public string Username { get; set; }
    public string PlayerType { get; set; }
}