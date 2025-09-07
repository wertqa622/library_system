using System.Collections.Generic;
using System.Threading.Tasks;
using library_management_system.Models;

namespace library_management_system.Services
{
    public interface ILoanService
    {
        Task<IEnumerable<Loan>> GetAllLoansAsync();
        Task<Loan> GetLoanByIdAsync(int id);
        Task<IEnumerable<Loan>> GetLoansByMemberIdAsync(int memberId);
        Task<IEnumerable<Loan>> GetLoansByBookIdAsync(int bookId);
        Task<IEnumerable<Loan>> GetActiveLoansAsync();
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();
        Task<Loan> CreateLoanAsync(int bookId, int memberId, int loanDays);
        Task<Loan> ReturnBookAsync(int loanId);
        Task<decimal> CalculateFineAsync(int loanId);
        Task<bool> CanMemberBorrowAsync(int memberId);
        Task<bool> IsBookAvailableAsync(int bookId);
    }
} 