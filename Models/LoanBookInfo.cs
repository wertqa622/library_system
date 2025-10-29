using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library_management_system.Models
{
    public class LoanBookInfo
    {
        public string BookName { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string LoanStatus { get; set; } = string.Empty;
    }
}
