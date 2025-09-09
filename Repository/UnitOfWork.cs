using System;
using System.Threading.Tasks;
using library_management_system.DataBase;

namespace library_management_system.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OracleDapperHelper _dbHelper;
        private bool _disposed = false;

        public UnitOfWork(OracleDapperHelper dbHelper)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));

            // Repository 인스턴스 생성
            Books = new BookRepository(_dbHelper);
            Members = new MemberRepository(_dbHelper);
            Loans = new LoanRepository(_dbHelper);
        }

        public IBookRepository Books { get; private set; }
        public IMemberRepository Members { get; private set; }
        public ILoanRepository Loans { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            // Oracle Dapper에서는 자동 커밋이므로 별도 처리 불필요
            // 트랜잭션 처리는 BeginTransaction, CommitTransaction에서 처리
            return await Task.FromResult(1);
        }

        public async Task BeginTransactionAsync()
        {
            // Oracle Dapper 트랜잭션 시작
            // 실제 구현에서는 OracleTransaction 사용
            await Task.CompletedTask;
        }

        public async Task CommitTransactionAsync()
        {
            // 트랜잭션 커밋
            await Task.CompletedTask;
        }

        public async Task RollbackTransactionAsync()
        {
            // 트랜잭션 롤백
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // 리소스 정리
                _disposed = true;
            }
        }
    }
}