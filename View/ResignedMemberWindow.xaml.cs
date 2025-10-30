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
using System.Windows.Shapes;
using library_management_system.Models;
using library_management_system.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace library_management_system.View
{
    public partial class ResignedMemberWindow : Window
    {
        private readonly IMemberRepository _memberRepository;
        public ObservableCollection<Member> WithdrawnMembers { get; set; }

        public ResignedMemberWindow()
        {
            InitializeComponent();
            DataContext = this;
            WithdrawnMembers = new ObservableCollection<Member>();

            // DI 컨테이너에서 리포지토리 인스턴스 가져오기
            _memberRepository = App.AppHost!.Services.GetRequiredService<IMemberRepository>();

            // 창이 로드될 때 비동기적으로 데이터 로드
            Loaded += async (s, e) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var members = await _memberRepository.GetWithdrawnMembersAsync();
            WithdrawnMembers.Clear();
            foreach (var member in members)
            {
                WithdrawnMembers.Add(member);
            }
        }

        // 공개 갱신 메서드: 외부에서 호출하여 목록 재로딩 가능
        public async Task RefreshAsync()
        {
            await LoadDataAsync();
        }

        private void ResignedMember_close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}