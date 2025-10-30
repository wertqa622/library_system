using library_management_system.Models;
using library_management_system.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace library_management_system.Repository
{
    public interface ILoanRepository
    {
        // 모든 대출 기록 가져오기
        Task<IEnumerable<Loan>> GetAllLoansAsync();

        Task<IEnumerable<Loan>> SearchLoansAsync(string filter, string searchText);

        // 특정 회원의 대출 기록 가져오기 (MemberId -> PhoneNumber)
        Task<IEnumerable<Loan>> GetLoansByPhoneNumberAsync(string phoneNumber);

        // 특정 책의 대출 기록 가져오기 (BookId -> Isbn)
        Task<IEnumerable<Loan>> GetLoansByIsbnAsync(string isbn);

        // 현재 대출 중인 모든 기록 가져오기
        Task<IEnumerable<Loan>> GetActiveLoansAsync();

        // 연체된 모든 대출 기록 가져오기
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();

        // 새로운 대출 기록 추가 (CreateLoanAsync -> AddLoanAsync)
        Task<Loan> AddLoanAsync(Loan loan);

        // 대출 정보 업데이트 (반납 처리 등) (ReturnBookAsync -> UpdateLoanAsync)
        Task<Loan> UpdateLoanAsync(Loan loan);

        // 특정 회원이 더 대출할 수 있는지 확인 (MemberId -> PhoneNumber)
        Task<bool> CanMemberBorrowAsync(string phoneNumber);

        // 특정 책이 대출 가능한 상태인지 확인 (BookId -> Isbn)
        Task<bool> IsBookAvailableAsync(string isbn);

        Task<IEnumerable<Member>> GetMembersWithActiveLoansAsync();

        Task<bool> ReturnBookAsync(int loanId);

        Task<IEnumerable<LoanBookViewModel>> GetActiveLoansByPhoneAsync(string phoneNumber);

        Task<IEnumerable<LoanBookViewModel>> SearchActiveLoansAsync(string phoneNumber, string keyword);

        // --- 아래는 필요에 따라 구현할 수 있는 메서드들입니다 ---
        // Task<Loan> GetLoanByIdAsync(int loanId);
        // Task<decimal> CalculateFineAsync(int loanId);
    }
}