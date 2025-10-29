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

        /// <summary>
        /// (수정) 현재 대출 중인 도서가 있는 모든 회원 목록 조회
        /// NOTE: MEMBER 테이블의 LOANSTATUS 플래그 대신, 실제 LOAN 테이블에 반납되지 않은 기록이 있는지 확인하는 것이 더 정확합니다.
        ///       또한, 스크린샷에 없는 컬럼(ADDRESS 등)은 조회 목록에서 제외했습니다.
        /// </summary>
        public async Task<IEnumerable<Member>> GetActiveLoanMembersAsync()
        {
            string sql = @"
                SELECT DISTINCT
                    M.MEMBERID AS Id,
                    M.NAME AS Name,
                    M.BIRTHDATE AS Birthdaydate,
                    M.EMAIL AS Email,
                    M.PHONENUMBER AS Phone
                FROM MEMBER M
                INNER JOIN LOAN L ON M.PHONENUMBER = L.PHONENUMBER
                WHERE L.RETURNDATE IS NULL
                ORDER BY M.NAME";

            // Member 모델이 스키마와 다르다면, 실제 모델에 맞게 SELECT 구문을 조정해야 합니다.
            var result = await _dbHelper.QueryAsync<Member>(sql);
            return result.ToList();
        }

        /// <summary>
        /// (수정) 특정 회원의 모든 대출 도서 목록 (대출 중 + 반납 완료)
        /// NOTE: 'RETURNDUEDATE'를 스키마에 맞는 'DUEDATE'로 수정했습니다.
        /// </summary>
        public async Task<IEnumerable<LoanBookInfo>> GetLoanBooksByMemberAsync(string phone)
        {
            string sql = @"
                SELECT
                    B.BOOKNAME AS BookName,
                    B.ISBN AS ISBN,
                    B.AUTHOR AS Author,
                    L.LOANDATE AS LoanDate,
                    L.DUEDATE AS DueDate, -- 컬럼명 수정 (RETURNDUEDATE -> DUEDATE)
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

        /// <summary>
        /// (신규) 도서 대출 처리
        /// NOTE: LOAN 테이블에 새로운 대출 기록을 INSERT 합니다. 대출일은 오늘, 반납예정일은 14일 뒤로 설정합니다.
        /// </summary>
        /// <param name="phoneNumber">대출하는 회원의 전화번호</param>
        /// <param name="isbn">대출할 도서의 ISBN</param>
        public async Task<int> LoanBookAsync(string phoneNumber, string isbn)
        {
            string sql = @"
        INSERT INTO LOAN (PHONENUMBER, ISBN, LOANDATE, DUEDATE)
        VALUES (:PhoneNumber, :Isbn, SYSDATE, SYSDATE + 14)";

            return await _dbHelper.ExecuteAsync(sql, new { PhoneNumber = phoneNumber, Isbn = isbn });
        }

        /// <summary>
        /// (신규) 키워드로 도서 검색 (도서명, 저자, 출판사)
        /// </summary>
        public async Task<IEnumerable<Book>> SearchBooksAsync(string keyword)
        {
            string sql = @"
            SELECT
                BOOKID AS Id,
                ISBN,
                BOOKIMAGE AS BookImage,
                BOOKNAME AS BookName,
                PUBLISHER,
                AUTHOR,
                DESCRIPTION,
                PRICE,
                BOOKURL
            FROM BOOK
            WHERE BOOKNAME LIKE :Keyword OR AUTHOR LIKE :Keyword OR PUBLISHER LIKE :Keyword";

            // Book 모델의 프로퍼티 이름이 테이블 컬럼과 일치해야 합니다.
            var result = await _dbHelper.QueryAsync<Book>(sql, new { Keyword = $"%{keyword}%" });
            return result.ToList();
        }

        /// <summary>
        /// 대출 가능한 (현재 대출 중이 아닌) 모든 도서 목록을 조회합니다.
        /// </summary>
        public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
        {
            string sql = @"
            SELECT
                b.BOOKID AS Id, b.ISBN, b.BOOKIMAGE AS BookImage, b.BOOKNAME AS BookName,
                b.PUBLISHER, b.AUTHOR, b.DESCRIPTION, b.PRICE, b.BOOKURL
            FROM BOOK b
            WHERE NOT EXISTS (
                SELECT 1
                FROM LOAN l
                WHERE l.ISBN = b.ISBN AND l.RETURNDATE IS NULL
            )
            ORDER BY b.BOOKNAME";

            return await _dbHelper.QueryAsync<Book>(sql);
        }
    }
}