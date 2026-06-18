using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using ChatApp.Shared.Models;

namespace DesktopChat
{
    
    public partial class MainWindow : Window
    {
        static string currentRoom = "General";  // current room
        const string urlString = "http://localhost:5291/chathub";
        string Name = string.Empty;
        VerificationWindow verificationWindow = new VerificationWindow();

        HubConnection connection;
        public MainWindow()
        {
            InitializeComponent();

            

            connection = new HubConnectionBuilder()
                .WithUrl(urlString)
                .Build();

            // Подписываемся на принятие сообщений
            connection.On<ChatMessage>("ReceiveMessage", (message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (message.Room == currentRoom || message.User == "System")
                    {
                        var newMessage = $"[{message.Timestamp:HH:mm} | {message.Room}] {message.User}: {message.Message}";
                        chatbox.Items.Insert(0, newMessage);
                    }
                });
            });
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Запускается окно для ввода никнейма
                if (verificationWindow.ShowDialog() == true)
                {
                    Name = verificationWindow.Name;
                }
                await connection.StartAsync();
                chatbox.Items.Add("✅ Connected!");
                sendBtn.IsEnabled = true;
                await connection.SendAsync("JoinRoom", currentRoom);
            }
            catch (Exception ex)
            {
                chatbox.Items.Add(ex.Message);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Отправка сообщений, передаём: комнату, имя пользователя, текст сообщения
                await connection.SendAsync("SendMessage", currentRoom, Name, messageTextBox.Text);
                messageTextBox.Clear();
            }
            catch (Exception ex)
            {
                chatbox.Items.Add(ex.Message);
            }
        }
    }
}