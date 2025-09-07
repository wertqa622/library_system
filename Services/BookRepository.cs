using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using library_management_system.DataBase;
using library_management_system.Models;
using Microsoft.Extensions.Logging;

namespace library_management_system.Services
{
    public class BookRepository
    {
        private readonly OracleDapperHelper _dbHelper;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(OracleDapperHelper dbHelper, ILogger<BookRepository> logger)
        {
            _dbHelper = dbHelper;
            _logger = logger;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            const string sql = @"
                SELECT 
                    ID, TITLE, AUTHOR, ISBN, PUBLISHER, 
                    PUBLISH_DATE, CATEGORY, IS_AVAILABLE, 
                    DESCRIPTION, PRICE, IMAGE_PATH
                FROM BOOKS 
                ORDER BY TITLE";

            try
            {
                return await _dbHelper.QueryAsync<Book>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all books from database");
                throw;
            }
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    ID, TITLE, AUTHOR, ISBN, PUBLISHER, 
                    PUBLISH_DATE, CATEGORY, IS_AVAILABLE, 
                    DESCRIPTION, PRICE, IMAGE_PATH
                FROM BOOKS 
                WHERE ID = :Id";

            try
            {
                return await _dbHelper.QuerySingleAsync<Book>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving book with ID {id} from database");
                throw;
            }
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            const string sql = @"
                SELECT 
                    ID, TITLE, AUTHOR, ISBN, PUBLISHER, 
                    PUBLISH_DATE, CATEGORY, IS_AVAILABLE, 
                    DESCRIPTION, PRICE, IMAGE_PATH
                FROM BOOKS 
                WHERE LOWER(TITLE) LIKE LOWER(:SearchTerm) 
                   OR LOWER(AUTHOR) LIKE LOWER(:SearchTerm)
                   OR LOWER(ISBN) LIKE LOWER(:SearchTerm)
                   OR LOWER(PUBLISHER) LIKE LOWER(:SearchTerm)
                   OR LOWER(CATEGORY) LIKE LOWER(:SearchTerm)
                ORDER BY TITLE";

            try
            {
                return await _dbHelper.QueryAsync<Book>(sql, new { SearchTerm = $"%{searchTerm}%" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching books with term '{searchTerm}' from database");
                throw;
            }
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            const string sql = @"
                INSERT INTO BOOKS (
                    TITLE, AUTHOR, ISBN, PUBLISHER, PUBLISH_DATE, 
                    CATEGORY, IS_AVAILABLE, DESCRIPTION, PRICE, IMAGE_PATH
                ) VALUES (
                    :Title, :Author, :ISBN, :Publisher, :PublishDate,
                    :Category, :IsAvailable, :Description, :Price, :ImagePath
                ) RETURNING ID INTO :Id";

            try
            {
                var parameters = new
                {
                    book.Title,
                    book.Author,
                    book.ISBN,
                    book.Publisher,
                    PublishDate = book.PublishDate,
                    book.Category,
                    book.IsAvailable,
                    book.Description,
                    book.Price,
                    book.ImagePath,
                    Id = 0
                };

                await _dbHelper.ExecuteAsync(sql, parameters);
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding book '{book.Title}' to database");
                throw;
            }
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            const string sql = @"
                UPDATE BOOKS SET 
                    TITLE = :Title,
                    AUTHOR = :Author,
                    ISBN = :ISBN,
                    PUBLISHER = :Publisher,
                    PUBLISH_DATE = :PublishDate,
                    CATEGORY = :Category,
                    IS_AVAILABLE = :IsAvailable,
                    DESCRIPTION = :Description,
                    PRICE = :Price,
                    IMAGE_PATH = :ImagePath
                WHERE ID = :Id";

            try
            {
                var parameters = new
                {
                    book.Id,
                    book.Title,
                    book.Author,
                    book.ISBN,
                    book.Publisher,
                    PublishDate = book.PublishDate,
                    book.Category,
                    book.IsAvailable,
                    book.Description,
                    book.Price,
                    book.ImagePath
                };

                await _dbHelper.ExecuteAsync(sql, parameters);
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating book with ID {book.Id} in database");
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            const string sql = "DELETE FROM BOOKS WHERE ID = :Id";

            try
            {
                var affectedRows = await _dbHelper.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting book with ID {id} from database");
                throw;
            }
        }
    }
} 