// View/Loan_Book.xaml.cs 파일을 아래 코드로 전체 교체하세요.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using library_management_system.Models;
using library_management_system.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace library_management_system.View
{
    public partial class Loan_Book : Window
    {
        private readonly ILoanRepository _loanRepository;
        private readonly string _memberPhoneNumber;

        // DataGrid에 바인딩할 컬렉션. UI 자동 업데이트를 위해 ObservableCollection<Loan> 사용
        public ObservableCollection<Loan> LoanedBooks { get; set; }

        // 생성자에서 전화번호만 받도록 수정
        public Loan_Book(string phoneNumber)
        {
            InitializeComponent();
            DataContext = this; // 데이터 바인딩의 기준을 이 클래스 자신으로 설정

            _memberPhoneNumber = phoneNumber;
            // DI 컨테이너를 통해 ILoanRepository의 인스턴스를 직접 가져옵니다.
            _loanRepository = App.AppHost!.Services.GetRequiredService<ILoanRepository>();

            LoanedBooks = new ObservableCollection<Loan>();

            // 창이 화면에 로드된 후, 데이터를 비동기적으로 불러와 UI 멈춤 현상을 방지합니다.
            Loaded += async (s, e) => await LoadLoanedBooksAsync();
        }

        private async Task LoadLoanedBooksAsync()
        {
            try
            {
                // Repository에 구현된 메서드를 호출하여 데이터를 가져옵니다.
                var books = await _loanRepository.GetActiveLoansWithBookDetailsAsync(_memberPhoneNumber);

                LoanedBooks.Clear();
                foreach (var book in books)
                {
                    LoanedBooks.Add(book);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"대출 목록을 불러오는 중 오류가 발생했습니다.\n{ex.Message}");
            }
        }

        private async void returnbook_Click(object sender, RoutedEventArgs e)
        {
            // 클릭된 버튼이 속한 행의 Loan 객체를 가져옵니다.
            var selectedLoan = (sender as FrameworkElement)?.DataContext as Loan;
            if (selectedLoan == null) return;

            var result = System.Windows.MessageBox.Show($"'{selectedLoan.BookName}' 도서를 반납하시겠습니까?", "반납 확인", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // 1. Repository의 반납 메서드를 호출하여 DB를 업데이트합니다.
                    await _loanRepository.ReturnBookAsync(selectedLoan.LoanId);

                    // 2. ObservableCollection에서 해당 항목을 제거하면 UI가 자동으로 업데이트됩니다.
                    LoanedBooks.Remove(selectedLoan);

                    System.Windows.MessageBox.Show("성공적으로 반납되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"반납 처리 중 오류가 발생했습니다.\n{ex.Message}");
                }
            }
        }

        private void loanbook_close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}