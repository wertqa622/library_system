using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_management_system.DataBase;
using library_management_system.Models;

namespace library_management_system.Repository
{
    public class MemberRepository : IMemberRepository
    {
        private readonly OracleDapperHelper _dbHelper;
        private readonly List<Member> _members = new List<Member>();
        private int _nextId = 1;

        public MemberRepository(OracleDapperHelper dbHelper)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            InitializeSampleData();
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            const string sql = @"SELECT * FROM MEMBER ORDER BY NAME";
            var Member = await _dbHelper.QueryAsync<Member>(sql);
            return Member;
        }

        public Task<Member> GetMemberByIdAsync(int id)
        {
            var member = _members.FirstOrDefault(m => m.Id == id);
            return Task.FromResult(member);
        }

        public Task<Member> GetMemberByEmailAsync(string email)
        {
            var member = _members.FirstOrDefault(m => m.Email == email);
            return Task.FromResult(member);
        }

        public Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllMembersAsync();

            var results = _members.Where(m => 
                m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.Phone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(results);
        }

        public Task<Member> AddMemberAsync(Member member)
        {
            member.Id = _nextId++;
            member.RegistrationDate = DateTime.Now;
            member.IsActive = true;
            member.MaxBooksAllowed = 5; // 기본값
            _members.Add(member);
            return Task.FromResult(member);
        }

        public Task<Member> UpdateMemberAsync(Member member)
        {
            var existingMember = _members.FirstOrDefault(m => m.Id == member.Id);
            if (existingMember != null)
            {
                var index = _members.IndexOf(existingMember);
                _members[index] = member;
            }
            return Task.FromResult(member);
        }

        public Task<bool> DeleteMemberAsync(int id)
        {
            var member = _members.FirstOrDefault(m => m.Id == id);
            if (member != null)
            {
                _members.Remove(member);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> IsMemberActiveAsync(int id)
        {
            var member = _members.FirstOrDefault(m => m.Id == id);
            return Task.FromResult(member?.IsActive ?? false);
        }

        public Task<int> GetCurrentLoanCountAsync(int memberId)
        {
            // 실제로는 LoanRepository에서 가져와야 하지만, 여기서는 0으로 반환
            return Task.FromResult(0);
        }

        private void InitializeSampleData()
        {
            _members.Add(new Member
            {
                Id = _nextId++,
                Name = "박정히",
                Email = "123",
                Phone = "010-1234-5678",
                Address = "부산",
                RegistrationDate = new DateTime(2023, 1, 1),
                IsActive = true,
                MaxBooksAllowed = 5
            });

            _members.Add(new Member
            {
                Id = _nextId++,
                Name = "이태경",
                Email = "이태경",
                Phone = "010-2345-6789",
                Address = "중국",
                RegistrationDate = new DateTime(2023, 2, 15),
                IsActive = true,
                MaxBooksAllowed = 3
            });

            _members.Add(new Member
            {
                Id = _nextId++,
                Name = "홍서진",
                Email = "123",
                Phone = "010-3456-7890",
                Address = "123",
                RegistrationDate = new DateTime(2023, 3, 10),
                IsActive = false,
                MaxBooksAllowed = 5
            });
        }
    }
}