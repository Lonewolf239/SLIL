using LiteNetLib.Utils;

namespace GameServer
{
    internal struct Packet : INetSerializable
    {
        public int PacketID { get; set; }
        public byte[] Data { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            PacketID = reader.GetInt();
            Data = reader.GetRemainingBytes();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PacketID);
            writer.Put(Data);
        }
    }
}