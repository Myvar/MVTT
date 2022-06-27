using ImGuiNET;
using Mvtt.Core.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Vector2 = System.Numerics.Vector2;

namespace Mvtt.Core.UI;

public static unsafe class ImGuiEngine
{
    //im way to upset atm to be doing code rewie but wtf this code is all over the palse
    public static void Skin()
    {
        /*var style = ImGuiNET.ImGui.GetStyle();
        var dark = new Vector4(Theam.Dark.R / 255f, Theam.Dark.G / 255f, Theam.Dark.B / 255f, 1);
        var veryDark = new Vector4(Theam.VeryDark.R / 255f, Theam.VeryDark.G / 255f, Theam.VeryDark.B / 255f, 1);
        var light = new Vector4(Theam.Light.R / 255f, Theam.Light.G / 255f, Theam.Light.B / 255f, 1);
        var veryLight = new Vector4(Theam.VeryLight.R / 255f, Theam.VeryLight.G / 255f, Theam.VeryLight.B / 255f,
            1);
        var meduim = new Vector4(Theam.Medium.R / 255f, Theam.Medium.G / 255f, Theam.Medium.B / 255f, 1);
        style.WindowRounding = 2;
        style.Colors[(int) ImGuiCol.Text] = new Vector4(1.00f, 0.36f, 0.95f, 1.00f);
        style.Colors[(int) ImGuiCol.FrameBg] = new Vector4(0.22f, 0.27f, 0.40f, 1.00f);
        style.Colors[(int) ImGuiCol.TitleBg] = new Vector4(0.22f, 0.27f, 0.40f, 1.00f);
        style.Colors[(int) ImGuiCol.Button] = new Vector4(0.56f, 0.80f, 0.83f, 1.00f);
        style.Colors[(int) ImGuiCol.Tab] = new Vector4(0.56f, 0.80f, 0.83f, 1.00f);
        style.Colors[(int) ImGuiCol.TabHovered] = new Vector4(0.49f, 0.70f, 0.72f, 1.00f);
        style.Colors[(int) ImGuiCol.TabActive] = new Vector4(0.30f, 0.93f, 1.00f, 1.00f);*/
    }

    private static System.Numerics.Vector4 RGB(int r, int g, int b)
    {
        return new System.Numerics.Vector4(r / 255f, g / 255f, b / 255f, 255 / 255f);
    }

