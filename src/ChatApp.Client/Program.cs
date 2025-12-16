// ChatApp.Client/Program.cs
using Microsoft.AspNetCore.SignalR.Client;
using ChatApp.Shared.Models;

class Program
{
    static async Task Main(string[] args)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5291/chathub")
            .Build();

        // Подписываемся на получение сообщений
        connection.On<ChatMessage>("ReceiveMessage", (message) =>
        {
            Console.WriteLine($"[{message.Timestamp:HH:mm}] {message.User}: {message.Message}");
        });

        try
        {
            await connection.StartAsync();
            Console.WriteLine("✅ Connected to chat hub!");
            Console.WriteLine("Type 'exit' to quit");
            Console.WriteLine("-----------------------");

            // Сначала запросим имя пользователя
            Console.Write("Enter your username: ");
            var username = Console.ReadLine();

            while (true)
            {
                Console.Write($"{username}> ");
                var input = Console.ReadLine();

                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (!string.IsNullOrWhiteSpace(input))
                {
                    // Отправляем два параметра: username и message
                    await connection.SendAsync("SendMessage", username, input);
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
}