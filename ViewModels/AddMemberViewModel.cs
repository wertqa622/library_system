using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using library_management_system.Models;
using library_management_system.Repository;
using MessageBox = System.Windows.MessageBox;

namespace library_management_system.ViewModels
{
    public class AddMemberViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Member> _members;
        private readonly IMemberRepository _memberRepository;

        public event Action RequestClose;

        public List<string> GenderOptions { get; private set; }
        public List<string> YearOptions { get; private set; }
        public List<string> MonthOptions { get; private set; }
        public List<string> DayOptions { get; private set; }

        public long phone { get; set; }

        private string _name;
        private string _selectedYear;
        private string _selectedMonth;
        private string _selectedDay;

        public string SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged();
                UpdateBirthdaydate();
            }
        }

        public string SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                _selectedMonth = value;
                OnPropertyChanged();
                UpdateBirthdaydate();
            }
        }

        public string SelectedDay
        {
            get => _selectedDay;
            set
            {
                _selectedDay = value;
                OnPropertyChanged();
                UpdateBirthdaydate();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _birthdaydate;

        public string Birthdaydate
        {
            get => _birthdaydate;
            set
            {
                _birthdaydate = value;
                OnPropertyChanged();
            }
        }

        private string _gender;

        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        private string _Phone;

        public string Phone
        {
            get => _Phone;
            set
            {
                if (SetProperty(ref _Phone, value))
                {
                    CheckPhoneFlag = true;
                }
            }
        }

        private string _email;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _imagePath;

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        private byte[] _photoBytes;

        public byte[] PhotoBytes
        {
            get => _photoBytes;
            set
            {
                _photoBytes = value;
                OnPropertyChanged();
            }
        }

        private bool _checkPhoneFlag;

        public bool CheckPhoneFlag
        {
            get => _checkPhoneFlag;
            private set => SetProperty(ref _checkPhoneFlag, value);
        }

        public ObservableCollection<Member> Members => _members;

        public RelayCommand AddMemberCommand { get; private set; }
        public ICommand CheckPhoneCommand { get; }

        public AddMemberViewModel(ObservableCollection<Member> existingMembers, IMemberRepository memberRepository)
        {
            _members = existingMembers;
            _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
            AddMemberCommand = new RelayCommand(async () => await AddMember());
            CheckPhoneCommand = new RelayCommand(CheckPhone);
            CheckPhoneFlag = true;
            // 성별 옵션 초기화
            GenderOptions = new List<string> { "남자", "여자" };

            // 생년월일 옵션 초기화
            YearOptions = new List<string> { "" };
            MonthOptions = new List<string> { "" };
            DayOptions = new List<string> { "" };

            // 년도 (1900 ~ 현재년도)
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= 1900; year--)
            {
                YearOptions.Add(year.ToString());
            }

            // 월 (1 ~ 12)
            for (int month = 1; month <= 12; month++)
            {
                MonthOptions.Add(month.ToString());
            }

            // 일 (1 ~ 31)
            for (int day = 1; day <= 31; day++)
            {
                DayOptions.Add(day.ToString());
            }
        }

        private async void CheckPhone(object parameter)
        {
            string phone = this.Phone?.Trim(); //데이터가 null일 경우 대비

            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("휴대폰 번호를 입력해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var existingMember = await _memberRepository.GetMemberByPhoneAsync(phone);
                if (existingMember == null)
                {
                    // DB에 없음 → 진행 가능
                    MessageBox.Show("사용 가능한 휴대폰 번호입니다.", "사용 가능", MessageBoxButton.OK, MessageBoxImage.Information);
                    CheckPhoneFlag = false;
                    return;
                }

                // existingMember가 있고, WithdrawalStatus가 'T'인 경우 → 우리 모델에서 IsActive == false
                if (!existingMember.IsActive)
                {
                    MessageBox.Show("사용가능한 번호입니다.", "안내", MessageBoxButton.OK, MessageBoxImage.Information);
                    CheckPhoneFlag = false;
                    return;
                }

                // existingMember가 있고, WithdrawalStatus가 'F'(활성) → 중복으로 간주
                MessageBox.Show("중복값입니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                CheckPhoneFlag = true;
            }
            catch
            {
                MessageBox.Show("휴대폰 번호 확인 중 오류가 발생했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<AddMemberViewModel> checkdbphone(AddMemberViewModel phone)
        {
            var existingMember = await _memberRepository.GetMemberByPhoneAsync(phone.Phone);
            if (existingMember != null)
            {
                MessageBox.Show("이미 존재하는 휴대폰 번호입니다.", "중복 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                CheckPhoneFlag = true;
            }
            else
            {
                MessageBox.Show("사용 가능한 휴대폰 번호입니다.", "사용 가능", MessageBoxButton.OK, MessageBoxImage.Information);
                CheckPhoneFlag = false;
            }
            return default;
        }

        private void UpdateBirthdaydate()
        {
            if (!string.IsNullOrWhiteSpace(SelectedYear) &&
                !string.IsNullOrWhiteSpace(SelectedMonth) &&
                !string.IsNullOrWhiteSpace(SelectedDay))
            {
                Birthdaydate = $"{SelectedYear}-{SelectedMonth.PadLeft(2, '0')}-{SelectedDay.PadLeft(2, '0')}";
            }
        }

        /// <summary>
        /// 회원 추가 로직
        /// </summary>
        private async Task AddMember()
        {
            // --- 입력 검증 ---
            if (CheckPhoneFlag) // 중복 확인을 통과했는지 먼저 검사
            {
                MessageBox.Show("휴대폰 번호 중복 체크를 확인해주세요.", "중복 확인 필요", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Phone))
            {
                MessageBox.Show("필수 항목(이름, 휴대폰 번호)을 모두 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 최종 저장 시: 해당 전화번호로 기존 레코드 무조건 삭제 후 재삽입
                var trimmedPhone = this.Phone.Trim();
                await _memberRepository.DeleteByPhoneAsync(trimmedPhone);

                var newMember = new Member
                {
                    Name = this.Name.Trim(),
                    Birthdaydate = this.Birthdaydate?.Trim() ?? "",
                    Gender = this.Gender?.Trim() ?? "",
                    Phone = trimmedPhone,
                    Email = this.Email?.Trim() ?? "",
                    Photo = this.PhotoBytes ?? new byte[0],
                    IsActive = true
                };

                var addedMember = await _memberRepository.AddMemberAsync(newMember);
                _members.Add(addedMember); // UI 목록에 추가

                MessageBox.Show("회원이 성공적으로 추가되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
                RequestClose?.Invoke(); // 창 닫기 요청
            }
            catch
            {
                MessageBox.Show("회원 추가 중 오류가 발생했습니다: 입력한 값을 확인해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}