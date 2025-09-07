using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using library_management_system.Models;

namespace library_management_system.Services
{
    public class OptimizedBookService
    {
        private readonly IMemoryCache _cache;
        private readonly IBookService _bookService;
        private readonly ILogger<OptimizedBookService> _logger;
        
        public OptimizedBookService(
            IMemoryCache cache, 
            IBookService bookService,
            ILogger<OptimizedBookService> logger)
        {
            _cache = cache;
            _bookService = bookService;
            _logger = logger;
        }
        
        public async Task<IEnumerable<Book>> GetAllBooksWithCache()
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                const string cacheKey = "all_books";
                
                // 캐시에서 확인
                if (_cache.TryGetValue(cacheKey, out IEnumerable<Book> cachedResult))
                {
                    _logger.LogInformation($"Cache hit: {cachedResult.Count()} books retrieved in {stopwatch.ElapsedMilliseconds}ms");
                    return cachedResult;
                }
                
                // DB에서 조회
                var books = await _bookService.GetAllBooksAsync();
                
                // 캐시 저장 (5분간)
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                
                _cache.Set(cacheKey, books, cacheOptions);
                
                stopwatch.Stop();
                _logger.LogInformation($"Query completed: {books.Count()} books in {stopwatch.ElapsedMilliseconds}ms");
                
                return books;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Query failed after {stopwatch.ElapsedMilliseconds}ms");
                throw;
            }
        }
        
        public async Task<IEnumerable<Book>> SearchBooksOptimized(string searchTerm)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // 캐시 키 생성
                var cacheKey = $"search_books_{searchTerm?.ToLower().Replace(" ", "_") ?? "empty"}";
                
                // 캐시에서 확인
                if (_cache.TryGetValue(cacheKey, out IEnumerable<Book> cachedResult))
                {
                    _logger.LogInformation($"Cache hit: {cachedResult.Count()} books found for '{searchTerm}' in {stopwatch.ElapsedMilliseconds}ms");
                    return cachedResult;
                }
                
                // DB에서 검색
                var books = await _bookService.SearchBooksAsync(searchTerm);
                
                // 캐시 저장 (3분간)
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(3))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                
                _cache.Set(cacheKey, books, cacheOptions);
                
                stopwatch.Stop();
                _logger.LogInformation($"Search completed: {books.Count()} books found for '{searchTerm}' in {stopwatch.ElapsedMilliseconds}ms");
                
                return books;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Search failed after {stopwatch.ElapsedMilliseconds}ms");
                throw;
            }
        }
        
        public async Task<Book> GetBookByIdOptimized(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var cacheKey = $"book_{id}";
                
                // 캐시에서 확인
                if (_cache.TryGetValue(cacheKey, out Book cachedResult))
                {
                    _logger.LogInformation($"Cache hit: Book {id} retrieved in {stopwatch.ElapsedMilliseconds}ms");
                    return cachedResult;
                }
                
                // DB에서 조회
                var book = await _bookService.GetBookByIdAsync(id);
                
                if (book != null)
                {
                    // 캐시 저장 (10분간)
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                    
                    _cache.Set(cacheKey, book, cacheOptions);
                }
                
                stopwatch.Stop();
                _logger.LogInformation($"Query completed: Book {id} in {stopwatch.ElapsedMilliseconds}ms");
                
                return book;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Query failed after {stopwatch.ElapsedMilliseconds}ms");
                throw;
            }
        }
        
        //public void ClearCache()
        //{
        //    _cache.Clear();
        //    _logger.LogInformation("Cache cleared");
        //}
        
        public void RemoveBookFromCache(int bookId)
        {
            _cache.Remove($"book_{bookId}");
            _cache.Remove("all_books"); // 전체 목록도 무효화
            _logger.LogInformation($"Book {bookId} removed from cache");
        }
    }
} 