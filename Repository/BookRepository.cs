using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_management_system.DataBase;
using library_management_system.Models;

namespace library_management_system.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly OracleDapperHelper _dbHelper;

        public BookRepository(OracleDapperHelper dbHelper)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
        }
        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            const string sql = @"
                SELECT 
                    ISBN,
                    BOOKIMAGE,
                    BOOKNAME,
                    PUBLISHER,
                    AUTHOR,
                    DESCRIPTION,
                    PRICE,
                    BOOKURL
                FROM BOOK 
                ORDER BY BOOKNAME";
            var books = await _dbHelper.QueryAsync<Book>(sql);
            return books;
        }

       

        public async Task<Book> AddBookAsync(Book book)
        {
            // 디버깅을 위한 로그
            System.Diagnostics.Debug.WriteLine($"AddBookAsync - BookUrl: '{book.BookUrl}'");
            System.Diagnostics.Debug.WriteLine($"AddBookAsync - ImagePath: '{book.ImagePath}'");
            System.Diagnostics.Debug.WriteLine($"AddBookAsync - BookImage 길이: {book.BookImage?.Length ?? 0} bytes");
            
            const string sql = @"
                INSERT INTO BOOK (
                    ISBN,
                    BOOKIMAGE,
                    BOOKNAME,
                    PUBLISHER,
                    AUTHOR,
                    DESCRIPTION,
                    PRICE,
                    BOOKURL
                ) VALUES (
                    :ISBN,
                    :BookImage,
                    :BookName,
                    :Publisher,
                    :Author,
                    :Description,
                    :Price,
                    :BookUrl
                )";

            await _dbHelper.ExecuteAsync(sql, book);

            return book;
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            
            const string sql = @"
                UPDATE BOOK SET
                    BOOKIMAGE = :BookImage,
                    BOOKNAME = :BookName,
                    PUBLISHER = :Publisher,
                    AUTHOR = :Author,
                    DESCRIPTION = :Description,
                    PRICE = :Price,
                    BOOKURL = :BookUrl
                WHERE ISBN = :ISBN";
            
            // BookImage가 null이면 빈 배열로 설정
            if (book.BookImage == null)
            {
                book.BookImage = new byte[0];
            }
            
            await _dbHelper.ExecuteAsync(sql, book);
            return book;
        }

        public async Task<bool> DeleteBookAsync(string isbn)
        {
            const string sql = "DELETE FROM BOOK WHERE ISBN = :ISBN";
            var result = await _dbHelper.ExecuteAsync(sql, new { ISBN = isbn });
            return result > 0;
        }

        
    }
}
