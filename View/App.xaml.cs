using System.IO;
using System.Windows;
using library_management_system.DataBase;
using library_management_system.View;
using library_management_system.ViewModels;
using library_management_system.Repository;
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
                    services.AddScoped<OracleDapperHelper>(provider =>
                    {
                        var logger = provider.GetRequiredService<ILogger<OracleDapperHelper>>();
                        string connectionString = Data.Global.GetConnectionString();
                        return new OracleDapperHelper(connectionString, logger);
                    });

                    services.AddScoped<IUnitOfWork, UnitOfWork>();

                    services.AddScoped<IBookRepository, BookRepository>();
                    services.AddScoped<IMemberRepository, MemberRepository>();
                    services.AddScoped<ILoanRepository, LoanRepository>();

                    services.AddTransient<MainViewModel>();

                    services.AddTransient<MainWindow>();

                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.AddConsole();
                    });
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Noto Sans KR 폰트 로드
                LoadCustomFont();
                
                await AppHost!.StartAsync();

                var logger = AppHost.Services.GetRequiredService<ILogger<App>>();

                var dbTestResult = await TestDatabaseConnection();
                if (dbTestResult)
                {
                    logger.LogInformation("DB 접속 성공");
                }
                else
                {
                    logger.LogError("DB 접속 실패");
                }

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

        private void LoadCustomFont()
        {
            try
            {
                // Pack URI로 폰트를 로드
                var fontUri = new Uri("pack://application:,,,/Fonts/NotoSansKR-Regular.ttf");
                
                System.Diagnostics.Debug.WriteLine("Noto Sans KR 폰트 로드 성공");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"폰트 로드 실패: {ex.Message}");
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            var logger = AppHost!.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("시스템 종료");

            await AppHost!.StopAsync();
            base.OnExit(e);
        }

        public static async Task<bool> TestDatabaseConnection()
        {
            try
            {
                using var scope = AppHost!.Services.CreateScope();
                var helper = scope.ServiceProvider.GetRequiredService<OracleDapperHelper>();
                var result = await helper.QuerySingleAsync<int>("SELECT 1 FROM DUAL");
                return result == 1;
            }
            catch (Exception ex)
            {
                var logger = AppHost!.Services.GetRequiredService<ILogger<App>>();
                logger.LogError(ex, "Database connection test failed");
                return false;
            }
        }
    }
}