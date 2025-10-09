using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library_management_system.Data
{
    public static class Global
    {
        public static string ServerIP = "deu.duraka.shop";
        public static string Port = "4261";
        public static string DatabaseName = "xe";
        public static string UserId = "sys";
        public static string Password = "mCYxbK01o";
       
        private static readonly string ConnectionStringTemplate =
            "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4}; DBA Privilege=SYSDBA;";

       
        public static string GetConnectionString()
        {
            return string.Format(ConnectionStringTemplate, ServerIP, Port, DatabaseName, UserId, Password);
        }     
    }
}