using System.IO;
using System.Windows;
using library_management_system.DataBase;
using library_management_system.Services;
using library_management_system.View;
using library_management_system.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.Logging;
using Application = System.Windows.Application;

namespace library_management_system
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            // 호스트 빌더 생성 및 DI, 로깅, 설정 구성
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Dapper Helper 등록 (Scoped)
                    services.AddScoped<OracleDapperHelper>(provider =>
                    {
                        var config = provider.GetRequiredService<IConfiguration>();
                        var logger = provider.GetRequiredService<ILogger<OracleDapperHelper>>();
                        string connectionString = GetConnectionStringFromConfig(config);
                        return new OracleDapperHelper(connectionString, logger);
                    });

                    // Repository 등록
                    services.AddScoped<BookRepository>();
                    //services.AddScoped<ILoanRepository, LoanRepository>();
                    //services.AddScoped<IMemberRepository, MemberRepository>();

                    // Service 등록(비즈니스 로직 계층)
                    services.AddScoped<IBookService, BookService>();
                    services.AddScoped<ILoanService, LoanService>();
                    services.AddScoped<IMemberService, MemberService>();

                    // 메모리 캐시 추가
                    services.AddMemoryCache();

                    // 최적화된 서비스 등록
                    services.AddScoped<OptimizedBookService>();

                    // ViewModel 등록
                    //services.AddTransient<MainViewModel>();
                    //services.AddTransient<BookViewModel>();
                    //services.AddTransient<MemberViewModel>();
                    //services.AddTransient<LoanViewModel>();

                    // View 등록
                    services.AddTransient<MainWindow>();
                    //services.AddTransient<BookView>();
                    //services.AddTransient<MemberView>();
                    //services.AddTransient<LoanView>();

                    // 로깅(Serilog)
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        //loggingBuilder.AddSerilog(dispose: true);
                    });
                })

                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Global 클래스에서 환경 변수 로드
                Data.Global.LoadFromEnvironmentVariables();

                await AppHost!.StartAsync();

                var logger = AppHost.Services.GetRequiredService<ILogger<App>>();
                logger.LogInformation("Library Management System Starting...");

                // 데이터베이스 연결 테스트
                var dbTestResult = await TestDatabaseConnection();
                if (dbTestResult)
                {
                    logger.LogInformation("Database connection test successful");
                }
                else
                {
                    logger.LogWarning("Database connection test failed - using in-memory data");
                }

                base.OnStartup(e);

                // DI로 MainWindow 생성 및 표시
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
            logger.LogInformation("시스템종료");

            await AppHost!.StopAsync();
            //Log.CloseAndFlush();
            base.OnExit(e);
        }

        private static string GetConnectionStringFromConfig(IConfiguration config)
        {
            // Global 클래스에서 연결 정보를 가져옴
            return Data.Global.GetConnectionString();
        }

        public static async Task<bool> TestDatabaseConnection()
        {
            try
            {
                using var helper = AppHost!.Services.GetRequiredService<OracleDapperHelper>();
                var result = helper.QuerySingle<int>("SELECT 1 FROM DUAL");
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