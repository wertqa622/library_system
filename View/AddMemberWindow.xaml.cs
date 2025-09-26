using library_management_system.Models;

using library_management_system.ViewModels;
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
    /// <summary>
    /// AddMemberWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddMemberWindow : Window
    {

        private readonly MainViewModel _mainViewModel;

        public AddMemberWindow()
        {
            InitializeComponent();
        }

        private void wndqhr_click(object sender, RoutedEventArgs e)
        {
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 버튼 비활성화 (중복 클릭 방지)
                var addButton = sender as System.Windows.Controls.Button;
                var cancelButton = this.FindName("CancelButton") as System.Windows.Controls.Button;

                if (addButton != null) addButton.IsEnabled = false;
                if (cancelButton != null) cancelButton.IsEnabled = false;




            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"도서 추가 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // 버튼 다시 활성화
                var addButton = sender as System.Windows.Controls.Button;
                var cancelButton = this.FindName("CancelButton") as System.Windows.Controls.Button;

                if (addButton != null) addButton.IsEnabled = true;
                if (cancelButton != null) cancelButton.IsEnabled = true;
            }
        }
    }
}