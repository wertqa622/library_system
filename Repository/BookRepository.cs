using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using library_management_system.DataBase;

namespace library_management_system.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly OracleDapperHelper _dbHelper;

        public BookRepository(OracleDapperHelper dbHelper)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
        }
    }
}
