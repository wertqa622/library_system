using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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

        private string _number;

        public string Number
        {
            get => _number;
            set
            {
                _number = value;
                OnPropertyChanged();
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

        public ObservableCollection<Member> Members => _members;

        public RelayCommand AddMemberCommand { get; private set; }

        public AddMemberViewModel(ObservableCollection<Member> existingMembers, IMemberRepository memberRepository)
        {
            _members = existingMembers;
            _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
            AddMemberCommand = new RelayCommand(async () => await AddMember());

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

        private void UpdateBirthdaydate()
        {
            if (!string.IsNullOrWhiteSpace(SelectedYear) &&
                !string.IsNullOrWhiteSpace(SelectedMonth) &&
                !string.IsNullOrWhiteSpace(SelectedDay))
            {
                Birthdaydate = $"{SelectedYear}-{SelectedMonth.PadLeft(2, '0')}-{SelectedDay.PadLeft(2, '0')}";
            }
        }

        private async Task AddMember()
        {
            try
            {
                // 입력 검증
                if (string.IsNullOrWhiteSpace(Name))
                {
                    MessageBox.Show("회원명을 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 새 멤버 객체 생성
                var newmember = new Member
                {
                    Name = Name?.Trim() ?? "",
                    Birthdaydate = Birthdaydate?.Trim() ?? "",
                    Gender = Gender?.Trim() ?? "",
                    Phone = Number?.Trim() ?? "",
                    Email = Email?.Trim() ?? "",
                    Photo = PhotoBytes ?? new byte[0],
                    IsActive = true // WITHDRAWALSTATUS = 'F' (false)
                };

                // 멤버 추가
                var addedMember = await _memberRepository.AddMemberAsync(newmember);

                // 데이터베이스에서 추가된 회원의 MemberID를 포함한 정보를 다시 가져옴
                _members.Add(addedMember);

                MessageBox.Show("회원이 성공적으로 추가되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);

                // 창 닫기 요청
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"회원 추가 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}