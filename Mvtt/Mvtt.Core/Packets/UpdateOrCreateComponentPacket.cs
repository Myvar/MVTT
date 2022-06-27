namespace Mvtt.Core.Packets;

public class UpdateOrCreateComponentPacket : Packet
{
    public override int Id => 0x1;

    public string ComponentType { get; set; }

    //todo replace with binary
    public string ComponentJson { get; set; }

    public override void Read(BinaryReader br)
    {
        ComponentType = br.ReadString();
        ComponentJson = br.ReadString();
    }

    public override void Write(BinaryWriter bw)
    {
        bw.Write(ComponentType);
        bw.Write(ComponentJson);
    }
}