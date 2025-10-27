using System;

namespace library_management_system.Models
{
    // In Models/Loan.cs
    public class Loan
    {
        public int LoanId { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Isbn { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? BookName { get; set; }
        public string? Author { get; set; }
        public byte[]? BookImage { get; set; }

        public string LoanStatus
        {
            get
            {
                return ReturnDate.HasValue ? "반납 완료" : "대출 중";
            }
        }
    }
}