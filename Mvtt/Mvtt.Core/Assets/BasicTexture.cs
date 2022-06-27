using System.Drawing;
using System.Net.Sockets;
using OpenTK.Graphics.OpenGL;

namespace Mvtt.Core.Assets
{
    public class BasicTexture
    {
        public int TextureId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public BasicTexture(string guild, string path)
        {
            using var fs = NetworkFileSystem.OpenReadFile(guild, path);

           // using var mem = new MemoryStream();
          //  fs.CopyTo(mem);

            using (var image = (Bitmap)Image.FromStream(fs))
            {
                var data = new List<byte>();

                Width = image.Width;
                Height = image.Height;

                for (var y = 0; y < Height; y++)
                {
                    for (var x = 0; x < Width; x++)
                    {
                        var px = image.GetPixel(x, y);
                        data.Add(px.R);
                        data.Add(px.G);
                        data.Add(px.B);
                        data.Add(px.A);
                    }
                }


                GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

                TextureId = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2D, TextureId);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, data.ToArray());

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -1);
            }
        }

        public void Dispose()
        {
            GL.DeleteTexture(TextureId);
        }

        public void Apply(int samplerSlot)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + samplerSlot);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
        }
    }
}