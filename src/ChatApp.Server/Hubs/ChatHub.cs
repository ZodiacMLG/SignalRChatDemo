// ChatApp.Server/Hubs/ChatHub.cs
using Microsoft.AspNetCore.SignalR;
using ChatApp.Shared.Models;

namespace ChatApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, string> _userNames = new();


        // Регистрация пользователя
        public void RegisterUser(string userName)
        {
            _userNames[Context.ConnectionId] = userName;
        }


        public async Task JoinRoom(string roomName)
        {
            var userName = _userNames.GetValueOrDefault(Context.ConnectionId, "Anonymous");

            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            await Clients.Group(roomName).SendAsync("ReceiveMessage", new ChatMessage
            {
                User = "System",
                Message = $"{userName} joined room '{roomName}'",
                Room = roomName,
                Timestamp = DateTime.Now
            });

            await Clients.Caller.SendAsync("ReceiveMessage", new ChatMessage
            {
                User = "System",
                Message = $"Welcome to '{roomName}', {userName}!\n",
                Room = roomName,
                Timestamp = DateTime.Now
            });
        }

        public async Task LeaveRoom(string roomName)
        {
            var userName = _userNames.GetValueOrDefault(Context.ConnectionId, "Anonymous");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

            await Clients.Group(roomName).SendAsync("ReceiveMessage", new ChatMessage
            {
                User = "System",
                Message = $"{userName} left room '{roomName}'\n",
                Room = roomName,
                Timestamp = DateTime.Now
            });
        }

        // Очистка при отключении
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _userNames.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
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