    private static System.Numerics.Vector4 RGBA(int r, int g, int b, int a)
    {
        return new System.Numerics.Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static void SkinEnd()
    {
        // ImGui.PopStyleColor(4);
    }

    private static int s_fontTexture;
    private static float _wheelPosition;
    private static float _scaleFactor;
    private static float _scaleMultiplyer = 1.0f;
    private static Vec2 _mousePos = new(0, 0);

    public static unsafe void Install(GameWindow Window)
    {
        var context = ImGuiNET.ImGui.CreateContext();
        ImGuiNET.ImGui.SetCurrentContext(context);

        SetOpenTKKeyMappings();
        CreateDeviceObjects();

        Window.KeyDown += OnKeyDown;
        Window.KeyUp += OnKeyUp;
        Window.KeyPress += OnKeyPress;

        Window.MouseMove += (sender, args) => _mousePos = new Vec2(args.X, args.Y);

        int desiredWidth = Window.Height;
        _scaleFactor = Window.Width / desiredWidth;

        _scaleFactor *= _scaleMultiplyer;


        // GameEngine.Window.Context.LoadAll();


        //ImGuiNative.igCreateContext();

        ImGui.GetIO().Fonts.AddFontDefault();


        // InstallViewports();


        Skin();
    }

    public struct InternalViewPort
    {
        public int Index { get; set; }
    }


    private static unsafe void SetOpenTKKeyMappings()
    {
        var io = ImGuiNET.ImGui.GetIO();
        //    io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        //  io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

        // io.BackendFlags |= ImGuiBackendFlags.PlatformHasViewports;
        // io.BackendFlags |= ImGuiBackendFlags.RendererHasViewports;

        // io.ConfigDockingWithShift = true;
        // io.BackendFlags |= ImGuiBackendFlags.HasMouseHoveredViewport;


        io.KeyMap[(int)ImGuiKey.Tab] = (int)Key.Tab;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)Key.PageDown;
        io.KeyMap[(int)ImGuiKey.Home] = (int)Key.Home;
        io.KeyMap[(int)ImGuiKey.End] = (int)Key.End;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)Key.Delete;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.BackSpace;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)Key.Enter;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)Key.Escape;
        io.KeyMap[(int)ImGuiKey.A] = (int)Key.A;
        io.KeyMap[(int)ImGuiKey.C] = (int)Key.C;
        io.KeyMap[(int)ImGuiKey.V] = (int)Key.V;
        io.KeyMap[(int)ImGuiKey.X] = (int)Key.X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)Key.Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)Key.Z;
    }

    private static void OnKeyPress(object sender, KeyPressEventArgs e)
    {
        ImGuiNET.ImGui.GetIO().AddInputCharacter(e.KeyChar);
    }

    private static void OnKeyDown(object sender, KeyboardKeyEventArgs e)
    {
        ImGuiNET.ImGui.GetIO().KeysDown[(int)e.Key] = true;
        UpdateModifiers(e);
    }


    private static unsafe void UpdateModifiers(KeyboardKeyEventArgs e)
    {
        var io = ImGuiNET.ImGui.GetIO();
        io.KeyAlt = e.Alt;
        io.KeyCtrl = e.Control;
        io.KeyShift = e.Shift;
    }

    private static void OnKeyUp(object sender, KeyboardKeyEventArgs e)
    {
        ImGuiNET.ImGui.GetIO().KeysDown[(int)e.Key] = false;
        UpdateModifiers(e);
    }

    private static unsafe void CreateDeviceObjects()
    {
        var io = ImGuiNET.ImGui.GetIO();

        io.Fonts.AddFontFromFileTTF("./GameData/Fonts/FiraCode-Retina.ttf", 18);


        byte* data = null;

        var width = 0;
        var height = 0;

        // Build texture atlas
        io.Fonts.GetTexDataAsRGBA32(out data, out width, out height);

        // Create OpenGL texture
        s_fontTexture = GL.GenTexture();
        /*GL.BindTexture(TextureTarget.Texture2D, s_fontTexture);
        GL.TexParameter(TextureTarget.Texture2D,
            TextureParameterName.TextureMinFilter, (int) All.Linear);
        GL.TexParameter(TextureTarget.Texture2D,
            TextureParameterName.TextureMagFilter, (int) All.Linear);
        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            texData.Width * texData.Width,
            texData.Height* texData.Width,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedInt,
            new IntPtr(texData.Pixels));*/


        GL.BindTexture(TextureTarget.Texture2D, s_fontTexture);

        GL.TexImage2D(TextureTarget.Texture2D, 0,
            PixelInternalFormat.Rgba, width, height, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr(data));

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Linear);

        // Store the texture identifier in the ImFontAtlas substructure.
        io.Fonts.SetTexID(new IntPtr(s_fontTexture));

        // Cleanup (don't clear the input data if you want to append new fonts later)
        //io.Fonts->ClearInputData();

        io.Fonts.ClearTexData();

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public static unsafe void RenderFrame(Action e, GameWindow Window)
    {
        // Texture.UnBind();

        var io = ImGuiNET.ImGui.GetIO();
        io.DisplaySize = new System.Numerics.Vector2(Window.Width, Window.Height);
        io.DisplayFramebufferScale = new System.Numerics.Vector2(_scaleFactor);
        //io.DisplayFramebufferScale = new System.Numerics.Vector2(1);
        io.DeltaTime = (1f / (float)Window.RenderFrequency);

        UpdateImGuiInput(io);

        ImGuiNET.ImGui.NewFrame();

        //  Skin();

        e();

        //  ImGui.PopStyleColor(27);

        ImGuiNET.ImGui.Render();


        // ImGui.UpdatePlatformWindows();
        // ImGui.RenderPlatformWindowsDefault();

        RenderImDrawData(ImGuiNET.ImGui.GetDrawData(), Window);
    }


    private static unsafe void UpdateImGuiInput(ImGuiIOPtr io)
    {
        MouseState cursorState = Mouse.GetCursorState();
        MouseState mouseState = Mouse.GetState();


        var windowPoint = new System.Numerics.Vector2(_mousePos.X, _mousePos.Y) /
                          new Vector2(_scaleMultiplyer);
        io.MousePos =
            windowPoint; //new System.Numerics.Vector2(windowPoint.X / io.DisplayFramebufferScale.X, windowPoint.Y / io.DisplayFramebufferScale.Y);


        io.MouseDown[0] = mouseState.LeftButton == ButtonState.Pressed;
        io.MouseDown[1] = mouseState.RightButton == ButtonState.Pressed;
        io.MouseDown[2] = mouseState.MiddleButton == ButtonState.Pressed;

        float newWheelPos = mouseState.WheelPrecise;
        float delta = newWheelPos - _wheelPosition;
        _wheelPosition = newWheelPos;
        io.MouseWheel = delta;
    }

    private static unsafe void RenderImDrawData(ImDrawDataPtr draw_data, GameWindow Window)
    {
        // Rendering
        int display_w, display_h;
        display_w = Window.Width;
        display_h = Window.Height;


        GL.Viewport(0, 0, display_w, display_h);

        GL.ActiveTexture(0);
        GL.UseProgram(0);
        //  Texture.UnBind();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        int last_texture;
        GL.GetInteger(GetPName.TextureBinding2D, out last_texture);
        GL.PushAttrib(AttribMask.EnableBit | AttribMask.ColorBufferBit | AttribMask.TransformBit);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.ScissorTest);
        GL.EnableClientState(ArrayCap.VertexArray);
        GL.EnableClientState(ArrayCap.TextureCoordArray);
        GL.EnableClientState(ArrayCap.ColorArray);
        GL.Enable(EnableCap.Texture2D);


        // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
        var io = ImGuiNET.ImGui.GetIO();
        // ImGui.ScaleClipRects(draw_data, io.DisplayFramebufferScale);

        draw_data.ScaleClipRects(ImGuiNET.ImGui.GetIO().DisplayFramebufferScale);

        // Setup orthographic projection matrix
        GL.MatrixMode(MatrixMode.Projection);
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.Ortho(
            0.0f,
            io.DisplaySize.X / io.DisplayFramebufferScale.X,
            io.DisplaySize.Y / io.DisplayFramebufferScale.Y,
            0.0f,
            -1.0f,
            1.0f);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();
        GL.LoadIdentity();

        // Render command lists

        for (int n = 0; n < draw_data.CmdListsCount; n++)
        {
            var cmd_list = ((ImDrawListPtr*)draw_data.CmdLists)[n];
            var vtx_buffer = cmd_list.VtxBuffer.Data;
            var idx_buffer = (ushort*)cmd_list.IdxBuffer.Data;

            var vert0 = *((ImDrawVert*)vtx_buffer);
            var vert1 = *(((ImDrawVert*)vtx_buffer) + 1);
            var vert2 = *(((ImDrawVert*)vtx_buffer) + 2);

            var PosOffset = 0;
            var UVOffset = 8;
            var ColOffset = 16;

            GL.VertexPointer(2, VertexPointerType.Float, sizeof(ImDrawVert),
                vtx_buffer);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(ImDrawVert),
                new IntPtr(vtx_buffer.ToInt64() + UVOffset));
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(ImDrawVert),
                new IntPtr(vtx_buffer.ToInt64() + ColOffset));

            for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
            {
                var pcmd = &(((ImDrawCmd*)cmd_list.CmdBuffer.Data)[cmd_i]);
                if (pcmd->UserCallback != IntPtr.Zero)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, pcmd->TextureId.ToInt32());
                    GL.Scissor(
                        (int)pcmd->ClipRect.X,
                        (int)(io.DisplaySize.Y - pcmd->ClipRect.W),
                        (int)(pcmd->ClipRect.Z - pcmd->ClipRect.X),
                        (int)(pcmd->ClipRect.W - pcmd->ClipRect.Y));
                    ushort[] indices = new ushort[pcmd->ElemCount];
                    for (int i = 0; i < indices.Length; i++)
                    {
                        indices[i] = idx_buffer[i];
                    }

                    GL.DrawElements(PrimitiveType.Triangles, (int)pcmd->ElemCount, DrawElementsType.UnsignedShort,
                        new IntPtr(idx_buffer));
                }

                idx_buffer += pcmd->ElemCount;
            }
        }

        // Restore modified state
        GL.DisableClientState(ArrayCap.ColorArray);
        GL.DisableClientState(ArrayCap.TextureCoordArray);
        GL.DisableClientState(ArrayCap.VertexArray);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PopMatrix();
        GL.MatrixMode(MatrixMode.Projection);
        GL.PopMatrix();
        GL.PopAttrib();
    }
}