using library_management_system.Models;
using library_management_system.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace library_management_system.View
{
    /// <summary>
    /// 회원 목록에서 도서 대출을 시작하는 UserControl
    /// </summary>
    public partial class LoanBookUserControl : System.Windows.Controls.UserControl
    {
        private readonly IMemberRepository _memberRepository;

        // UI와 바인딩될 전체 회원 목록
        public ObservableCollection<Member> Members { get; set; }

        public LoanBookUserControl()
        {
            InitializeComponent();

            // 의존성 주입을 통해 Repository 인스턴스를 가져옵니다.
            _memberRepository = App.AppHost!.Services.GetRequiredService<IMemberRepository>();

            Members = new ObservableCollection<Member>();
            this.DataContext = this;
        }

        // 컨트롤이 로드될 때 모든 회원 목록을 불러옵니다.
        public async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllMembersAsync();
        }

        // DB에서 모든 회원 정보를 비동기적으로 로드하는 메서드
        public async Task LoadAllMembersAsync()
        {
            Members.Clear();

            // 모든 회원 정보를 조회하는 Repository 메서드를 호출합니다.
            var allMembers = await _memberRepository.GetAllMembersAsync();

            if (allMembers != null)
            {
                foreach (var member in allMembers)
                {
                    Members.Add(member);
                }
            }
        }

        // '대출' 버튼 클릭 시
        private void StartLoan_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: Member selectedMember })
            {
                // Return_Book 창을 열고, 선택된 회원 정보를 전달합니다.
                var loanWindow = new Return_Book(selectedMember);
                loanWindow.Owner = Window.GetWindow(this);

                // 대출 창(Return_Book)에서 대출이 성공하면 BookLoaned 이벤트가 발생합니다.
                loanWindow.BookLoaned += async (s, args) =>
                {
                    // 대출 후 회원 상태가 변경될 수 있으므로, 전체 회원 목록을 다시 불러옵니다.
                    await LoadAllMembersAsync();
                };

                loanWindow.ShowDialog();
            }
        }
    }
}