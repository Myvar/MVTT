namespace Mvtt.Core.Packets;

public class DeleteComponentPacket : Packet
{
    public override int Id => 0x3;

    public string Type { get; set; }
    public Guid ComponentId { get; set; }

    public override void Read(BinaryReader br)
    {
        Type = br.ReadString();
        ComponentId = Guid.Parse(br.ReadString());
    }

    public override void Write(BinaryWriter bw)
    {
        bw.Write(Type);
        bw.Write(ComponentId.ToString());
    }
}