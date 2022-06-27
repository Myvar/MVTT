namespace Mvtt.Core.Packets;

public class LoginPacket : Packet
{
    public override int Id => 0x2;

    public string Username { get; set; }
    public string PlayerType { get; set; }

    public override void Read(BinaryReader br)
    {
        Username = br.ReadString();
        PlayerType = br.ReadString();
    }

    public override void Write(BinaryWriter bw)
    {
        bw.Write(Username);
        bw.Write(PlayerType);
    }
}