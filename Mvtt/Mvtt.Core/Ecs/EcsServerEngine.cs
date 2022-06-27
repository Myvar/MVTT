using System.Collections;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Mvtt.Core.Demo.Components;
using Mvtt.Core.Packets;
using Mvtt.Core.UI;
using Newtonsoft.Json;
using OpenTK;

namespace Mvtt.Core.Ecs;

public class NetworkClient : IDisposable
{
    public TcpClient Client { get; set; }
    public NetworkStream NetworkStream { get; set; }
    public BinaryReader Reader { get; set; }
    public BinaryWriter Writer { get; set; }

    public NetworkClient(TcpClient client)
    {
        Client = client;

        NetworkStream = Client.GetStream();
        Reader = new BinaryReader(NetworkStream, Encoding.Default, true);
        Writer = new BinaryWriter(NetworkStream, Encoding.Default, true);
    }

    public void Dispose()
    {
        Client.Dispose();
        Reader.Dispose();
        Writer.Dispose();
    }
}

public static class EcsServerEngine
{
    private static TcpListener _listener;
    private static TcpListener _fileListener;
    private static Dictionary<int, Type> _packets = new();
    private static List<NetworkClient> _clients { get; set; } = new();

    static EcsServerEngine()
    {
        _listener = TcpListener.Create(6969);
        _fileListener = TcpListener.Create(6970);
        Load();
    }

    //we dont want to allow outside change because we might need to rebuild the cache


    private static List<Component> Components { get; set; } = new();
    private static object _locker = new object();
    private static List<SystemMethodEntry> _systems { get; set; } = new();

