using library_management_system.Models;
using library_management_system.Repository;
using library_management_system.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace library_management_system.View
{
    /// <summary>
    /// LoanBookUserControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReturnMemberUserControl : System.Windows.Controls.UserControl
    {
        private readonly IMemberRepository _memberRepository;

        // 대출 중인 회원 목록
        public ObservableCollection<Member> Members { get; set; }

        public ReturnMemberUserControl()
        {
            InitializeComponent();

            _memberRepository = App.AppHost!.Services.GetRequiredService<IMemberRepository>();

            Members = new ObservableCollection<Member>();

            this.DataContext = this;
        }

        // 컨트롤이 로드될 때 대출 중인 회원 목록 불러오기
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Members.Clear();

            var membersFromDb = await _memberRepository.GetMembersWithActiveLoansAsync();

            //System.Windows.MessageBox.Show($"조회된 회원 수: {membersFromDb.Count()}");

            foreach (var member in membersFromDb)
            {
                Members.Add(member);
            }
        }

        // 선택된 회원의 대출 도서 목록 창 띄우기
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            var selectedMember = (sender as FrameworkElement)?.DataContext as Member;
            if (selectedMember == null) return;

                // 현재 창을 부모로 설정하여 모달 대화상자로 엽니다.
                loanWindow.Owner = Window.GetWindow(this);
                loanWindow.ShowDialog();

            // Loan_Book 창에서 도서 반납 완료 시 이벤트 수신
            window.BookListChanged += async (s, phoneNumber) =>
            {
                // DB에서 최신 대출 중 회원 목록 다시 조회
                var membersFromDb = await _memberRepository.GetMembersWithActiveLoansAsync();
                Members.Clear();
                foreach (var member in membersFromDb)
                    Members.Add(member);
            };
            window.ShowDialog();
        }
    }
}