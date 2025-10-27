using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using library_management_system.DataBase;
using library_management_system.Models;
using Oracle.ManagedDataAccess.Client;

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
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            // Oracle 대소문자 구분을 위해 명시적으로 컬럼명 매핑
            const string sql = @"
                SELECT
                    MEMBERID AS ""MemberID"",
                    NAME AS ""Name"",
                    TO_CHAR(BIRTHDATE, 'YYYY-MM-DD') AS ""Birthdaydate"",
                    EMAIL AS ""Email"",
                    PHONENUMBER AS ""Phone"",
                    '' AS ""Address"",
                    SYSDATE AS ""RegistrationDate"",
                    CASE WITHDRAWALSTATUS WHEN 'F' THEN 1 ELSE 0 END AS ""IsActive"",
                    5 AS ""MaxBooksAllowed"",
                    GENDER AS ""Gender"",
                    PHOTO AS ""Photo"",
                    CASE LOANSTATUS WHEN 'T' THEN 1 ELSE 0 END AS ""LoanStatus""
                FROM MEMBER
                ORDER BY NAME";

            var members = await _dbHelper.QueryAsync<Member>(sql);
            return members ?? Enumerable.Empty<Member>();
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

        public async Task UpdateWithdrawalStatusAsync(int memberId, bool withdrawalStatus)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                var transaction = connection.BeginTransaction(); // 트랜잭션 변수 선언
                try
                {
                    await connection.OpenAsync(); // OracleConnection의 OpenAsync 호출
                    using (transaction = connection.BeginTransaction()) // 트랜잭션 시작
                    {
                        const string query = @"
                    UPDATE Members
                    SET WithdrawalStatus = :WithdrawalStatus
                    WHERE MemberID = :MemberID";

                        await connection.ExecuteAsync(query, new { MemberID = memberId, WithdrawalStatus = withdrawalStatus }, transaction);

                        // 트랜잭션 커밋
                        transaction.Commit();
                    }
                }
                catch
                {
                    // 트랜잭션 롤백
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<Member> AddMemberAsync(Member member)
        {
            const string sql = @"
                INSERT INTO MEMBER (
                    NAME,
                    PHONENUMBER,
                    PHOTO,
                    BIRTHDATE,
                    EMAIL,
                    GENDER,
                    WITHDRAWALSTATUS,
                    LOANSTATUS
                )
                VALUES (
                    :Name,
                    :PhoneNumber,
                    :Photo,
                    TO_DATE(:Birthdaydate, 'YYYY-MM-DD'),
                    :Email,
                    :Gender,
                    'F',
                    :LoanStatus
                )
                RETURNING MEMBERID INTO :MemberID";

            var parameters = new DynamicParameters();
            parameters.Add("Name", member.Name ?? "");
            parameters.Add("PhoneNumber", member.Phone ?? "");
            parameters.Add("Photo", member.Photo ?? new byte[0]);
            parameters.Add("Birthdaydate", member.Birthdaydate ?? "");
            parameters.Add("Email", member.Email ?? "");
            parameters.Add("Gender", member.Gender ?? "");
            parameters.Add("LoanStatus", member.LoanStatus ? 'T' : 'F');
            parameters.Add("MemberID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbHelper.ExecuteAsync(sql, parameters);

            // 생성된 MemberID를 가져오기
            member.MemberID = parameters.Get<int>("MemberID");

            return member;
        }

        public async Task<Member> UpdateMemberAsync(Member member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            const string sql = @"
                UPDATE MEMBER SET
                    PHONENUMBER = :PhoneNumber,
                    PHOTO = :Photo,
                    NAME = :Name,
                    BIRTHDATE = :Birthdate,
                    EMAIL = :Email,
                    GENDER = :Gender,
                    WITHDRAWALSTATUS = :WithdrawalStatus,
                    LOANSTATUS = :LoanStatus
                WHERE MEMBERID = :MemberID";

            var parameters = new
            {
                MemberID = member.MemberID,
                PhoneNumber = member.Phone ?? "",
                Photo = member.Photo ?? new byte[0],
                Name = member.Name ?? "",
                Birthdate = member.Birthdaydate != null && DateTime.TryParse(member.Birthdaydate, out var birthdate) ? birthdate : (DateTime?)null,
                Email = member.Email ?? "",
                Gender = member.Gender ?? "",
                WithdrawalStatus = member.IsActive ? 'F' : 'T',
                LoanStatus = member.LoanStatus ? 'T' : 'F'
            };

            await _dbHelper.ExecuteAsync(sql, parameters);
            return member;
        }

        public async Task<bool> DeleteMemberAsync(int id)
        {
            // MemberID를 사용하여 회원 삭제
            const string sql = @"
                DELETE FROM MEMBER
                WHERE MEMBERID = :MemberID";

            var parameters = new { MemberID = id };

            await _dbHelper.ExecuteAsync(sql, parameters);
            return true;
        }
    }
}