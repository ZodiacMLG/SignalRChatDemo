namespace ChatApp.Shared.Models
{
    public class ChatMessage
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }
        public string Room {  get; set; }
    }
}
