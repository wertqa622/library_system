using System.ComponentModel;
using System.Runtime.CompilerServices;
using library_management_system.Models;

namespace library_management_system.ViewModels
{
    public class ModifyBookViewModel : ViewModelBase
    {
        private string _title;
        private string _author;
        private string _publisher;
        private int _price;
        private string _imagePath;
        private string _description;
<<<<<<< HEAD
=======
        private string _bookUrl;
        private byte[] _bookImageBytes;
        private bool _checkIsbnFlag = false;
        private bool _isInitializing = false;
        private readonly IBookRepository _bookRepository;
        private readonly Book _originalBook;
>>>>>>> ca40a71 ([홍서진] 최종 수정 완료)

        public ModifyBookViewModel(Book book)
        {
<<<<<<< HEAD
=======
            if (book == null)
                throw new ArgumentNullException(nameof(book));
            if (bookRepository == null)
                throw new ArgumentNullException(nameof(bookRepository));

            _bookRepository = bookRepository;
            _originalBook = book;

            // 초기화 시작
            _isInitializing = true;

>>>>>>> ca40a71 ([홍서진] 최종 수정 완료)
            // 기존 도서 정보로 초기화
            Title = book.BookName;
            Author = book.Author;
            Publisher = book.Publisher;
            ISBN = book.ISBN;
            Price = book.Price;
<<<<<<< HEAD
            ImagePath = book.ImagePath;
            Description = book.Description;
        }

        public string Title { get; set; }
=======
            ImagePath = book.ImagePath ?? "";
            Description = book.Description ?? "";
            BookUrl = book.BookUrl ?? "";
            BookImageBytes = book.BookImage;

            // 초기화 완료 - 초기에는 ISBN TextBox를 활성화 상태로 설정 (수정 가능)
            CheckIsbnFlag = true;
            _isInitializing = false;

            UpdateBookCommand = new RelayCommand(UpdateBook);
            CheckIsbnCommand = new RelayCommand(CheckIsbn);
        }

        public ICommand UpdateBookCommand { get; }
        public ICommand CheckIsbnCommand { get; }

        public bool CheckIsbnFlag
        {
            get => _checkIsbnFlag;
            set
            {
                _checkIsbnFlag = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsIsbnEnabled));
            }
        }

        public bool IsIsbnEnabled
        {
            get => CheckIsbnFlag;
        }
>>>>>>> ca40a71 ([홍서진] 최종 수정 완료)

        public string Author { get; set; }

        public string Publisher { get; set; }

<<<<<<< HEAD
        public string ISBN { get; set; }
=======
        public string Publisher { get; set; } = "";

        private string _isbnBackingField;
        public string ISBN
        {
            get => _isbnBackingField;
            set
            {
                _isbnBackingField = value;
                // 초기화 중이 아니고, ISBN이 변경되면 중복 확인 필요 (활성화)
                if (!_isInitializing && _originalBook != null && value?.Trim() != (_originalBook.ISBN?.Trim() ?? ""))
                {
                    CheckIsbnFlag = true;
                }
            }
        }
>>>>>>> ca40a71 ([홍서진] 최종 수정 완료)

        public decimal Price { get; set; }

        public string ImagePath { get; set; }

<<<<<<< HEAD
        public string Description { get; set; }
=======
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

        private async void CheckIsbn(object parameter)
        {
            string isbn = ISBN?.Trim();

            if (string.IsNullOrWhiteSpace(isbn))
            {
                MessageBox.Show("ISBN을 입력해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 원본 ISBN과 같으면 중복 확인 불필요
            if (isbn == (_originalBook.ISBN?.Trim() ?? ""))
            {
                MessageBox.Show("사용 가능한 ISBN입니다.", "확인", MessageBoxButton.OK, MessageBoxImage.Information);
                CheckIsbnFlag = false;
                return;
            }

            try
            {
                var existingBook = await _bookRepository.GetBookByIsbnAsync(isbn);
                if (existingBook == null)
                {
                    // DB에 없음 → 진행 가능
                    MessageBox.Show("사용 가능한 ISBN입니다.", "사용 가능", MessageBoxButton.OK, MessageBoxImage.Information);
                    CheckIsbnFlag = false;
                    return;
                }

                // ISBN이 존재함 → 중복으로 간주
                MessageBox.Show("이미 존재하는 ISBN입니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                CheckIsbnFlag = true;
            }
            catch
            {
                MessageBox.Show("ISBN 확인 중 오류가 발생했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

                // ISBN 중복 확인 검증
                string trimmedIsbn = ISBN?.Trim() ?? "";
                if (trimmedIsbn != (_originalBook.ISBN?.Trim() ?? ""))
                {
                    // ISBN이 변경되었다면 중복 확인이 필요함 (CheckIsbnFlag가 true이면 아직 중복 확인 안 함)
                    if (CheckIsbnFlag)
                    {
                        MessageBox.Show("ISBN 중복 체크를 확인해주세요.", "중복 확인 필요", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                // ISBN이 원본과 같으면 중복 확인 불필요 (CheckIsbnFlag 상태와 무관하게 진행)

                // 가격 검증
                if (Price < 0)
                {
                    MessageBox.Show("가격은 0 이상이어야 합니다.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 수정된 도서 객체 생성
                var updatedBook = new Book
                {
                    BookID = _originalBook.BookID,
                    ISBN = trimmedIsbn,
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
>>>>>>> 4343ef4 ([홍서진] 전체 예외처리 및 오류 수정)
    }
}