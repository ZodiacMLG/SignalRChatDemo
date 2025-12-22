// ChatApp.Server/Hubs/ChatHub.cs
using Microsoft.AspNetCore.SignalR;
using ChatApp.Shared.Models;

namespace ChatApp.Server.Hubs
{
    public class ChatHub : Hub
    {

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", new ChatMessage
            {
                User = "System",
                Message = $"{Context.ConnectionId} joined room '{roomName}'",
                Timestamp = DateTime.Now
            });

            await Clients.Caller.SendAsync("ReceiveMessage", new ChatMessage
            {
                User = "System",
                Message = $"You joined room '{roomName}'",
                Timestamp = DateTime.Now
            });
        }
        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

            await Clients.Group(roomName).SendAsync("ReceiveMessage", new ChatMessage
            {
                User = "System",
                Message = $"{Context.ConnectionId} left room '{roomName}'",
                Timestamp = DateTime.Now
            });
        }

        // Изменяем метод: принимаем отдельно user и message
        public async Task SendMessage(string roomName, string user, string message)
        {
            // Создаем объект ChatMessage для отправки
            await Clients.Group(roomName).SendAsync("ReceiveMessage", new ChatMessage
            {
                User = user,
                Message = message,
                Room = roomName,
                Timestamp = DateTime.Now
            });
        }
    }
}