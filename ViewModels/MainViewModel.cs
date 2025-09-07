using System.Collections.ObjectModel;
using System.Windows.Input;
using library_management_system.Models;
using library_management_system.Services;
using library_management_system;

namespace library_management_system.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly OptimizedBookService _optimizedBookService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;
        private readonly ILoanService _loanService;

        private ObservableCollection<Book> _books;
        private ObservableCollection<Member> _members;
        private ObservableCollection<Loan> _loans;
        private Book _selectedBook;
        private Member _selectedMember;
        private Loan _selectedLoan;
        private string _searchText;
        private string _selectedSearchFilter;
        private string _hintText;
        private string _hintText_1;

        public MainViewModel(OptimizedBookService optimizedBookService, IBookService bookService, IMemberService memberService, ILoanService loanService)
        {
            _optimizedBookService = optimizedBookService;
            _bookService = bookService;
            _memberService = memberService;
            _loanService = loanService;

            Books = new ObservableCollection<Book>();
            Members = new ObservableCollection<Member>();
            Loans = new ObservableCollection<Loan>();
            //책 제목조회 콤보박스 아이템 목록
            SearchFilters = new ObservableCollection<string> {"제목","ISBN"};
            SearchFilters_Person = new ObservableCollection<string> { "이름", "전화번호" };
            SelectedSearchFilter = "제목";
            SelectedSearchFilter_Person = "이름";
            HintText = "제목 검색...";
            HintText_1 = "이름 검색...";
            // 커맨드 초기화
            AddBookCommand = new RelayCommand(AddBook);
            EditBookCommand = new RelayCommand(EditBook, CanEditBook);
            DeleteBookCommand = new RelayCommand(DeleteBook, CanDeleteBook);

            LoadData();
        }

        // 커맨드들
        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }

        public ObservableCollection<Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public ObservableCollection<Member> Members
        {
            get => _members;
            set => SetProperty(ref _members, value);
        }

        public ObservableCollection<Loan> Loans
        {
            get => _loans;
            set => SetProperty(ref _loans, value);
        }

        public Book SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        public Member SelectedMember
        {
            get => _selectedMember;
            set => SetProperty(ref _selectedMember, value);
        }

        public Loan SelectedLoan
        {
            get => _selectedLoan;
            set => SetProperty(ref _selectedLoan, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string SelectedSearchFilter
        {
            get => _selectedSearchFilter;
            set
            {
                if (SetProperty(ref _selectedSearchFilter, value))
                {
                    HintText = $"{value} 검색...";
                }
            }
        }
        public string SelectedSearchFilter_Person
        {
            get => _selectedSearchFilter;
            set
            {
                if (SetProperty(ref _selectedSearchFilter, value))
                {
                    HintText_1 = $"{value} 검색...";
                }
            }
        }
        public ObservableCollection<string> SearchFilters { get; }

        public ObservableCollection<string> SearchFilters_Person { get; }
        public string HintText
        {
            get => _hintText;
            set => SetProperty(ref _hintText, value);
        }
        public string HintText_1
        {
            get => _hintText_1;
            set => SetProperty(ref _hintText_1, value);
        }
        private async void LoadData()
        {
            // 최적화된 서비스 사용
            var books = await _optimizedBookService.GetAllBooksWithCache();
            var members = await _memberService.GetAllMembersAsync();
            var loans = await _loanService.GetAllLoansAsync();

            Books.Clear();
            Members.Clear();
            Loans.Clear();

            // LINQ 최적화된 변환
            foreach (var book in books)
                Books.Add(book);

            foreach (var member in members)
                Members.Add(member);

            foreach (var loan in loans)
                Loans.Add(loan);
        }

        // 도서 추가
        private void AddBook()
        {
            var addBookWindow = new AddBookWindow(_bookService, this);
            addBookWindow.Owner = System.Windows.Application.Current.MainWindow;
            addBookWindow.ShowDialog();
        }

        // 도서 수정
        private void EditBook()
        {
            if (SelectedBook == null) return;

            var modifyBookWindow = new ModifyBookWindow(_bookService, this, SelectedBook);
            modifyBookWindow.Owner = System.Windows.Application.Current.MainWindow;
            modifyBookWindow.ShowDialog();
        }

        private bool CanEditBook()
        {
            return SelectedBook != null;
        }

        // 도서 삭제
        private async void DeleteBook()
        {
            if (SelectedBook == null) return;

            var result = System.Windows.MessageBox.Show(
                $"정말로 '{SelectedBook.Title}' 도서를 삭제하시겠습니까?",
                "도서 삭제 확인",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    await _bookService.DeleteBookAsync(SelectedBook.Id);
                    Books.Remove(SelectedBook);
                    System.Windows.MessageBox.Show("도서가 성공적으로 삭제되었습니다.", "성공", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show($"도서 삭제 중 오류가 발생했습니다: {ex.Message}", "오류", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private bool CanDeleteBook()
        {
            return SelectedBook != null;
        }
    }
} 