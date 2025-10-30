using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using library_management_system.Models;
using library_management_system.Repository;
using MessageBox = System.Windows.MessageBox;

namespace library_management_system.ViewModels
{
    public class AddBookViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Book> _books;

        public event Action RequestClose;

        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public DateTime? PublishDate { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        private string _imagePath;

        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        private string _bookURL;

        public string BookURL
        {
            get => _bookURL;
            set => SetProperty(ref _bookURL, value);
        }

        private byte[] _bookImageBytes;

        public byte[] BookImageBytes
        {
            get => _bookImageBytes;
            set => SetProperty(ref _bookImageBytes, value);
        }

        private bool _checkISBN;

        public bool CheckISBN
        {
            get => _checkISBN;
            set
            {
                _checkISBN = value;
                OnPropertyChanged(nameof(CheckISBN));
            }
        }

        public bool IsAvailable { get; set; }

        public ICommand CheckIsbnCommand { get; }
        public ICommand AddBookCommand { get; }
        private readonly IBookRepository _bookRepository;

        public AddBookViewModel(ObservableCollection<Book> existingBooks, IBookRepository bookRepository)
        {
            _books = existingBooks;
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));

            // 기본값 설정
            PublishDate = DateTime.Now;
            IsAvailable = true;
            Price = 0;
            CheckISBN = true;

            CheckIsbnCommand = new RelayCommand(CheckIsbn);
            AddBookCommand = new RelayCommand(AddBook);
        }

        private void CheckIsbn(object parameter)
        {
            string isbn = this.ISBN?.Trim();

            if (string.IsNullOrWhiteSpace(isbn))
            {
                MessageBox.Show("ISBN을 입력해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isDuplicate = _books.Any(book => book.ISBN == isbn);

            if (isDuplicate)
            {
                MessageBox.Show("이미 존재하는 ISBN입니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                CheckISBN = true;
            }
            else
            {
                MessageBox.Show("사용 가능한 ISBN입니다.", "확인", MessageBoxButton.OK, MessageBoxImage.Information);
                CheckISBN = false;
            }
        }

        private async void AddBook(object parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Author) ||
                    string.IsNullOrWhiteSpace(Publisher) || string.IsNullOrWhiteSpace(ISBN) || Price < 0)
                {
                    MessageBox.Show("필수 정보를 모두 입력하고 가격을 확인해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (CheckISBN == true)
                {
                    MessageBox.Show("ISBN 중복 체크를 확인해주세요", "중복확인", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newBook = new Book
                {
                    ISBN = this.ISBN.Trim(),
                    BookName = this.Title.Trim(),
                    Author = this.Author.Trim(),
                    Publisher = this.Publisher.Trim(),
                    Price = this.Price,
                    BookImage = this.BookImageBytes ?? new byte[0],
                    Description = this.Description?.Trim() ?? "",
                    BookUrl = this.BookURL?.Trim() ?? "",
                    IsAvailable = true // 기본값으로 대여 가능 설정
                };

                // 디버깅을 위한 로그
                System.Diagnostics.Debug.WriteLine($"AddBook - ImagePath: '{this.ImagePath}'");
                System.Diagnostics.Debug.WriteLine($"AddBook - BookURL: '{this.BookURL}'");
                System.Diagnostics.Debug.WriteLine($"AddBook - BookImageBytes 길이: {this.BookImageBytes?.Length ?? 0} bytes");
                System.Diagnostics.Debug.WriteLine($"AddBook - BookUrl: '{newBook.BookUrl}'");

                var addedBook = await _bookRepository.AddBookAsync(newBook);

                // 컬렉션에 직접 추가하지 않고 DialogResult만 설정
                // MainViewModel에서 DB 재조회로 갱신됨
                if (addedBook != null)
                {
                    MessageBox.Show("도서가 성공적으로 추가되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.hdgd();
                    }
                    // ViewModel이 View에 창을 닫아달라고 요청
                    RequestClose?.Invoke();
                }
            }
            catch
            {
                MessageBox.Show("도서 추가 중 오류가 발생했습니다: 입력한 값을 확인해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}