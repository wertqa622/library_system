using System;

namespace library_management_system.Models
{
    public class Member
    {
        public int MemberID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Birthdaydate { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public int MaxBooksAllowed { get; set; } = 5;
        public string Gender { get; set; } = string.Empty;
        public byte[] Photo { get; set; }
        public bool LoanStatus { get; set; } = true;
        public bool WithdrawalStatus { get; set; }
    }
}