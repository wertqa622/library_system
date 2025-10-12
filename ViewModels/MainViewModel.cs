using System.Collections.ObjectModel;
using System.Windows.Input;
using library_management_system.Models;
using library_management_system.Repository;
using library_management_system;
using System.Linq;
using System.Collections.Generic;
using System; // For StringComparison

namespace library_management_system.ViewModels
{
    public class MainViewModel : ViewModelBase
    {           
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ILoanRepository _loanRepository;

        // --- 도서 관련 변수 ---
        private ObservableCollection<Book> _books;
        private Book _selectedBook;
        private string _searchText;
        private string _selectedSearchFilter;
        private string _hintText;
        private List<Book> _allBooks;

        // --- 회원 관련 변수 ---
        private ObservableCollection<Member> _members;
        private Member _selectedMember;
        private List<Member> _allMembers;
        private string _searchText_Person;
        private string _selectedSearchFilter_Person;
        private string _hintText_1;

        // --- 대출 관련 변수 ---
        private ObservableCollection<Loan> _loans;
        private Loan _selectedLoan;

        public MainViewModel(IBookRepository bookRepository, IMemberRepository memberRepository, ILoanRepository loanRepository)
        {
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _loanRepository = loanRepository;

            Books = new ObservableCollection<Book>();
            Members = new ObservableCollection<Member>();
            Loans = new ObservableCollection<Loan>();

            SearchFilters = new ObservableCollection<string> { "제목", "ISBN" };
            SearchFilters_Person = new ObservableCollection<string> { "이름", "전화번호" };

            SelectedSearchFilter = "제목";
            SelectedSearchFilter_Person = "이름";
            HintText = "제목 검색...";
            HintText_1 = "이름 검색...";

            // 커맨드 초기화
            AddBookCommand = new RelayCommand(AddBook);
            EditBookCommand = new RelayCommand(EditBook, CanEditBook);
            DeleteBookCommand = new RelayCommand(DeleteBook, CanDeleteBook);
            SearchBookCommand = new RelayCommand(SearchBooks);
            SearchMemberCommand = new RelayCommand(SearchMembers);

            LoadData();
        }

        // --- 커맨드 속성 ---
        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand SearchBookCommand { get; }
        public ICommand SearchMemberCommand { get; }

        // --- Public 속성들 ---
        #region Public Properties
        public ObservableCollection<Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public Book SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
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

        public string HintText
        {
            get => _hintText;
            set => SetProperty(ref _hintText, value);
        }

        public ObservableCollection<string> SearchFilters { get; }

        public ObservableCollection<Member> Members
        {
            get => _members;
            set => SetProperty(ref _members, value);
        }

        public Member SelectedMember
        {
            get => _selectedMember;
            set => SetProperty(ref _selectedMember, value);
        }

        public ObservableCollection<string> SearchFilters_Person { get; }

        public string HintText_1
        {
            get => _hintText_1;
            set => SetProperty(ref _hintText_1, value);
        }

        public string SearchText_Person
        {
            get => _searchText_Person;
            set => SetProperty(ref _searchText_Person, value);
        }

        public string SelectedSearchFilter_Person
        {
            get => _selectedSearchFilter_Person;
            set
            {
                if (SetProperty(ref _selectedSearchFilter_Person, value))
                {
                    HintText_1 = $"{value} 검색...";
                }
            }
        }

        public ObservableCollection<Loan> Loans
        {
            get => _loans;
            set => SetProperty(ref _loans, value);
        }

        public Loan SelectedLoan
        {
            get => _selectedLoan;
            set => SetProperty(ref _selectedLoan, value);
        }
        #endregion Public Properties

        // --- 메서드 ---
        private async void LoadData()
        {
            var books = await _bookRepository.GetAllBooksAsync();
            var members = await _memberRepository.GetAllMembersAsync();
            var loans = await _loanRepository.GetAllLoansAsync();

            _allBooks = new List<Book>(books);
            _allMembers = new List<Member>(members);

            Books.Clear();
            foreach (var book in _allBooks) Books.Add(book);

            Members.Clear();
            foreach (var member in _allMembers) Members.Add(member);

            Loans.Clear();
            foreach (var loan in loans) Loans.Add(loan);
        }

        private void SearchBooks()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Books.Clear();
                foreach (var book in _allBooks) Books.Add(book);
                return;
            }

            var filteredBooks = SelectedSearchFilter == "제목"
                ? _allBooks.Where(b => b.BookName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                : _allBooks.Where(b => b.ISBN.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            Books.Clear();
            foreach (var book in filteredBooks) Books.Add(book);
        }

        private void SearchMembers()
        {
            if (string.IsNullOrWhiteSpace(SearchText_Person))
            {
                Members.Clear();
                foreach (var member in _allMembers) Members.Add(member);
                return;
            }
            //회원 정보 조회 추후 구현
           /* var filteredMembers = SelectedSearchFilter_Person == "이름"
                ? _allMembers.Where(m => m.Name.Contains(SearchText_Person, StringComparison.OrdinalIgnoreCase))
                : _allMembers.Where(m => m.PhoneNumber.Contains(SearchText_Person));

            Members.Clear();
            foreach (var member in filteredMembers) Members.Add(member);*/
        }

        // 도서 추가
        private void AddBook()
        {   
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            var addBookWindow = new AddBookWindow(_bookRepository, this);
            addBookWindow.Owner = System.Windows.Application.Current.MainWindow;
            mainWindow.vbgd();
            addBookWindow.ShowDialog();
        }

        // 도서 수정
        private void EditBook()
        {
            if (SelectedBook == null) return;

            var modifyBookWindow = new ModifyBookWindow(_bookRepository, this, SelectedBook);
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
                $"정말로 '{SelectedBook.BookName}' 도서를 삭제하시겠습니까?",
                "도서 삭제 확인",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    await _bookRepository.DeleteBookAsync(SelectedBook.ISBN);
                    Books.Remove(SelectedBook);
                    _allBooks.Remove(SelectedBook); // 원본 리스트에서도 제거
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
