using System.Collections.Generic;
using System.Threading.Tasks;
using library_management_system.Models;

namespace library_management_system.Repository
{
    public interface IMemberRepository
    {
        Task<IEnumerable<Member>> GetAllMembersAsync();

        Task<Member> GetMemberByIdAsync(int id);

        Task<Member> GetMemberByEmailAsync(string email);

        Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm);

        Task<Member> GetMemberByPhoneAsync(string phone);

        Task<Member> AddMemberAsync(Member member);

        Task<Member> UpdateMemberAsync(Member member);

        Task<bool> DeleteMemberAsync(int id);

<<<<<<< HEAD
        Task<bool> IsMemberActiveAsync(int id);
=======
        Task DeleteByPhoneAsync(string phone);

        Task UpdateWithdrawalStatusAsync(int memberId, bool withdrawalStatus);
>>>>>>> 4343ef4 ([홍서진] 전체 예외처리 및 오류 수정)

        Task<int> GetCurrentLoanCountAsync(int memberId);

        Task<IEnumerable<Member>> GetMembersWithActiveLoansAsync();
    }
}