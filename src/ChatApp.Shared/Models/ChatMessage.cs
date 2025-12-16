// ChatApp.Shared/Models/ChatMessage.cs
using System;

namespace ChatApp.Shared.Models
{
    public class ChatMessage
    {
        // Убедись, что есть это свойство!
        public string User { get; set; }           // ← должно быть User
        public string Message { get; set; }        // ← должно быть Message
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Room { get; set; } = "General";
    }
}