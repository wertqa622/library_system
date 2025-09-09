using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using library_management_system.DataBase;

namespace library_management_system.Repository
{
    public class LoanRepository : ILoanRepository
    {
        private readonly OracleDapperHelper _dbHelper;
        public LoanRepository(OracleDapperHelper dbHelper)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
        }
    }
}
