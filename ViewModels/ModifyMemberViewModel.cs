using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public event Action RequestClose;

        // [추가된 부분] 생년월일/성별 관련 속성들
        public List<string> GenderOptions { get; private set; }

        public List<string> YearOptions { get; private set; }
        public List<string> MonthOptions { get; private set; }
        public List<string> DayOptions { get; private set; }

        private string _name;
        private string _phone;
        private string _email;
        private byte[] _photoBytes;
        private string _gender;
        private string _birthdaydate;
        private string _selectedYear;
        private string _selectedMonth;
        private string _selectedDay;

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public byte[] PhotoBytes { get => _photoBytes; set => SetProperty(ref _photoBytes, value); }
        public string Gender { get => _gender; set => SetProperty(ref _gender, value); }
        public string Birthdaydate { get => _birthdaydate; private set => SetProperty(ref _birthdaydate, value); }

        public string SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (SetProperty(ref _selectedYear, value))
                    UpdateBirthdaydate();
            }
        }

        public string SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (SetProperty(ref _selectedMonth, value))
                    UpdateBirthdaydate();
            }
        }

        public string SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (SetProperty(ref _selectedDay, value))
                    UpdateBirthdaydate();
            }
        }

        public ICommand UpdateMemberCommand { get; private set; }

        public ModifyMemberViewModel(Member member, IMemberRepository memberRepository)
        {
            _originalMember = member ?? throw new ArgumentNullException(nameof(member));
            _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));

            // 기존 회원 정보로 속성 초기화
            Name = member.Name ?? "";
            Phone = member.Phone ?? "";
            Email = member.Email ?? "";
            PhotoBytes = member.Photo ?? Array.Empty<byte>();
            Gender = member.Gender ?? ""; // 성별 초기화

            // [추가된 부분] 콤보박스 옵션 초기화 (AddMemberViewModel과 동일)
            GenderOptions = new List<string> { "남자", "여자" };
            YearOptions = new List<string> { "" };
            MonthOptions = new List<string> { "" };
            DayOptions = new List<string> { "" };

            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= 1900; year--)
                YearOptions.Add(year.ToString());

            for (int month = 1; month <= 12; month++)
                MonthOptions.Add(month.ToString());

            for (int day = 1; day <= 31; day++)
                DayOptions.Add(day.ToString());

            // [핵심 로직] 기존 생년월일 정보를 파싱하여 콤보박스 초기값 설정
            InitializeBirthday(member.Birthdaydate);

            UpdateMemberCommand = new RelayCommand(UpdateMember);
        }

        // [추가된 부분] 생년월일 문자열을 파싱하여 SelectedYear/Month/Day를 설정하는 메서드
        private void InitializeBirthday(string birthday)
        {
            if (string.IsNullOrWhiteSpace(birthday)) return;

            try
            {
                string[] parts = birthday.Split('-');
                if (parts.Length == 3)
                {
                    _selectedYear = parts[0];
                    // "07" -> "7" 처럼 앞에 0이 붙은 경우 제거
                    _selectedMonth = int.Parse(parts[1]).ToString();
                    _selectedDay = int.Parse(parts[2]).ToString();

                    // UI에 변경사항을 알리기 위해 OnPropertyChanged 호출
                    //OnPropertyChanged(nameof(SelectedYear));
                    //OnPropertyChanged(nameof(SelectedMonth));
                    //OnPropertyChanged(nameof(SelectedDay));
                }
            }
            catch
            {
                // 날짜 형식이 잘못된 경우 무시
            }
        }

        // [추가된 부분] 콤보박스 선택 시 Birthdaydate 속성을 업데이트하는 메서드
        private void UpdateBirthdaydate()
        {
            if (!string.IsNullOrWhiteSpace(SelectedYear) &&
                !string.IsNullOrWhiteSpace(SelectedMonth) &&
                !string.IsNullOrWhiteSpace(SelectedDay))
            {
                Birthdaydate = $"{SelectedYear}-{SelectedMonth.PadLeft(2, '0')}-{SelectedDay.PadLeft(2, '0')}";
            }
            else
            {
                Birthdaydate = "";
            }
        }

        private async void UpdateMember()
        {
            UpdateBirthdaydate();
            //InitializeBirthday(Birthdaydate);
            try
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    MessageBox.Show("회원명을 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // [수정된 부분] 업데이트할 정보에 생년월일과 성별 추가
                _originalMember.Name = Name?.Trim() ?? "";
                _originalMember.Birthdaydate = Birthdaydate?.Trim() ?? "";
                _originalMember.Gender = Gender?.Trim() ?? "";
                _originalMember.Phone = Phone?.Trim() ?? "";
                _originalMember.Email = Email?.Trim() ?? "";
                _originalMember.Photo = PhotoBytes ?? Array.Empty<byte>();

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