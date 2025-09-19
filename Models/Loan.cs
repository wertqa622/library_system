namespace library_management_system
{
    public class Loan
    {
        public string Name { get; set; }
        public string ISBN { get; set; }
        public string MemberId { get; set; } // 대여일로 사용됨
        public string LoanDate { get; set; } // 실제로는 반납예정일로 사용됨
        public string DueDate { get; set; }  // 반납일로 사용됨
        public string IsOverdue { get; set; }
    }
}