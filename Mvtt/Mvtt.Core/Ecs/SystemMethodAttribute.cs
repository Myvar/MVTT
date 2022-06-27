namespace Mvtt.Core.Ecs;

[AttributeUsage(
    AttributeTargets.Method,
    AllowMultiple = false)]
public class SystemMethodAttribute : Attribute
{
}