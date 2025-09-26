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
using library_management_system.Services;
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
        private IBookService _bookService;
        private IMemberService _memberService;
        private ILoanService _loanService;
        private OptimizedBookService _optimizedBookService;

        public MainWindow()
        {
            InitializeComponent();

            // DI 컨테이너에서 서비스 가져오기
            _optimizedBookService = App.AppHost!.Services.GetRequiredService<OptimizedBookService>();
            _bookService = App.AppHost!.Services.GetRequiredService<IBookService>();
            _memberService = App.AppHost!.Services.GetRequiredService<IMemberService>();
            _loanService = App.AppHost!.Services.GetRequiredService<ILoanService>();

            // ViewModel 초기화 및 DataContext 설정
            _mainViewModel = new MainViewModel(_optimizedBookService, _bookService, _memberService, _loanService);
            DataContext = _mainViewModel;
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
            var addBookWindow = new AddBookWindow(_bookService, _mainViewModel);
            addBookWindow.Owner = this;
            vbgd();
            addBookWindow.ShowDialog();
        }

        private void Modify_book(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedBook == null)
            {
                System.Windows.MessageBox.Show("수정할 도서를 먼저 선택하세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var modedifyBookWindow = new ModifyBookWindow(_bookService, _mainViewModel, _mainViewModel.SelectedBook);
            modedifyBookWindow.Owner = this;
            vbgd();
            modedifyBookWindow.ShowDialog();
        }

        private void Delete_book(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedBook == null)
            {
                System.Windows.MessageBox.Show("삭제할 도서를 먼저 선택하세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // MainViewModel의 DeleteBook 메서드 호출 (확인 대화상자 포함)
            _mainViewModel.DeleteBookCommand.Execute(null);
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
            AddBookWindow addbook = new AddBookWindow(_bookService, _mainViewModel);
            vbgd();
            addbook.ShowDialog();
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
            if (_mainViewModel.SelectedMember == null)
            {
                System.Windows.MessageBox.Show("수정할 회원을 먼저 선택하세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var modedifyMemberWindow = new ModifyMemberWindow();
            vbgd();
            modedifyMemberWindow.ShowDialog();
        }

        #endregion 화면 부가적 기능 메서드
    }
}