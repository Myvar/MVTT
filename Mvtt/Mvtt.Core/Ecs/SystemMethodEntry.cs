using System.Net.Sockets;
using System.Reflection;

namespace Mvtt.Core.Ecs;



public class SystemMethodEntry
{
    
    
    public List<Type> Arguments { get; set; } = new();
    public MethodInfo Info;

    public bool HasQuery;

    public List<List<int>> Indices { get; set; } = new();

    public QueryAttribute QueryAttribute { get; set; }
    public Type QueryType { get; set; }

    public SystemMethodEntry(MethodInfo info)
    {
        Info = info;

        foreach (var parameter in info.GetParameters())
        {
            if (!parameter.CustomAttributes.Any())
            {
                Arguments.Add(parameter.ParameterType);
            }
            else
            {
                QueryAttribute = parameter.GetCustomAttribute<QueryAttribute>();
                HasQuery = true;
                QueryType = parameter.ParameterType;
            }
        }

        Clear();
    }

    public void Clear()
    {
        Indices.Clear();
        foreach (var argument in Arguments)
        {
            Indices.Add(new List<int>());
        }
    }
}