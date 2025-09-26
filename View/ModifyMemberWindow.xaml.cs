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
    /// ModifyMemberWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModifyMemberWindow : Window
    {
        public ModifyMemberWindow()
        {
            InitializeComponent();
            Window_Loaded();
        }

        private void Window_Loaded()
        {
        }

        private void wndqhr_click(object sender, RoutedEventArgs e)
        {
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Application.Current.MainWindow is MainWindow main)
            {
                main.hdgd();
            }
            DialogResult = false;
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}