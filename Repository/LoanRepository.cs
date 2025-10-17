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
        private readonly List<Loan> _loans = new List<Loan>();
        private int _nextId = 1;

        public LoanRepository(OracleDapperHelper dbHelper)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            InitializeSampleData();
        }

        public Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            return Task.FromResult(_loans.AsEnumerable());
        }

        public Task<Loan> GetLoanByIdAsync(int id)
        {
            var loan = _loans.FirstOrDefault(l => l.Id == id);
            return Task.FromResult(loan);
        }

        public Task<IEnumerable<Loan>> GetLoansByMemberIdAsync(int memberId)
        {
            var loans = _loans.Where(l => l.MemberId == memberId);
            return Task.FromResult(loans);
        }

        public Task<IEnumerable<Loan>> GetLoansByBookIdAsync(int bookId)
        {
            var loans = _loans.Where(l => l.BookId == bookId);
            return Task.FromResult(loans);
        }

        public Task<IEnumerable<Loan>> GetActiveLoansAsync()
        {
            var activeLoans = _loans.Where(l => !l.IsReturned);
            return Task.FromResult(activeLoans);
        }

        public Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            var overdueLoans = _loans.Where(l => !l.IsReturned && l.DueDate < DateTime.Now);
            return Task.FromResult(overdueLoans);
        }

        public Task<Loan> CreateLoanAsync(int bookId, int memberId, int loanDays)
        {
            var loan = new Loan
            {
                Id = _nextId++,
                BookId = bookId,
                MemberId = memberId,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(loanDays),
                FineAmount = 0
            };

            _loans.Add(loan);
            return Task.FromResult(loan);
        }

        public Task<Loan> ReturnBookAsync(int loanId)
        {
            var loan = _loans.FirstOrDefault(l => l.Id == loanId);
            if (loan != null)
            {
                loan.ReturnDate = DateTime.Now;

                // 연체료 계산
                if (loan.ReturnDate > loan.DueDate)
                {
                    var overdueDays = (loan.ReturnDate.Value - loan.DueDate).Days;
                    loan.FineAmount = overdueDays * 1000m; // 하루당 1000원
                }
            }
            return Task.FromResult(loan);
        }

        public Task<decimal> CalculateFineAsync(int loanId)
        {
            var loan = _loans.FirstOrDefault(l => l.Id == loanId);
            if (loan == null || loan.IsReturned)
                return Task.FromResult(0m);

            if (DateTime.Now > loan.DueDate)
            {
                var overdueDays = (DateTime.Now - loan.DueDate).Days;
                return Task.FromResult(overdueDays * 1000m); // 하루당 1000원
            }

            return Task.FromResult(0m);
        }

        public Task<bool> CanMemberBorrowAsync(int memberId)
        {
            // 현재 대여 중인 책 수 계산
            var currentLoans = _loans.Count(l => l.MemberId == memberId && !l.IsReturned);

            // 기본적으로 5권까지 대여 가능 (실제로는 Member의 MaxBooksAllowed를 확인해야 함)
            return Task.FromResult(currentLoans < 5);
        }

        public Task<bool> IsBookAvailableAsync(int bookId)
        {
            // 해당 책이 현재 대여 중인지 확인
            var isLoaned = _loans.Any(l => l.BookId == bookId && !l.IsReturned);
            return Task.FromResult(!isLoaned);
        }

        private void InitializeSampleData()
        {
            _loans.Add(new Loan
            {
                Id = _nextId++,
                BookId = 1,
                MemberId = 1,
                LoanDate = new DateTime(2024, 1, 15),
                DueDate = new DateTime(2024, 2, 15),
                FineAmount = 0
            });

            _loans.Add(new Loan
            {
                Id = _nextId++,
                BookId = 2,
                MemberId = 2,
                LoanDate = new DateTime(2024, 1, 10),
                DueDate = new DateTime(2024, 2, 10),
                ReturnDate = new DateTime(2024, 2, 5),
                FineAmount = 0
            });

            _loans.Add(new Loan
            {
                Id = _nextId++,
                BookId = 3,
                MemberId = 1,
                LoanDate = new DateTime(2024, 1, 1),
                DueDate = new DateTime(2024, 2, 1),
                FineAmount = 5000 // 연체료
            });
        }
    }
}