namespace Mvtt.Core.Packets;

public abstract class Packet
{
    public abstract int Id { get; }

    public abstract void Read(BinaryReader br);
    public abstract void Write(BinaryWriter bw);
}

