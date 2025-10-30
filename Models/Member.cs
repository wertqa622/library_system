using System;

namespace library_management_system.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Birthdaydate { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public int MaxBooksAllowed { get; set; } = 5;
        public string Gender { get; set; } = string.Empty;
<<<<<<< HEAD
=======
        public byte[] Photo { get; set; }
        public bool LoanStatus { get; set; } = true;
        public string WithdrawalStatus { get; set; } 

        // UI 탈퇴 가능 여부 판단용 (DB 저장용 아님)
        public bool CanWithdraw
        {
            get { return WithdrawalStatus == "F"; }
        }
<<<<<<< HEAD
>>>>>>> 4343ef4 ([홍서진] 전체 예외처리 및 오류 수정)
=======

        // UI 대출 가능 여부 (현재 대출 권수 < MaxBooksAllowed)
        public bool CanBorrow { get; set; }

        // 현재 대출 중 여부 (하나라도 대출 중이면 true)
        public bool HasActiveLoans { get; set; }
>>>>>>> 4c71f2c ([홍서진] 전체 수정)
    }
}