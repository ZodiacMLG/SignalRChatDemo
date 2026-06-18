// ChatApp.Client/Program.cs
using Microsoft.AspNetCore.SignalR.Client;
using ChatApp.Shared.Models;

class Program
{
    static string currentRoom = "General";  // current room
    static string userName = "";
    static string currentInput = "";        // Текущий ввод пользователя
    static object consoleLock = new object();

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
                lock (consoleLock)
                {
                    int currentLeft = Console.CursorLeft;
                    Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                    Console.WriteLine($"[{message.Timestamp:HH:mm} | {message.Room}] {message.User}: {message.Message}");
                    Console.Write($"{userName}@{currentRoom}> {currentInput}");
                }
                //Console.WriteLine($"[{message.Timestamp:HH:mm} | {message.Room}] {message.User}: {message.Message}");
            }
        });

        try
        {
            await connection.StartAsync();
            Console.WriteLine("✅ Connected!");

            // Сначала запросим имя пользователя
            Console.Write("Enter your username: ");
            var username = Console.ReadLine();

            Console.WriteLine("Commands:");
            Console.WriteLine("  /join <room> - join room");
            Console.WriteLine("  /leave <room> - leave room");
            Console.WriteLine("  /rooms - list rooms");
            Console.WriteLine("  /exit - quit");
            Console.WriteLine("-----------------------");

            await connection.SendAsync("RegisterUser", username);
            await connection.InvokeAsync("JoinRoom", currentRoom);

            await Task.Delay(100);

            while (true)
            {
                Console.Write($"{username}@{currentRoom}> ");
                var input = ReadLineWithTracking();

                if (string.IsNullOrEmpty(input)) continue;

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

    static string ReadLineWithTracking()
    {
        currentInput = "";

        while (true) {
            var key = Console.ReadKey(intercept: true);

            lock (consoleLock)
            {
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    var result = currentInput;
                    currentInput = "";
                    return result;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (currentInput.Length > 0 )
                    {
                        currentInput = currentInput.Substring(0, currentInput.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    currentInput += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }
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
                    await Task.Delay(100);
                }
                break;

            case "/leave":
                await connection.SendAsync("LeaveRoom", currentRoom);
                currentRoom = "General";
                await connection.SendAsync("JoinRoom", currentRoom);
                await Task.Delay(100);
                break;

            case "/exit":
                await connection.SendAsync("LeaveRoom", currentRoom);
                await connection.DisposeAsync();
                Environment.Exit(0);
                break;
        }
    }
}