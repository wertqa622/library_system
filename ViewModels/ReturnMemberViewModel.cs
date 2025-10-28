using System.Collections.ObjectModel;
using System.Threading.Tasks;
using library_management_system.Models;
using library_management_system.Repository;

namespace library_management_system.ViewModels // 네임스페이스는 프로젝트에 맞게 조정하세요
{
    public class ReturnMemberViewModel
    {
        private readonly ILoanRepository _loanRepository;

        // DataGrid에 바인딩될 컬렉션입니다.
        public ObservableCollection<Member> MembersWithLoans { get; } = new ObservableCollection<Member>();

        public ReturnMemberViewModel(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        // DB에서 데이터를 비동기적으로 로드하는 메서드입니다.
        public async Task LoadMembersAsync()
        {
            MembersWithLoans.Clear();
            var members = await _loanRepository.GetMembersWithActiveLoansAsync();
            foreach (var member in members)
            {
                MembersWithLoans.Add(member);
            }
        }
    }
}