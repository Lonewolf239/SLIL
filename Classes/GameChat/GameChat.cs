using LiteNetLib.Utils;
using System.Collections.Generic;

namespace SLIL.Classes.GameChat
{
    public class GameChat : INetSerializable
    {
        private readonly List<ChatMessage> ChatMessages;

        public GameChat()
        {
            ChatMessages = new List<ChatMessage>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ChatMessages.Count);
            foreach (ChatMessage message in ChatMessages)
            {
                writer.Put(message.Name);
                writer.Put(message.Message);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            int messagesCount = reader.GetInt();
            List<ChatMessage> newMessages = new List<ChatMessage>();
            for (int i = 0; i < messagesCount; i++)
            {
                newMessages.Add(new ChatMessage(reader.GetString(), reader.GetString()));
            }
        }

    }
}