using System.Windows.Controls;
using System.Collections.ObjectModel; // ObservableCollection을 위해 추가
using library_management_system.Models;
using library_management_system.Repository; // ILoanRepository를 위해 추가
using Microsoft.Extensions.DependencyInjection; // GetRequiredService를 위해 추가
using System.Windows.Input; // ICommand를 위해 추가
using CommunityToolkit.Mvvm.Input; // RelayCommand를 위해 추가
using System.Windows;

namespace library_management_system.View
{
    public partial class LoanManagementUsercontrol : System.Windows.Controls.UserControl
    {
        // 데이터베이스 작업을 위한 Repository
        private readonly ILoanRepository _loanRepository;

        private LoanBookUserControl _loanBookControl;
        private ReturnMemberUserControl _returnMemberControl;

        // UI와 바인딩될 데이터 컬렉션
        public ObservableCollection<Loan> Loans { get; set; }

        public ObservableCollection<string> LoanSearchFilters { get; set; }
        public string SelectedLoanSearchFilter { get; set; }
        public string LoanSearchText { get; set; }
        public ICommand SearchLoanCommand { get; }
        public ICommand RefreshLoanCommand { get; }

        public LoanManagementUsercontrol()
        {
            InitializeComponent();

            _loanRepository = App.AppHost!.Services.GetRequiredService<ILoanRepository>();

            Loans = new ObservableCollection<Loan>();
            LoanSearchFilters = new ObservableCollection<string> { "이름", "전화번호" };

            SelectedLoanSearchFilter = "이름"; // 기본 검색 조건
            LoanSearchText = string.Empty;

            SearchLoanCommand = new RelayCommand(SearchLoans);
            RefreshLoanCommand = new RelayCommand(RefreshLoans);

            _loanBookControl = new LoanBookUserControl();
            _returnMemberControl = new ReturnMemberUserControl();
            this.DataContext = this;
        }

        private async void LoadLoans()
        {
            Loans.Clear();

            var loansFromDb = await _loanRepository.GetAllLoansAsync();

            foreach (var loan in loansFromDb)
            {
                Loans.Add(loan);
            }
        }

        private async void SearchLoans()
        {
            // 검색어가 비어 있으면 모든 목록을 로드
            if (string.IsNullOrWhiteSpace(LoanSearchText))
            {
                LoadLoans();
                return;
            }

            Loans.Clear();
            var searchResult = await _loanRepository.SearchLoansAsync(SelectedLoanSearchFilter, LoanSearchText);
            foreach (var loan in searchResult)
            {
                Loans.Add(loan);
            }
        }

        private void RefreshLoans()
        {
            LoanSearchText = string.Empty; // 검색창 비우기
            LoadLoans();
        }

        private void loan_book(object sender, RoutedEventArgs e)
        {
            // 'loangd' 대신 'this.Parent'를 사용하도록 수정합니다.
            var parentGrid = this.Parent as Grid;
            if (parentGrid != null)
            {
                parentGrid.Children.Clear();
                parentGrid.Children.Add(_loanBookControl);
            }
        }

        private void return_member(object sender, RoutedEventArgs e)
        {
            // 'loangd' 대신 'this.Parent'를 사용하도록 수정합니다.
            var parentGrid = this.Parent as Grid;
            if (parentGrid != null)
            {
                parentGrid.Children.Clear();
                parentGrid.Children.Add(_returnMemberControl);
            }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadLoans();
        }
    }
}