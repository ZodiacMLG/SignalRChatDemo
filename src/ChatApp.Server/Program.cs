using ChatApp.Server.Hubs;

namespace ChatApp.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseCors("AllowAll");
            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}
