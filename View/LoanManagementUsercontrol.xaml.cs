using System.Windows.Controls;
using System.Collections.ObjectModel; // ObservableCollection을 위해 추가
using library_management_system.Models;
using library_management_system.Repository; // ILoanRepository를 위해 추가
using Microsoft.Extensions.DependencyInjection; // GetRequiredService를 위해 추가

namespace library_management_system.View
{
    public partial class LoanManagementUsercontrol : System.Windows.Controls.UserControl
    {
        // 데이터베이스 작업을 위한 Repository
        private readonly ILoanRepository _loanRepository;

        // UI와 바인딩될 데이터 컬렉션
        public ObservableCollection<Loan> Loans { get; set; }

        public LoanManagementUsercontrol()
        {
            InitializeComponent();

            _loanRepository = App.AppHost!.Services.GetRequiredService<ILoanRepository>();

            Loans = new ObservableCollection<Loan>();

            this.DataContext = this;
        }

        private async void LoadLoans()
        {
            // 기존 데이터를 비웁니다.
            Loans.Clear();

            // Repository를 통해 DB에서 모든 대출 정보를 가져옵니다.
            var loansFromDb = await _loanRepository.GetAllLoansAsync();

            // 가져온 데이터를 하나씩 Loans 컬렉션에 추가합니다.
            // ObservableCollection은 데이터가 추가될 때마다 UI를 자동으로 갱신합니다.
            foreach (var loan in loansFromDb)
            {
                Loans.Add(loan);
            }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadLoans();
        }
    }
}