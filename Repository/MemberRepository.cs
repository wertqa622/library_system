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
        private readonly ILoanRepository _loanRepository;
        private int _nextId = 1;

        public MemberRepository(OracleDapperHelper dbHelper, ILoanRepository loanRepository)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _loanRepository = loanRepository;
        }

        public MemberRepository(OracleDapperHelper dbHelper)
        {
<<<<<<< HEAD
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            InitializeSampleData();
=======
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// 회원을 탈퇴 처리합니다. 대출 중인 도서가 있으면 실패합니다.
        /// </summary>
        /// <param name="memberId">탈퇴시킬 회원의 ID</param>
        /// <returns>성공 시 true, 실패(대출 도서 존재) 시 false</returns>
        public async Task<bool> WithdrawMemberAsync(int memberId)
        {
            var member = await GetMemberByIdAsync(memberId);
            if (member == null || string.IsNullOrEmpty(member.Phone))
            {
                throw new Exception("회원 정보를 찾을 수 없습니다.");
            }

            // 대출 중인 도서가 있는지 확인
            bool hasActiveLoans = await _loanRepository.HasActiveLoansAsync(member.Phone);
            if (hasActiveLoans)
            {
                return false; // 대출 중이면 탈퇴 실패
            }

            // WITHDRAWALSTATUS를 'T'로 변경하여 탈퇴 처리
            const string sql = "UPDATE MEMBER SET WITHDRAWALSTATUS = 'T' WHERE MEMBERID = :Id";
            var affectedRows = await _dbHelper.ExecuteAsync(sql, new { Id = memberId });
            return affectedRows > 0;
        }

        public async Task<Member> GetMemberByIdAsync(int memberId)
        {
            const string sql = @"
                SELECT
                    m.MEMBERID AS Id,
                    m.NAME AS Name,
                    TO_CHAR(m.BIRTHDATE, 'YYYY-MM-DD') AS Birthdaydate,
                    m.EMAIL AS Email,
                    m.PHONENUMBER AS Phone,
                    m.GENDER AS Gender,
                    m.PHOTO AS Photo,
                    CASE m.LOANSTATUS WHEN 'T' THEN 1 ELSE 0 END AS LoanStatus,
                    CASE m.WITHDRAWALSTATUS WHEN 'F' THEN 1 ELSE 0 END AS IsActive,
                    CASE 
                        WHEN (SELECT COUNT(*) FROM LOAN l WHERE l.PHONENUMBER = m.PHONENUMBER AND l.RETURNDATE IS NULL) < 5 THEN 1
                        ELSE 0
                    END AS CanBorrow,
                    CASE 
                        WHEN (SELECT COUNT(*) FROM LOAN l2 WHERE l2.PHONENUMBER = m.PHONENUMBER AND l2.RETURNDATE IS NULL) > 0 THEN 1
                        ELSE 0
                    END AS HasActiveLoans
                FROM MEMBER m
                WHERE m.MEMBERID = :Id";
            var members = await _dbHelper.QueryAsync<Member>(sql, new { Id = memberId });
            return members.FirstOrDefault();
>>>>>>> 4343ef4 ([홍서진] 전체 예외처리 및 오류 수정)
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
<<<<<<< HEAD
            string sql = "SELECT MEMBERID AS Id, NAME, EMAIL, PHONENUMBER AS Phone, BIRTHDATE AS Birthdaydate, GENDER FROM MEMBER WHERE WITHDRAWALSTATUS = 'N' ORDER BY NAME";
            return await _dbHelper.QueryAsync<Member>(sql);
=======
            // Oracle 대소문자 구분을 위해 명시적으로 컬럼명 매핑
            const string sql = @"
                SELECT
     m.MEMBERID AS MemberID,
     m.NAME AS Name,
     TO_CHAR(m.BIRTHDATE, 'YYYY-MM-DD') AS Birthdaydate,
     m.EMAIL AS Email,
     m.PHONENUMBER AS Phone,
     '' AS Address,
     SYSDATE AS RegistrationDate,
     CASE m.WITHDRAWALSTATUS WHEN 'F' THEN 1 ELSE 0 END AS IsActive,
     5 AS MaxBooksAllowed,
     m.GENDER AS Gender,
     m.PHOTO AS Photo,
     CASE m.LOANSTATUS WHEN 'T' THEN 1 ELSE 0 END AS LoanStatus,
     CASE
         WHEN (SELECT COUNT(*) FROM LOAN l WHERE l.PHONENUMBER = m.PHONENUMBER AND l.RETURNDATE IS NULL) < 5 THEN 1
         ELSE 0
     END AS CanBorrow,
     CASE 
         WHEN (SELECT COUNT(*) FROM LOAN l2 WHERE l2.PHONENUMBER = m.PHONENUMBER AND l2.RETURNDATE IS NULL) > 0 THEN 1
         ELSE 0
     END AS HasActiveLoans
 FROM MEMBER m
 WHERE m.WITHDRAWALSTATUS = 'F'
 ORDER BY m.NAME";

            var members = await _dbHelper.QueryAsync<Member>(sql);
            return members ?? Enumerable.Empty<Member>();
>>>>>>> 4c71f2c ([홍서진] 전체 수정)
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

<<<<<<< HEAD
        public Task<Member> AddMemberAsync(Member member)
=======
        public async Task<Member> GetMemberByPhoneAsync(string phone)
        {
            try
            {
                const string sql = @"
                SELECT
                    m.MEMBERID AS Id,
                    m.NAME AS Name,
                    TO_CHAR(m.BIRTHDATE, 'YYYY-MM-DD') AS Birthdaydate,
                    m.EMAIL AS Email,
                    m.PHONENUMBER AS Phone,
                    m.GENDER AS Gender,
                    m.PHOTO AS Photo,
                    CASE WHEN m.LOANSTATUS = 'T' THEN 1 ELSE 0 END AS LoanStatus,
                    CASE WHEN m.WITHDRAWALSTATUS = 'F' THEN 1 ELSE 0 END AS IsActive,
                    m.WITHDRAWALSTATUS AS WithdrawalStatus,
                    CASE 
                        WHEN (SELECT COUNT(*) FROM LOAN l WHERE l.PHONENUMBER = m.PHONENUMBER AND l.RETURNDATE IS NULL) < 5 THEN 1
                        ELSE 0
                    END AS CanBorrow,
                    CASE 
                        WHEN (SELECT COUNT(*) FROM LOAN l2 WHERE l2.PHONENUMBER = m.PHONENUMBER AND l2.RETURNDATE IS NULL) > 0 THEN 1
                        ELSE 0
                    END AS HasActiveLoans
                FROM MEMBER m
                WHERE m.PHONENUMBER = :Phone";

                var members = await _dbHelper.QueryAsync<Member>(sql, new { Phone = phone });
                return members.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public async Task UpdateWithdrawalStatusAsync(int memberId, bool withdrawalStatus)
>>>>>>> 4343ef4 ([홍서진] 전체 예외처리 및 오류 수정)
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

<<<<<<< HEAD
        public Task<int> GetCurrentLoanCountAsync(int memberId)
        {
            // 실제로는 LoanRepository에서 가져와야 하지만, 여기서는 0으로 반환
            return Task.FromResult(0);
        }

        // 실제 DB에서 대출 중인 회원만 불러오기
        public async Task<IEnumerable<Member>> GetMembersWithActiveLoansAsync()
=======
        public async Task DeleteByPhoneAsync(string phone)
        {
            const string sql = @"
                DELETE FROM MEMBER
                WHERE PHONENUMBER = :Phone";
            await _dbHelper.ExecuteAsync(sql, new { Phone = phone });
        }

        public async Task<IEnumerable<Member>> GetWithdrawnMembersAsync()
>>>>>>> 4343ef4 ([홍서진] 전체 예외처리 및 오류 수정)
        {
            const string sql = @"
                    SELECT DISTINCT
                        m.MEMBERID AS Id,
                        m.NAME AS Name,
                        m.BIRTHDATE AS Birthdaydate,
                        m.EMAIL AS Email,
                        m.PHONENUMBER AS Phone,
                        m.GENDER AS Gender
                    FROM MEMBER m
                    JOIN LOAN l ON m.PHONENUMBER = l.PHONENUMBER
                    WHERE l.RETURNDATE IS NULL";

            var members = await _dbHelper.QueryAsync<Member>(sql);

            // 디버깅용 메시지 — 실제 데이터가 로드되는지 확인
            System.Diagnostics.Debug.WriteLine($"[DEBUG] 조회된 회원 수: {members.Count()}");

            return members;
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