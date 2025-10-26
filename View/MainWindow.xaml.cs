using System.Windows;
using library_management_system.Repository;
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
        private IBookRepository _bookRepository;
        private IMemberRepository _memberRepository;
        private ILoanRepository _loanRepository;
        private ReturnMemberUserControl _returnMemberControl;

        public MainWindow()
        {
            InitializeComponent();

            // DI 컨테이너에서 서비스 가져오기
            _bookRepository = App.AppHost!.Services.GetRequiredService<IBookRepository>();
            _memberRepository = App.AppHost!.Services.GetRequiredService<IMemberRepository>();
            _loanRepository = App.AppHost!.Services.GetRequiredService<ILoanRepository>();

            // ViewModel 초기화 및 DataContext 설정
            _mainViewModel = new MainViewModel(_bookRepository, _memberRepository, _loanRepository);
            DataContext = _mainViewModel;

            _returnMemberControl = new ReturnMemberUserControl();
        }

        #region 도서 관리

        private void Search_book(object sender, RoutedEventArgs e)
        {
        }

        private void Search_member(object sender, RoutedEventArgs e)
        {
        }

        private void Info_click(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedBook == null)
            {
                System.Windows.MessageBox.Show("도서를 선택해주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            vbgd();
            Book_Info bookInfoWindow = new Book_Info(_mainViewModel.SelectedBook);
            bookInfoWindow.ShowDialog();
        }

        #endregion 도서 관리

        #region 대출관리

        private void loan_book(object sender, RoutedEventArgs e)
        {
            var loan = new LoanBookUserControl();
            loangd.Children.Clear();
            loangd.Children.Add(loan);
        }

        private void return_member(object sender, RoutedEventArgs e)
        {
            // loangd의 컨텐츠를 모두 지우고
            loangd.Children.Clear();
            // 도서 반납 UserControl을 추가
            loangd.Children.Add(_returnMemberControl);
        }

        #endregion 대출관리



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

        #endregion 화면 부가적 기능 메서드
    }
}