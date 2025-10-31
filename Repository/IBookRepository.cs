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

        /// <summary>
        /// ISBN으로 도서를 조회합니다. ISBN 중복 확인에 사용됩니다.
        /// </summary>
        Task<Book> GetBookByIsbnAsync(string isbn);
    }
}