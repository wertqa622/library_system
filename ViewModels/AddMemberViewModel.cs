using System;
using System.Collections.ObjectModel;
using System.Windows;
using library_management_system.Models;
using library_management_system.Repository;

namespace library_management_system.ViewModels
{
    public class AddMemberViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Member> _members;
        private readonly IMemberRepository _memberRepository;

        public string Name { get; set; }
        public string Birthdaydate { get; set; }
        public string Gender { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }

        public ObservableCollection<Member> Members => _members;

        public AddMemberViewModel(ObservableCollection<Member> existingMembers, IMemberRepository memberRepository)
        {
            _members = existingMembers;
            _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        }
    }
}