using System.Net;
using System.Net.Sockets;

namespace Mvtt.Core;

public static class NetworkFileSystem
{
    public static string Ip { get; set; } = "127.0.0.1";

    static NetworkFileSystem()
    {
        Ip = Dns.GetHostEntry("mvtt.myvar.cloud").AddressList[0].ToString();
    }
    public static int Port { get; set; } = 6970;
    public static int QueryPort { get; set; } = 6969;

    public static Stream OpenReadFile(string guild, string path)
    {
        var tcpClient = new TcpClient();
        tcpClient.Connect(Ip, Port);

        var ns = tcpClient.GetStream();
        var bw = new BinaryWriter(ns);
        var br = new BinaryReader(ns);

        bw.Write((byte)1);
        bw.Write(guild);
        bw.Write(path);
        ns.Flush();

        while (!ns.DataAvailable)
        {
            Thread.Sleep(5);
        }

        var code = br.ReadByte();
        if (code == 1)
        {
            throw new FileNotFoundException();
        }

        return ns;
    }
    public static Stream OpenWriteFile(string guild, string path)
    {
        var tcpClient = new TcpClient();
        tcpClient.Connect(Ip, Port);

        var ns = tcpClient.GetStream();
        var bw = new BinaryWriter(ns);

        bw.Write((byte)2);
        bw.Write(guild);
        bw.Write(path);

        return ns;
    }
    
    /*
 * File:
 *  Delete
 *  Move
 *  Rename
 * 
 * Directory:
 *  ListFiles
 *  ListDirectories
 *  Move
 *  Rename
 *  Delete
 *  Create
 */
}