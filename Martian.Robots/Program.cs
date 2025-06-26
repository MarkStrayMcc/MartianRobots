using Martian.Robots.Core;
using Martian.Robots.Core.Abstractions;
using Martian.Robots.Core.Commands;
using Martian.Robots.Core.Services;
using Martian.Robots.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Martian.Robots
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Configure dependency injection
                var serviceProvider = ConfigureServices();

                // Run the application
                var app = serviceProvider.GetRequiredService<Application>();
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register Core Components
            services.AddSingleton<IWorldFactory, WorldFactory>();
            services.AddSingleton<ICommandRegistry, CommandRegistry>();

            // Register Commands
            RegisterAllCommands(services);

            // Register Application
            services.AddSingleton<Application>();

            return services.BuildServiceProvider();
        }

        private static void RegisterAllCommands(IServiceCollection services)
        {
            // Default Commands
            services.AddSingleton<ICommand, TurnLeftCommand>();
            services.AddSingleton<ICommand, TurnRightCommand>();
            services.AddSingleton<ICommand, MoveForwardCommand>();

            // Extended Commands (Add your custom ones here)
            // services.AddSingleton<ICommand, MoveBackwardCommand>();
            // services.AddSingleton<ICommand, TurboBoostCommand>();
        }
    }
}