    private static void Load()
    {
        //we will cache components fields for the query system

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.BaseType == typeof(Packet))
            {
                var instance = (Packet)Activator.CreateInstance(type);

                _packets.Add(instance.Id, type);
            }
            else if (type.BaseType == typeof(Component))
            {
                //register fields for query validation
            }
            else if (type.CustomAttributes.Any(x => x.AttributeType == typeof(SystemAttribute)))
            {
                //register system for later execution

                //all methods must be static with SystemMethod Attribute
                //All arguments must inherit a Component Type at some point in the chain

                foreach (var method in type.GetMethods())
                {
                    if (method.CustomAttributes.Any(x => x.AttributeType == typeof(SystemMethodAttribute)))
                    {
                        if (!method.IsStatic && IsValidSystemMethod(method.GetParameters()))
                        {
                            throw new Exception("Not a valid SystemMethod");
                        }

                        //here we can register it
                        _systems.Add(new SystemMethodEntry(method));
                    }
                }
            }
        }
    }

    private static bool IsValidSystemMethod(ParameterInfo[] param)
    {
        foreach (var info in param)
        {
            if (!IsComponent(info.ParameterType)) return false;
        }

        return true;
    }

    private static bool IsComponent(Type type)
    {
        if (type.BaseType == typeof(Component))
        {
            return true;
        }

        return IsComponent(type.BaseType);
    }


    public static Guid CreateNewEntity(params Component[] comps)
    {
        var guid = Guid.NewGuid();

        foreach (var comp in comps)
        {
            comp.Guid = guid;
            BroadcastPacket(new UpdateOrCreateComponentPacket()
            {
                ComponentType = comp.GetType().ToString(),
                ComponentJson = JsonConvert.SerializeObject(comp)
            });
        }

        lock (_locker)
        {
            Components.AddRange(comps);
        }

        UpdateCache();

        return guid;
    }

    private static int TotalEntities { get; set; }

    private static void UpdateCache()
    {
        //fist we build an entity grouping
        var dict = new Dictionary<Guid, List<int>>();
        TotalEntities = dict.Count;
        for (var i = 0; i < Components.Count; i++)
        {
            lock (_locker)
            {
                var component = Components[i];
                if (!dict.ContainsKey(component.Guid))
                {
                    dict.Add(component.Guid, new List<int>());
                }

                if (!dict[component.Guid].Contains(i))
                {
                    dict[component.Guid].Add(i);
                }
            }
        }

        //clear cache
        foreach (var system in _systems)
        {
            system.Clear();
        }


        //now we create Indices cache

        foreach (var entity in dict)
        {
            foreach (var component in entity.Value)
            {
                foreach (var system in _systems)
                {
                    //first determin if the entity as all the requerd args

                    var types = new List<Type>();
                    foreach (var i in entity.Value)
                    {
                        lock (_locker)
                        {
                            types.Add(Components[i].GetType());
                        }
                    }

                    foreach (var type in system.Arguments)
                    {
                        if (!types.Contains(type))
                        {
                            goto _continue;
                        }
                    }

                    for (var i = 0; i < system.Arguments.Count; i++)
                    {
                        var argument = system.Arguments[i];
                        lock (_locker)
                        {
                            if (Components[component].GetType() == argument)
                            {
                                system.Indices[i].Add(component);
                            }
                        }
                    }

                    _continue: ;
                }
            }
        }
    }

    public static void Step()
    {
        foreach (var system in _systems)
        {
            for (int i = 0; i < system.Indices.First().Count; i++)
            {
                var args = new List<object>();
                lock (_locker)
                {
                    for (var index = 0; index < system.Arguments.Count; index++)
                    {
                        Components[system.Indices[index][i]].HasChanged = true;
                        args.Add(Components[system.Indices[index][i]]);
                    }

                    if (system.HasQuery)
                    {
                        //no we do the query
                        //but for now it will be a "any" query


                        var listType = typeof(List<>);
                        var constructedListType = listType.MakeGenericType(system.QueryType.GetElementType());

                        var lst = Activator.CreateInstance(constructedListType) as IList;
                        lock (_locker)
                        {
                            foreach (var component in Components)
                            {
                                if (component.GetType() == system.QueryType.GetElementType())
                                {
                                    lst.Add(component);
                                }
                            }
                        }

                        var elementType = system.QueryType.GetElementType();
                        var castMethod = typeof(Enumerable).GetMethod("Cast")
                            .MakeGenericMethod(elementType);
                        var toArrayMethod = typeof(Enumerable).GetMethod("ToArray")
                            .MakeGenericMethod(elementType);

                        var castedObjectEnum = castMethod.Invoke(null, new object[] { lst });
                        var castedObject = toArrayMethod.Invoke(null, new object[] { castedObjectEnum });

                        args.Add(castedObject);
                    }


                    system.Info.Invoke(null, args.ToArray());
                }
            }
        }


        foreach (var component in Components)
        {
            if (component is PhysicalComponent pc)
            {
            }

            if (component.HasChanged)
            {
                lock (_locker)
                {
                    //here we can send a network update


                    BroadcastPacket(new UpdateOrCreateComponentPacket()
                    {
                        ComponentType = component.GetType().ToString(),
                        ComponentJson = JsonConvert.SerializeObject(component)
                    });

                    component.HasChanged = false;
                }
            }
        }
    }


    private static void BroadcastPacket(Packet p)
    {
        foreach (var client in _clients)
        {
            SendPacket(client, p);
        }
    }

    private static void SendPacket(NetworkClient client, Packet p)
    {
        client.Writer.Write(p.Id);
        p.Write(client.Writer);
    }

    private static void NetworkTick()
    {
        //handel incomming packets here
        foreach (var client in _clients)
        {
            if (client.NetworkStream.DataAvailable)
            {
                var id = client.Reader.ReadInt32();

                var packet = (Packet)Activator.CreateInstance(_packets[id]);
                packet.Read(client.Reader);


                switch (packet)
                {
                    case UpdateOrCreateComponentPacket ucp:
                        lock (_locker)
                        {
                            var comp = (Component)JsonConvert.DeserializeObject(ucp.ComponentJson,
                                Type.GetType(ucp.ComponentType)!)!;

                            if (Components.All(x =>
                                    !(x.Guid == comp.Guid && x.GetType().ToString() == ucp.ComponentType)))
                            {
                                Components.Add(comp);
                            }
                            else
                            {
                                var index = Components.IndexOf(Components.First(x =>
                                    x.Guid == comp.Guid && x.GetType().ToString() == ucp.ComponentType));
                                Components[index] = comp;
                            }
                        }

                        SendPacket(client, ucp);

                        break;
                }
            }
        }
    }

    private static void FileServerTick()
    {
        var ftcp = _fileListener.AcceptTcpClient();
        ThreadPool.QueueUserWorkItem((state) =>
        {
            using var f = (TcpClient)state;
            var ns = f.GetStream();
            var br = new BinaryReader(ns);
            var bw = new BinaryWriter(ns);

            var mode = br.ReadByte();
            var guild = br.ReadString();
            var path = br.ReadString();
            path = path.Replace("..", "");

            path = Path.Combine($"./state/{guild}/", path);

            if (!File.Exists(path))
            {
                bw.Write((byte)1);
                return;
            }

            bw.Write((byte)2);
            if (mode == 1)
            {
                using (var file = File.Open(path, FileMode.OpenOrCreate))
                {
                    file.CopyTo(ns);
                }
            }
            else if (mode == 2)
            {
                using (var file = File.Open(path, FileMode.OpenOrCreate))
                {
                    ns.CopyTo(file);
                }
            }
        }, ftcp);
    }

    public static void Start()
    {
        _listener.Start();
        _fileListener.Start();

        //move to thread
        ThreadPool.QueueUserWorkItem((state) =>
        {
            while (true)
            {
                var client = new NetworkClient(_listener.AcceptTcpClient());

                _clients.Add(client);

                foreach (var component in Components)
                {
                    SendPacket(client, new UpdateOrCreateComponentPacket()
                    {
                        ComponentType = component.GetType().ToString(),
                        ComponentJson = JsonConvert.SerializeObject(component)
                    });
                }
            }
        });
        ThreadPool.QueueUserWorkItem((state) =>
        {
            while (true)
            {
                NetworkTick();
                Thread.Sleep(25);
            }
        });

        ThreadPool.QueueUserWorkItem((state) =>
        {
            while (true)
            {
                FileServerTick();
                Thread.Sleep(25);
            }
        });


        while (true)
        {
            if (_clients.Count < 1)
            {
                Thread.Sleep(25);
                continue;
            }


            // NetworkTick();

            Step();
            //Thread.Sleep(1000 / 60);
            Thread.Sleep(500);
        }
    }
}