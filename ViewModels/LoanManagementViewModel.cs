using System.Collections.ObjectModel;
using System.Threading.Tasks;
using library_management_system.Models;
using library_management_system.Repository;

// ViewModelBase가 있는 네임스페이스를 using 해야 합니다.
// 예를 들어: using library_management_system.ViewModels.Base; 

namespace library_management_system.ViewModels
{
    // 'ViewModelBase'는 프로젝트에 있는 기본 ViewModel 클래스 이름으로 바꿔야 할 수 있습니다.
    public class LoanManagementViewModel : ViewModelBase
    {
        private readonly ILoanRepository _loanRepository;
        public ObservableCollection<Loan> Loans { get; set; }

        private Loan _selectedLoan; // 선택된 Loan을 저장할 private 변수

        public Loan SelectedLoan
        {
            get { return _selectedLoan; }
            set
            {
                _selectedLoan = value;
                // 'OnPropertyChanged'는 ViewModelBase에 구현된 메서드여야 합니다.
                // 속성 값이 변경되었음을 UI에 알리는 역할을 합니다.
                OnPropertyChanged(nameof(SelectedLoan));
            }
        }
        // 생성자: ILoanRepository를 외부에서 주입받습니다.
        public LoanManagementViewModel(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
            Loans = new ObservableCollection<Loan>();
            LoadLoans(); // ViewModel이 생성될 때 데이터를 로드합니다.
        }

        private async Task LoadLoans()
        {
            Loans.Clear();
            var loansFromDb = await _loanRepository.GetAllLoansAsync();

            if (loansFromDb != null)
            {
                foreach (var loan in loansFromDb)
                {
                    Loans.Add(loan);
                }
            }
        }
    }
}