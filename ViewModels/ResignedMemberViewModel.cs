using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using library_management_system.Models;
using library_management_system.Repository;

namespace library_management_system.ViewModels
{
    public class ResignedMemberViewModel
    {
        private readonly IMemberRepository _memberRepository;

        public ObservableCollection<Member> Members { get; } = new ObservableCollection<Member>();

        public ResignedMemberViewModel(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task LoadAsync()
        {
            Members.Clear();
            var all = await _memberRepository.GetAllMembersAsync();
            foreach (var m in all.Where(x => x.WithdrawalStatus))
            {
                Members.Add(m);
            }
        }
    }
}