namespace SLIL.Classes.GameChat
{
    public class ChatMessage
    {
        public string Name { get; set; }
        public string Message { get; set; }

        public ChatMessage(string Name, string Message)
        {
            this.Name = Name;
            this.Message = Message;
        }
    }
}