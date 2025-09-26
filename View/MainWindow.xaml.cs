using System.Text;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using library_management_system;
using library_management_system.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using library_management_system.View;

namespace library_management_system
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _mainViewModel;
 

        public MainWindow()
        {
            InitializeComponent();

            // DI 컨테이너에서 서비스 가져오기
         
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //LoanManagementUsercontrol LMU = new LoanManagementUsercontrol();
            //loangd.Children.Clear();
            //loangd.Children.Add(LMU);
        }

        #region 도서 관리

        private void Add_book(object sender, RoutedEventArgs e)
        {
          
        }

        private void Modify_book(object sender, RoutedEventArgs e)
        {
           
        }

        private void Delete_book(object sender, RoutedEventArgs e)
        {
        
        }

        private void Search_book(object sender, RoutedEventArgs e)
        {
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
        }

        private void Search_member(object sender, RoutedEventArgs e)
        {
        }

        private void Info_click(object sender, RoutedEventArgs e)
        {
            Book_Info a = new Book_Info();
            a.Show();
        }

        #endregion 도서 관리

        #region 대출관리

        private void loan_book(object sender, RoutedEventArgs e)
        {
            var loan = new LoanBookUserControl();
            loangd.Children.Clear();
            loangd.Children.Add(loan);
        }

        #endregion 대출관리

        #region 고객 관리

        private void Add_Member(object sender, RoutedEventArgs e)
        {
          
        }

        private void ResignedMember_Click(object sender, RoutedEventArgs e)
        {
            ResignedMemberWindow aaa = new ResignedMemberWindow();
            vbgd();
            aaa.ShowDialog();
        }

        #endregion 고객 관리

        #region 대출관리

        private void LoanHeader_Click(object sender, RoutedEventArgs e)
        {
            LoanManagementUsercontrol LMU = new LoanManagementUsercontrol();
            loangd.Children.Clear();
            loangd.Children.Add(LMU);
        }

        #endregion 대출관리

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }

        #region 화면 부가적 기능 메서드

        public void vbgd()
        {
            hiddengd.Opacity = 0.5;
            hiddengd.Visibility = Visibility.Visible;
        }

        public void hdgd()
        {
            hiddengd.Visibility = Visibility.Collapsed;
        }

        private void Modify_Member(object sender, RoutedEventArgs e)
        {
           
        }

        #endregion 화면 부가적 기능 메서드
    }
}