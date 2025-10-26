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
using library_management_system.Repository;
using library_management_system;
using library_management_system.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using library_management_system.View;
using System.Threading.Tasks;

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
        private LoanBookUserControl _loanBookControl;
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

            _loanBookControl = new LoanBookUserControl();
            _returnMemberControl = new ReturnMemberUserControl();
        }



        #region 도서 관리

        private void Add_book(object sender, RoutedEventArgs e)
        {
            var addBookWindow = new AddBookWindow(_bookRepository, _mainViewModel);
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
            var modedifyBookWindow = new ModifyBookWindow(_bookRepository, _mainViewModel, _mainViewModel.SelectedBook);
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
            // loangd의 컨텐츠를 모두 지우고
            loangd.Children.Clear();
            // 도서 대출 UserControl을 추가
            loangd.Children.Add(_loanBookControl);
        }

        // "도서 반납" 버튼 클릭 이벤트

        #endregion 대출관리

        #region 고객 관리

        private void Add_Member(object sender, RoutedEventArgs e)
        {
            AddBookWindow addbook = new AddBookWindow(_bookRepository, _mainViewModel);
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

        private LoanManagementUsercontrol _loanManagementControl;
        private void LoanHeader_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // UserControl이 아직 생성되지 않았다면 새로 만듭니다. (최초 한 번만 실행됨)
            if (_loanManagementControl == null)
            {
                _loanManagementControl = new LoanManagementUsercontrol();
            }

            // loangd Grid의 내용을 모두 비우고
            loangd.Children.Clear();
            // 준비된 UserControl을 Grid에 추가하여 화면에 보여줍니다.
            loangd.Children.Add(_loanManagementControl);
        }
        private void return_member(object sender, RoutedEventArgs e)
        {
            // loangd의 컨텐츠를 모두 지우고
            loangd.Children.Clear();
            // 도서 반납 UserControl을 추가
            loangd.Children.Add(_returnMemberControl);
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}