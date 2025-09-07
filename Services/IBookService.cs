using System.Collections.Generic;
using System.Threading.Tasks;
using library_management_system.Models;

namespace library_management_system.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int id);
        
        Task<Book> GetBookByImageAsync(string ImagePath);
        Task<Book> GetBookByPriceAsync(int Price);

        Task<Book> GetBookByIsbnAsync(string isbn);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
        Task<Book> AddBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<IEnumerable<Book>> GetAvailableBooksAsync();
    }
} 