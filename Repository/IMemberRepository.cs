using System.Collections.Generic;
using System.Threading.Tasks;
using library_management_system.Models;

namespace library_management_system.Repository
{
    public interface IMemberRepository
    {
        Task<IEnumerable<Member>> GetAllMembersAsync();

        Task<Member> GetMemberByEmailAsync(string email);

        Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm);

        Task<Member> AddMemberAsync(Member member);

        Task<Member> UpdateMemberAsync(Member member);

        Task<bool> DeleteMemberAsync(int id);

        Task UpdateWithdrawalStatusAsync(int memberId, bool withdrawalStatus);

        Task<IEnumerable<Member>> GetWithdrawnMembersAsync();

        Task<IEnumerable<Member>> GetMembersWithActiveLoansAsync();
    }
}