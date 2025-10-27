using System;
using System.Windows;
using System.Windows.Input;
using library_management_system.Models;
using library_management_system.Repository;
using MessageBox = System.Windows.MessageBox;

namespace library_management_system.ViewModels
{
    public class ModifyMemberViewModel : ViewModelBase
    {
        private readonly IMemberRepository _memberRepository;
        private readonly Member _originalMember;
        private byte[] _photoBytes;

        public event Action RequestClose;

        public string Name { get; set; }
        public string Birthdaydate { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public byte[] PhotoBytes
        {
            get => _photoBytes;
            set
            {
                _photoBytes = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateMemberCommand { get; private set; }

        public ModifyMemberViewModel(Member member, IMemberRepository memberRepository)
        {
            _originalMember = member ?? throw new System.ArgumentNullException(nameof(member));
            _memberRepository = memberRepository ?? throw new System.ArgumentNullException(nameof(memberRepository));

            // 기존 회원 정보로 초기화
            Name = member.Name ?? "";
            Birthdaydate = member.Birthdaydate ?? "";
            Gender = member.Gender ?? "";
            Phone = member.Phone ?? "";
            Email = member.Email ?? "";
            PhotoBytes = member.Photo ?? new byte[0];

            UpdateMemberCommand = new RelayCommand(UpdateMember);
        }

        private async void UpdateMember()
        {
            try
            {
                // 입력 검증
                if (string.IsNullOrWhiteSpace(Name))
                {
                    MessageBox.Show("회원명을 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 회원 정보 업데이트
                _originalMember.Name = Name?.Trim() ?? "";
                _originalMember.Birthdaydate = Birthdaydate?.Trim() ?? "";
                _originalMember.Gender = Gender?.Trim() ?? "";
                _originalMember.Phone = Phone?.Trim() ?? "";
                _originalMember.Email = Email?.Trim() ?? "";
                _originalMember.Photo = PhotoBytes ?? new byte[0];

                // DB 업데이트
                await _memberRepository.UpdateMemberAsync(_originalMember);

                MessageBox.Show("회원 정보가 성공적으로 수정되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);

                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"회원 수정 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}