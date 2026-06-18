// ChatApp.Client/Program.cs
using Microsoft.AspNetCore.SignalR.Client;
using ChatApp.Domain.Models;

class Program
{
    static string currentRoom = "General";  // current room
    static async Task Main(string[] args)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5291/chathub")
            .Build();

        // Подписываемся на получение сообщений
        connection.On<ChatMessage>("ReceiveMessage", (message) =>
        {
            if (message.Room == currentRoom || message.User == "System")
            {
                Console.WriteLine($"[{message.Timestamp:HH:mm} | {message.Room}] {message.User}: {message.Message}");
            }
        });

        try
        {
            await connection.StartAsync();
            Console.WriteLine("✅ Connected!");
            Console.Write("Enter your username: ");
            var username = Console.ReadLine();
            await connection.SendAsync("JoinRoom", currentRoom);

            Console.WriteLine("Commands:");
            Console.WriteLine("  /join <room> - join room");
            Console.WriteLine("  /leave <room> - leave room");
            Console.WriteLine("  /rooms - list rooms");
            Console.WriteLine("  /exit - quit");
            Console.WriteLine("-----------------------");

            // Сначала запросим имя пользователя
            

            while (true)
            {
                Console.Write($"{username}@{currentRoom}> ");
                var input = Console.ReadLine();

                if (input.StartsWith("/"))
                {
                    await HandleCommand(connection, username, input);
                }
                else if (!string.IsNullOrWhiteSpace(input))
                {
                    // Отправляем два параметра: username и message
                    await connection.SendAsync("SendMessage", currentRoom, username, input);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }

    static async Task HandleCommand(HubConnection connection, string username, string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        switch (parts[0].ToLower())
        {
            case "/join":
                if (parts.Length > 1)
                {
                    var newRoom = parts[1];
                    await connection.SendAsync("LeaveRoom", currentRoom);
                    await connection.SendAsync("JoinRoom", newRoom);
                    currentRoom = newRoom;
                    Console.WriteLine($"Joined room: {newRoom}\n");
                }
                break;

            case "/leave":
                await connection.SendAsync("LeaveRoom", currentRoom);
                currentRoom = "General";
                await connection.SendAsync("JoinRoom", currentRoom);
                break;

            case "/exit":
                await connection.SendAsync("LeaveRoom", currentRoom);
                await connection.DisposeAsync();
                Environment.Exit(0);
                break;
        }
    }
}