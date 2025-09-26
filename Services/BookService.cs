using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_management_system.Models;

namespace library_management_system.Services
{
    public class BookService : IBookService
    {
        private readonly List<Book> _books = new List<Book>();
        private int _nextId = 1;

        public BookService()
        {
            // 샘플 데이터 추가
            InitializeSampleData();
        }

        public Task<Book> GetBookByPriceAsync(int Price)
        {
            var book = _books.FirstOrDefault(b => b.Id == Price);
            return Task.FromResult(book);
        }

        public Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return Task.FromResult(_books.AsEnumerable());
        }

        public Task<Book> GetBookByImageAsync(string imagePath)
        {
            var book = _books.FirstOrDefault(b => b.ImagePath == imagePath);
            return Task.FromResult(book);
        }

        public Task<Book> GetBookByIdAsync(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            return Task.FromResult(book);
        }

        public Task<Book> GetBookByIsbnAsync(string isbn)
        {
            var book = _books.FirstOrDefault(b => b.ISBN == isbn);
            return Task.FromResult(book);
        }

        public Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllBooksAsync();

            var results = _books.Where(b =>
                b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.ISBN.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.Publisher.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(results);
        }

        public Task<Book> AddBookAsync(Book book)
        {
            book.Id = _nextId++;
            _books.Add(book);
            return Task.FromResult(book);
        }

        public Task<Book> UpdateBookAsync(Book book)
        {
            var existingBook = _books.FirstOrDefault(b => b.Id == book.Id);
            if (existingBook != null)
            {
                var index = _books.IndexOf(existingBook);
                _books[index] = book;
            }
            return Task.FromResult(book);
        }

        public Task<bool> DeleteBookAsync(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                _books.Remove(book);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<Book>> GetAvailableBooksAsync()
        {
            var availableBooks = _books.Where(b => b.IsAvailable);
            return Task.FromResult(availableBooks);
        }

        private void InitializeSampleData()
        {
            _books.Add(new Book
            {
                Id = _nextId++,
                Title = "C# 프로그래밍",
                Author = "김철수",
                ISBN = "978-89-1234-5678-1",
                Publisher = "한빛미디어",
                PublishDate = new DateTime(2023, 1, 15),
                Category = "프로그래밍",
                IsAvailable = true,
                Description = "C# 언어의 기초부터 고급까지 다루는 책",
                Price = 35000,
                ImagePath = "Images/books/132.png"
            });

            _books.Add(new Book
            {
                Id = _nextId++,
                Title = "WPF 마스터하기",
                Author = "이영희",
                ISBN = "978-89-1234-5678-2",
                Publisher = "정보문화사",
                PublishDate = new DateTime(2023, 3, 20),
                Category = "프로그래밍",
                IsAvailable = true,
                Description = "WPF를 이용한 데스크톱 애플리케이션 개발",
                Price = 28000,
                ImagePath = "Images/books/132.png"
            });

            _books.Add(new Book
            {
                Id = _nextId++,
                Title = "데이터베이스 설계",
                Author = "박민수",
                ISBN = "978-89-1234-5678-3",
                Publisher = "교보문고",
                PublishDate = new DateTime(2022, 11, 10),
                Category = "데이터베이스",
                IsAvailable = false,
                Description = "효율적인 데이터베이스 설계 방법론",
                Price = 32000,
                ImagePath = "Images/books/132.png"
            });
        }
    }
}