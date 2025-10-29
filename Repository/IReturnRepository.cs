using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using library_management_system.Models;

namespace library_management_system.Repository
{
    public interface IReturnRepository
    {
        // 현재 도서 대출 중인 회원 목록 조회
        Task<IEnumerable<Member>> GetActiveLoanMembersAsync();

        // 특정 회원의 대출 도서 목록 조회
        Task<IEnumerable<LoanBookInfo>> GetLoanBooksByMemberAsync(string phone);

        Task<IEnumerable<Book>> SearchBooksAsync(string keyword);

        Task<int> LoanBookAsync(string phoneNumber, string isbn);

        Task<IEnumerable<Book>> GetAvailableBooksAsync();
    }
}