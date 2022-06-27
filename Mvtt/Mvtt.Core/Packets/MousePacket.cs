namespace Mvtt.Core.Packets;

public class MousePacket : Packet
{
    public override int Id => 0x4;

    public int X { get; set; }
    public int Y { get; set; }
    public string Username { get; set; }

    public override void Read(BinaryReader br)
    {
        X = br.ReadInt32();
        Y = br.ReadInt32();
        Username = br.ReadString();
    }

    public override void Write(BinaryWriter bw)
    {
        bw.Write(X);
        bw.Write(Y);
        bw.Write(Username);
    }
}