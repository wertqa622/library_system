using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_management_system.DataBase;
using library_management_system.Models;

namespace library_management_system.Repository
{
    public class LoanRepository : ILoanRepository
    {
        private readonly OracleDapperHelper _dbHelper;

        public LoanRepository(OracleDapperHelper dbHelper)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
        }

        // 모든 대출 기록 조회
        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            const string sql = @"
                   SELECT
                        m.NAME,
                        l.PHONENUMBER,
                        l.ISBN,
                        l.LOANDATE,
                        l.DUEDATE,
                        l.RETURNDATE,
                        CASE
                            WHEN l.RETURNDATE IS NULL THEN '대출 중'
                            ELSE '반납 완료'
                    END AS LoanStatus
                    FROM LOAN l
                    LEFT JOIN
                    MEMBER m ON l.PHONENUMBER = m.PHONENUMBER";

            var loan = await _dbHelper.QueryAsync<Loan>(sql);
            return loan;
        }

        // 특정 회원의 대출 기록 조회
        public async Task<IEnumerable<Loan>> GetLoansByPhoneNumberAsync(string phoneNumber)
        {
            const string sql = @"
                SELECT
                    LOAN_ID,
                    PHONENUMBER,
                    ISBN,
                    LOANDATE,
                    DUEDATE,
                    RETURNDATE
                FROM LOAN
                WHERE PHONENUMBER = :PhoneNumber";

            return await _dbHelper.QueryAsync<Loan>(sql, new { PhoneNumber = phoneNumber });
        }

        // 새로운 대출 기록 추가
        public async Task<Loan> AddLoanAsync(Loan loan)
        {
            // LOAN_ID는 시퀀스(Sequence)를 통해 자동으로 생성하는 것을 권장합니다.
            // 예: LOAN_SEQ.NEXTVAL
            const string sql = @"
                INSERT INTO LOAN (LOAN_ID, PHONENUMBER, ISBN, LOANDATE, DUEDATE)
                VALUES (LOAN_SEQ.NEXTVAL, :PhoneNumber, :Isbn, :LoanDate, :DueDate)";

            await _dbHelper.ExecuteAsync(sql, loan);
            return loan; // 실제로는 시퀀스로 생성된 ID를 다시 조회해서 반환하는 것이 더 좋습니다.
        }

        // 반납 처리 (대출 기록 업데이트)
        public async Task<Loan> UpdateLoanAsync(Loan loan)
        {
            const string sql = @"
                UPDATE LOAN SET
                    RETURNDATE = :ReturnDate
                WHERE LOAN_ID = :LoanId";

            await _dbHelper.ExecuteAsync(sql, loan);
            return loan;
        }

        // 연체된 대출 기록 조회 (반납일이 비어있고, 반납예정일이 오늘보다 이전인 경우)
        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            // SYSDATE는 Oracle에서 현재 날짜와 시간을 의미합니다.
            const string sql = @"
                SELECT
                    LOAN_ID,
                    PHONENUMBER,
                    ISBN,
                    LOANDATE,
                    DUEDATE,
                    RETURNDATE
                FROM LOAN
                WHERE RETURNDATE IS NULL AND DUEDATE < SYSDATE";

            return await _dbHelper.QueryAsync<Loan>(sql);
        }

        // 특정 책이 현재 대출 가능한지 확인
        public async Task<bool> IsBookAvailableAsync(string isbn)
        {
            const string sql = "SELECT COUNT(*) FROM LOAN WHERE ISBN = :Isbn AND RETURNDATE IS NULL";
            var loanCount = await _dbHelper.ExecuteAsync(sql, new { Isbn = isbn });
            return loanCount == 0; // 대출 기록이 없으면(0) 대출 가능(true)
        }

        // --- 여기에 누락된 메서드 3개 추가 ---

        // 특정 책의 대출 기록 조회
        public async Task<IEnumerable<Loan>> GetLoansByIsbnAsync(string isbn)
        {
            const string sql = @"
                SELECT
                    LOAN_ID,
                    PHONENUMBER,
                    ISBN,
                    LOANDATE,
                    DUEDATE,
                    RETURNDATE
                FROM LOAN
                WHERE ISBN = :Isbn";

            return await _dbHelper.QueryAsync<Loan>(sql, new { Isbn = isbn });
        }

        // 현재 대출 중인 모든 기록 가져오기 (반납되지 않은 것)
        public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
        {
            const string sql = @"
                SELECT
                    LOAN_ID,
                    PHONENUMBER,
                    ISBN,
                    LOANDATE,
                    DUEDATE,
                    RETURNDATE
                FROM LOAN
                WHERE RETURNDATE IS NULL";

            return await _dbHelper.QueryAsync<Loan>(sql);
        }

        // 특정 회원이 더 대출할 수 있는지 확인
        public async Task<bool> CanMemberBorrowAsync(string phoneNumber)
        {
            // 한 사람이 최대 5권까지 빌릴 수 있다고 가정
            const int maxLoans = 5;

            const string sql = @"
                SELECT COUNT(*)
                FROM LOAN
                WHERE PHONENUMBER = :PhoneNumber AND RETURNDATE IS NULL";

            var currentLoanCount = await _dbHelper.ExecuteAsync(sql, new { PhoneNumber = phoneNumber });

            return currentLoanCount < maxLoans;
        }

        // ILoanRepository 인터페이스에 정의된 다른 메서드들도
        // 위와 같은 방식으로 SQL 쿼리를 사용하여 구현해야 합니다.
    }
}