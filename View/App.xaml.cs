using System.IO;
using System.Windows;
using library_management_system.View;
using library_management_system.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Application = System.Windows.Application;

namespace library_management_system
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                   


                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await AppHost!.StartAsync();

                var logger = AppHost.Services.GetRequiredService<ILogger<App>>();

               

                base.OnStartup(e);

                var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"예외: {ex.Message}");
                throw;
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            var logger = AppHost!.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("시스템 종료");

            await AppHost!.StopAsync();
            base.OnExit(e);
        }

    }
}