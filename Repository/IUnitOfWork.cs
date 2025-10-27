using System;
using System.Threading.Tasks;

namespace library_management_system.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Books { get; }
        IMemberRepository Members { get; }
        ILoanRepository Loans { get; }
        IReturnRepository Returns { get; }

        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}