using System;
using System.Collections.Generic;
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

namespace library_management_system.View
{
    public partial class ResignedMemberWindow : Window
    {
        public ResignedMemberWindow()
        {
            InitializeComponent();
        }

        // 공개 갱신 메서드: 외부에서 호출하여 목록 재로딩 가능
        public async Task RefreshAsync()
        {
            await LoadDataAsync();
        }

        private void ResignedMember_close(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Application.Current.MainWindow is MainWindow main)
            {
                main.hdgd();
            }
            DialogResult = false;
            this.Close();
        }
    }
}