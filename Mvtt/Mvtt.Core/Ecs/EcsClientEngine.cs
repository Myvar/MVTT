using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Mvtt.Core.Assets;
using Mvtt.Core.Components;
using Mvtt.Core.Packets;
using Mvtt.Core.UI;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Vector2 = System.Numerics.Vector2;

namespace Mvtt.Core.Ecs;

public class SystemUiMethodEntry
{
    private MethodInfo _method;

    public SystemUiMethodEntry(MethodInfo method)
    {
        _method = method;
    }

    public ParameterInfo[] Parameters => _method.GetParameters();

    public void Run(object[] args)
    {
        _method.Invoke(null, args);
    }
}

public static class EcsClientEngine
{
    static EcsClientEngine()
    {
        Load();
    }

    public static List<SystemUiMethodEntry> UiSystems { get; set; } = new();

    public static List<Component> Components { get; set; } = new();
    private static object _locker = new object();

    private static TcpClient _client;
    private static Dictionary<int, Type> _packets = new();


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
                    if (method.CustomAttributes.Any(x => x.AttributeType == typeof(SystemUiMethodAttribute)))
                    {
                        if (!method.IsStatic && IsValidSystemMethod(method.GetParameters()))
                        {
                            throw new Exception("Not a valid SystemUiMethod");
                        }

                        //here we can register it
                        UiSystems.Add(new SystemUiMethodEntry(method));
                    }
                }
            }
        }
    }

    private static object _packetLocker = new object();

    private static void SendPacket(Packet p)
    {
        lock (_packetLocker)
        {
            try
            {
                bw.Write(p.Id);
                p.Write(bw);
            }
            catch (Exception e)
            {
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

    //we dont want to allow outside change because we might need to rebuild the cache

    //This is what we should sync


    public static Guid CreateNewEntity(params Component[] comps)
    {
        var guid = Guid.NewGuid();

        foreach (var comp in comps)
        {
            comp.Guid = guid;
            var p = new UpdateOrCreateComponentPacket();
            p.ComponentType = comp.GetType().ToString();
            p.ComponentJson = JsonConvert.SerializeObject(comp);

            SendPacket(p);
        }

        lock (_locker)
        {
            Components.AddRange(comps);
        }

        return guid;
    }

    private static BasicTexture _texture { get; set; }

    private static void LoadAssets()
    {
        _texture = new BasicTexture("myvar", "DevTexture/uv_1.png");
    }

    public static bool LoggedIn { get; set; }
    public static string Username = "";
    private static int SelectedType = 0;
    private static string[] PlayerTypes = { "PC", "NPC", "GM" };

    public static string PlayerType => PlayerTypes[SelectedType];

    public static void LoginUi()
    {
        var notValidUsername = string.IsNullOrEmpty(Username);

        // ImGui.ShowDemoWindow();
        var io = ImGui.GetIO();

        var displaySize = io.DisplaySize;

        ImGui.SetNextWindowSize(new Vector2(350, 100));
        ImGui.Begin("Login",
            //ImGuiWindowFlags.AlwaysAutoResize |
            ImGuiWindowFlags.Modal |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoResize |
            ImGuiWindowFlags.NoDecoration
        );
        var windowSize = ImGui.GetWindowSize();

        ImGui.SetWindowPos((displaySize / 2) - (windowSize / 2));


        ImGui.Text("Login");

        if (notValidUsername)
        {
            ImGui.PushStyleColor(ImGuiCol.FrameBg, 0xFF0000FF);
        }

        ImGui.InputText("Name", ref Username, 255);
        if (notValidUsername)
        {
            ImGui.PopStyleColor();
        }

        ImGui.Combo("PlayerType", ref SelectedType, PlayerTypes, PlayerTypes.Length);


        if (ImGui.Button("Login", new Vector2(-1, 0)))
        {
            if (notValidUsername)
            {
            }
            else
            {
                File.WriteAllText("username.txt", Username);
                LoggedIn = true;

                CreateNewEntity(new PlayerComponent()
                {
                    Username = Username,
                    PlayerType = PlayerType
                });

                var loginP = new LoginPacket();
                loginP.Username = Username;
                loginP.PlayerType = PlayerType;

                SendPacket(loginP);
            }
        }

        ImGui.End();
    }

    public static void TickUi()
    {
        if (!LoggedIn)
        {
            LoginUi();
            return;
        }

        ImGui.DockSpaceOverViewport();

        foreach (var (name, mouse) in _mouses)
        {
            if (name != Username)
            {
                var g = ImGui.GetForegroundDrawList();
                g.AddCircleFilled(new Vector2(mouse.X, mouse.Y), 2, 0xFFFFFFFF);
                g.AddText(new Vector2(mouse.X, mouse.Y), 0xFFFFFFFF, name);
            }
        }

        ImGui.Text($"Total Components: {Components.Count}");
        ImGui.Text($"Username: {Username}");
        ImGui.Text($"PlayerType: {PlayerType}");

        ImGui.Image((IntPtr)_texture.TextureId, new Vector2(100, 100));
        ImGui.ShowDemoWindow();


        foreach (var uiSystem in UiSystems)
        {
            var argCols = new List<List<Component>>();
            var paramss = uiSystem.Parameters;

            lock (_locker)
            {
                foreach (var info in paramss)
                {
                    // this is horific
                    var lst = Components
                        .Where(x => x.GetType() == info.ParameterType)
                        .ToList();

                    argCols.Add(lst);
                }

                var smallest = argCols.OrderBy(x => x.Count).ToList();

                for (var i = 0; i < argCols.Count; i++)
                {
                    if (argCols.Count > 0)
                    {
                        argCols[i] = argCols[i]
                            .Where(it => smallest[0].Any(x => x.Guid == it.Guid))
                            .ToList();
                    }

                    argCols[i] = argCols[i]
                        .OrderBy(x => x.Guid).ToList();
                }

                for (int i = 0; i < argCols[0].Count; i++)
                {
                    var o = new object[paramss.Length];
                    var hash = new string[paramss.Length];
                    for (int j = 0; j < paramss.Length; j++)
                    {
                        o[j] = argCols[j][i];
                        hash[j] = MD5HashGenerator.GenerateKey(o[j]);
                    }

                    uiSystem.Run(o);

                    for (var j = 0; j < o.Length; j++)
                    {
                        var o1 = o[j];
                        if (hash[j] != MD5HashGenerator.GenerateKey(o1))
                        {
                            var p = new UpdateOrCreateComponentPacket();
                            p.ComponentType = o1.GetType().ToString();
                            p.ComponentJson = JsonConvert.SerializeObject(o1);

                            SendPacket(p);
                        }
                    }
                }
            }
        }
    }

    private static BinaryReader br;
    private static BinaryWriter bw;
    private static Dictionary<string, MousePacket> _mouses { get; set; } = new();

    private static void NetworkThread()
    {
        var ns = _client.GetStream();
        br = new BinaryReader(ns);
        bw = new BinaryWriter(ns);

        while (true)
        {
            if (ns.DataAvailable)
            {
                var id = br.ReadInt32();

                var packet = (Packet)Activator.CreateInstance(_packets[id]);
                packet.Read(br);


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

                        break;
                    case DeleteComponentPacket dcp:
                        lock (Components)
                        {
                            var t = Type.GetType(dcp.Type);
                            Components.RemoveAll(x => x.GetType() == t && x.Guid == dcp.ComponentId);
                        }

                        break;
                    case MousePacket mousePacket:
                        if (!_mouses.ContainsKey(mousePacket.Username))
                        {
                            _mouses.Add(mousePacket.Username, mousePacket);
                        }
                        else
                        {
                            _mouses[mousePacket.Username] = mousePacket;
                        }

                        break;
                }
            }

            Thread.Sleep(25);
        }
    }

    private static MousePacket _mouse = new MousePacket();

    public static void Start()
    {
        var ip = Dns.GetHostEntry("mvtt.myvar.cloud").AddressList[0].ToString();
        _client = new TcpClient(ip, 6969);


        ThreadPool.QueueUserWorkItem((state) => { NetworkThread(); });
        ThreadPool.QueueUserWorkItem((state) =>
        {
            while (true)
            {
                if (_client.Connected && LoggedIn)
                    SendPacket(_mouse);
                Thread.Sleep(150);
            }
        });

        if (File.Exists("username.txt"))
        {
            Username = File.ReadAllText("username.txt");
        }

        var gw = new GameWindow();
        gw.Title = "Myvar Virtual Table Top";
        gw.TargetRenderFrequency = 120;
        gw.TargetUpdateFrequency = 120;

        gw.Load += (sender, args) =>
        {
            LoadAssets();
            ImGuiEngine.Install(gw);

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        };
        gw.RenderFrame += (sender, args) =>
        {
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            ImGuiEngine.RenderFrame(TickUi, gw);

            gw.SwapBuffers();
        };

        gw.MouseMove += (sender, args) =>
        {
            _mouse.X = args.X;
            _mouse.Y = args.Y;
            _mouse.Username = Username;
        };

        gw.Run();
    }
}