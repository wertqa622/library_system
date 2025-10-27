using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using library_management_system.DataBase;
using library_management_system.Models;


namespace library_management_system.Repository
{
    // 실제 DB 접근 로직이 구현된 클래스
    public class ReturnRepository : IReturnRepository
    {
        private readonly OracleDapperHelper _dbHelper;

        public ReturnRepository(OracleDapperHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // 현재 대출 중인 회원 목록 (활성 회원)
        public async Task<IEnumerable<Member>> GetActiveLoanMembersAsync()
        {
            string sql = @"
                SELECT 
                    MEMBERID AS Id,
                    NAME AS Name,
                    BIRTHDATE AS Birthdaydate,
                    EMAIL AS Email,
                    PHONENUMBER AS Phone,
                    ADDRESS AS Address,
                    REGISTRATIONDATE AS RegistrationDate,
                    CASE WHEN LOANSTATUS = 'A' THEN 1 ELSE 0 END AS IsActive,
                    MAXBOOKS AS MaxBooksAllowed
                FROM MEMBER
                WHERE LOANSTATUS = 'A'
                ORDER BY NAME";

            var result = await _dbHelper.QueryAsync<Member>(sql);
            return result.ToList();
        }

        // 특정 회원의 대출 도서 목록
        public async Task<IEnumerable<LoanBookInfo>> GetLoanBooksByMemberAsync(string phone)
        {
            string sql = @"
                SELECT 
                    B.BOOKNAME AS BookName,
                    B.ISBN AS ISBN,
                    B.AUTHOR AS Author,
                    L.LOANDATE AS LoanDate,
                    L.RETURNDUEDATE AS DueDate,
                    L.RETURNDATE AS ReturnDate,
                    CASE 
                        WHEN L.RETURNDATE IS NULL THEN '대출 중'
                        ELSE '반납 완료'
                    END AS LoanStatus
                FROM LOAN L
                JOIN BOOK B ON L.ISBN = B.ISBN
                JOIN MEMBER M ON L.PHONENUMBER = M.PHONENUMBER
                WHERE M.PHONENUMBER = :Phone
                ORDER BY L.LOANDATE DESC";

            var result = await _dbHelper.QueryAsync<LoanBookInfo>(sql, new { Phone = phone });
            return result.ToList();
        }
    }
}