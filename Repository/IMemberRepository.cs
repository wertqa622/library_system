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
        Task<Member> AddMemberAsync(Member member);
        Task<Member> UpdateMemberAsync(Member member);
        Task<bool> DeleteMemberAsync(int id);
        Task<bool> IsMemberActiveAsync(int id);
        Task<int> GetCurrentLoanCountAsync(int memberId);
        Task<IEnumerable<Member>> GetMembersWithActiveLoansAsync();
    }
}