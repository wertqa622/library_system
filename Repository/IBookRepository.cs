using System.Collections.Generic;
using System.Threading.Tasks;
using library_management_system.Models;

namespace library_management_system.Repository
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();

        Task<Book> AddBookAsync(Book book);

        Task<Book> UpdateBookAsync(Book book);

        Task<bool> DeleteBookAsync(string isbn);

        // 부모(BOOK) 삭제 시 관련 LOAN(대출 이력)도 함께 삭제하는 메서드 추가
        Task<bool> DeleteBookAndLoansAsync(string isbn);
    }
}