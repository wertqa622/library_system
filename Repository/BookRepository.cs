using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_management_system.DataBase;
using library_management_system.Models;
using System.Data;
using Dapper;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

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
                    b.BOOKID AS BookID,
                    b.ISBN,
                    b.BOOKIMAGE,
                    b.BOOKNAME,
                    b.PUBLISHER,
                    b.AUTHOR,
                    b.DESCRIPTION,
                    b.PRICE,
                    b.BOOKURL,
                    CASE WHEN EXISTS (
                        SELECT 1 FROM LOAN l
                        WHERE l.ISBN = b.ISBN AND l.RETURNDATE IS NULL
                    ) THEN 0 ELSE 1 END AS IsAvailable
                FROM BOOK b
                ORDER BY b.BOOKNAME";
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
                    ISBN = :ISBN,
                    BOOKIMAGE = :BookImage,
                    BOOKNAME = :BookName,
                    PUBLISHER = :Publisher,
                    AUTHOR = :Author,
                    DESCRIPTION = :Description,
                    PRICE = :Price,
                    BOOKURL = :BookUrl
                WHERE BOOKID = :BookID";

            // BookImage가 null이면 빈 배열로 설정
            if (book.BookImage == null)
            {
                book.BookImage = new byte[0];
            }

            await _dbHelper.ExecuteAsync(sql, book);
            return book;
        }

        /// <summary>
        /// ISBN으로 도서를 조회합니다. ISBN 중복 확인에 사용됩니다.
        /// </summary>
        /// <param name="isbn">조회할 ISBN</param>
        /// <returns>도서 정보, 없으면 null</returns>
        public async Task<Book> GetBookByIsbnAsync(string isbn)
        {
            try
            {
                const string sql = @"
                SELECT
                    b.BOOKID AS BookID,
                    b.ISBN,
                    b.BOOKIMAGE,
                    b.BOOKNAME,
                    b.PUBLISHER,
                    b.AUTHOR,
                    b.DESCRIPTION,
                    b.PRICE,
                    b.BOOKURL,
                    CASE WHEN EXISTS (
                        SELECT 1 FROM LOAN l
                        WHERE l.ISBN = b.ISBN AND l.RETURNDATE IS NULL
                    ) THEN 0 ELSE 1 END AS IsAvailable
                FROM BOOK b
                WHERE b.ISBN = :ISBN";

                var books = await _dbHelper.QueryAsync<Book>(sql, new { ISBN = isbn });
                return books.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteBookAsync(string isbn)
        {
            const string sql = "DELETE FROM BOOK WHERE ISBN = :ISBN";
            var result = await _dbHelper.ExecuteAsync(sql, new { ISBN = isbn });
            return result > 0;
        }

        // 트랜잭션으로 LOAN(자식) 먼저 삭제한 다음 BOOK(부모) 삭제
        public async Task<bool> DeleteBookAndLoansAsync(string isbn)
        {
            using var conn = _dbHelper.GetConnection();
            // GetConnection() 이미 내부에서 Open() 하므로 중복 Open 호출 제거
            using var tran = conn.BeginTransaction();
            try
            {
                const string deleteLoans = "DELETE FROM LOAN WHERE ISBN = :Isbn";
                await conn.ExecuteAsync(deleteLoans, new { Isbn = isbn }, transaction: tran);

                const string deleteBook = "DELETE FROM BOOK WHERE ISBN = :Isbn";
                int affected = await conn.ExecuteAsync(deleteBook, new { Isbn = isbn }, transaction: tran);

                tran.Commit();
                return affected > 0;
            }
            catch
            {
                try { tran.Rollback(); } catch { /* ignore */ }
                throw;
            }
        }
    }
}