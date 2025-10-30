using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using library_management_system.Models;
using library_management_system.Repository;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace library_management_system.ViewModels
{
    public class ModifyBookViewModel : ViewModelBase
    {
        private string _title;
        private string _author;
        private string _publisher;
        private string _isbn;
        private int _price;
        private string _imagePath;
        private string _description;
        private string _bookUrl;
        private byte[] _bookImageBytes;
        private readonly IBookRepository _bookRepository;
        private readonly Book _originalBook;

        public event Action<bool> RequestClose;

        public ModifyBookViewModel(Book book, IBookRepository bookRepository)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));
            if (bookRepository == null)
                throw new ArgumentNullException(nameof(bookRepository));

            _bookRepository = bookRepository;
            _originalBook = book;

            // 기존 도서 정보로 초기화
            Title = book.BookName ?? "";
            Author = book.Author ?? "";
            Publisher = book.Publisher ?? "";
            ISBN = book.ISBN ?? "";
            Price = book.Price;
            ImagePath = book.ImagePath ?? "";
            Description = book.Description ?? "";
            BookUrl = book.BookUrl ?? "";
            BookImageBytes = book.BookImage;

            UpdateBookCommand = new RelayCommand(UpdateBook);
        }

        public ICommand UpdateBookCommand { get; }

        public string Title { get; set; } = "";

        public string Author { get; set; } = "";

        public string Publisher { get; set; } = "";

        public string ISBN { get; set; } = "";

        public decimal Price { get; set; }

        public string ImagePath { get; set; } = "";

        public string Description { get; set; } = "";

        public string BookUrl { get; set; } = "";

        public byte[] BookImageBytes
        {
            get => _bookImageBytes;
            set
            {
                _bookImageBytes = value;
                OnPropertyChanged();
            }
        }

        // PhotoBytes는 BookImageBytes의 별칭
        public byte[] PhotoBytes
        {
            get => BookImageBytes;
            set => BookImageBytes = value;
        }

        private async void UpdateBook(object parameter)
        {
            try
            {
                // 입력 검증
                if (string.IsNullOrWhiteSpace(Title))
                {
                    MessageBox.Show("도서 제목을 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Author))
                {
                    MessageBox.Show("저자를 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Publisher))
                {
                    MessageBox.Show("출판사를 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ISBN))
                {
                    MessageBox.Show("ISBN을 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 가격 검증
                if (Price < 0)
                {
                    MessageBox.Show("가격은 0 이상이어야 합니다.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 수정된 도서 객체 생성
                var updatedBook = new Book
                {
                    ISBN = ISBN?.Trim() ?? "",
                    BookName = Title?.Trim() ?? "",
                    Author = Author?.Trim() ?? "",
                    Publisher = Publisher?.Trim() ?? "",
                    Price = Price,
                    BookImage = BookImageBytes ?? new byte[0],
                    Description = Description?.Trim() ?? "",
                    BookUrl = BookUrl?.Trim() ?? "",
                    IsAvailable = true
                };

                // 도서 수정
                await _bookRepository.UpdateBookAsync(updatedBook);

                MessageBox.Show("도서가 성공적으로 수정되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);

                // 성공적으로 수정되었음을 알림
                RequestClose?.Invoke(true);
            }
            catch (Exception)
            {
                MessageBox.Show($"도서 수정 중 오류가 발생했습니다: 입력한 값을 확인해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}