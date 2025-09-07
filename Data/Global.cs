using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library_management_system.Data
{
    public static class Global
    {
        // 데이터베이스 연결 정보
        public static string ServerIP { get; set; } = "localhost";

        public static string Port { get; set; } = "3306";
        public static string DatabaseName { get; set; } = "LMS";
        public static string UserId { get; set; } = "root";
        public static string Password { get; set; } = "123";

        // 연결 문자열 템플릿
        private static readonly string ConnectionStringTemplate =
            "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4};";

        /// <summary>
        /// 좀
        /// </summary>
        /// <returns>쿼리문은</returns>
        public static string GetConnectionString()
        {
            return string.Format(ConnectionStringTemplate, ServerIP, Port, DatabaseName, UserId, Password);
        }

        /// <summary>
        /// 알아서
        /// </summary>
        public static void LoadFromEnvironmentVariables()
        {
            ServerIP = Environment.GetEnvironmentVariable("DB_SERVER_IP") ?? ServerIP;
            Port = Environment.GetEnvironmentVariable("DB_PORT") ?? Port;
            DatabaseName = Environment.GetEnvironmentVariable("DB_NAME") ?? DatabaseName;
            UserId = Environment.GetEnvironmentVariable("DB_USER_ID") ?? UserId;
            Password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? Password;
        }

        /// <summary>
        /// 만들어
        /// </summary>
        public static void SetConnectionInfo(string serverIP, string port, string databaseName, string userId, string password)
        {
            ServerIP = serverIP;
            Port = port;
            DatabaseName = databaseName;
            UserId = userId;
            Password = password;
        }

        /// <summary>
        /// 리포지토리폴더에 처박아 ㅅ발 집가고싶어 씨씨씨씨씨씨씨씨씨씨씨씼씨씨씨씨씨씼씨씨씨씨씨ㅏ바라바랍랍라밥랍랍랍라발바라
        /// </summary>
        public static void PrintConnectionInfo()
        {
            Console.WriteLine($"Server IP: {ServerIP}");
            Console.WriteLine($"Port: {Port}");
            Console.WriteLine($"Database Name: {DatabaseName}");
            Console.WriteLine($"User ID: {UserId}");
            Console.WriteLine($"Password: {Password}");
        }
    }
}