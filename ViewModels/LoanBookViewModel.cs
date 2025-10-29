using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library_management_system.ViewModels
{
    public class LoanBookViewModel
    {
        public int LoanId { get; set; }
        public string Isbn { get; set; } = string.Empty;
        public string BookName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}