// ChatApp.Server/Hubs/ChatHub.cs
using Microsoft.AspNetCore.SignalR;
using ChatApp.Shared.Models;

namespace ChatApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        // Изменяем метод: принимаем отдельно user и message
        public async Task SendMessage(string user, string message)
        {
            // Создаем объект ChatMessage для отправки
            var chatMessage = new ChatMessage
            {
                User = user,
                Message = message,
                Timestamp = DateTime.Now
            };

            await Clients.All.SendAsync("ReceiveMessage", chatMessage);
        }
    }
}