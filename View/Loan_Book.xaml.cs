using library_management_system.DataBase;
using library_management_system.Models;
using library_management_system.Repository;
using library_management_system.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
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
using static System.Reflection.Metadata.BlobBuilder;

namespace library_management_system.View
{
    /// <summary>
    /// Book_Info.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Loan_Book : Window
    {
        private readonly ILoanRepository _loanRepository;
        private readonly string _phoneNumber;
        public ObservableCollection<LoanBookViewModel> Books { get; set; }

        // 반납 완료 후 상위 창에 갱신 요청 이벤트
        public event EventHandler<string>? BookListChanged;

        public Loan_Book(string phoneNumber)
        {
            InitializeComponent();
            _loanRepository = App.AppHost!.Services.GetRequiredService<ILoanRepository>();
            _phoneNumber = phoneNumber;
            Books = new ObservableCollection<LoanBookViewModel>();
            this.DataContext = this;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadLoanBooksAsync();
        }

        private async Task LoadLoanBooksAsync()
        {
            Books.Clear();
            var booksFromDb = await _loanRepository.GetActiveLoansByPhoneAsync(_phoneNumber);
            // null 확인 추가
            if (booksFromDb == null)
            {
                System.Windows.MessageBox.Show("조회된 도서가 없습니다.");
                return;
            }

            foreach (var item in booksFromDb)
            {
                if (item != null)  // 혹시라도 null인 경우 방지
                    Books.Add(item);
            }

            //System.Windows.MessageBox.Show($"조회된 도서 수: {Books.Count}");
        }

        // 반납 버튼 클릭
        private async void ReturnBook_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            var selectedBook = button?.DataContext as LoanBookViewModel;

            if (selectedBook == null)
            {
                System.Windows.MessageBox.Show("선택된 도서 정보가 없습니다.");
                return;
            }

            bool result = await _loanRepository.ReturnBookAsync(selectedBook.LoanId);

            if (result)
            {
                System.Windows.MessageBox.Show($"도서 '{selectedBook.BookName}'이(가) 반납되었습니다.");
                // DB에서 최신 데이터 다시 불러오기
                await LoadLoanBooksAsync();

                // 책이 하나도 남지 않으면 상위 창에 알림
                if (Books.Count == 0)
                    BookListChanged?.Invoke(this, _phoneNumber);
            }
            else
            {
                System.Windows.MessageBox.Show("반납 처리 중 오류가 발생했습니다.");
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchTextBox.Text.Trim();

            // 검색어가 비어 있으면 전체 목록을 다시 로드
            if (string.IsNullOrWhiteSpace(keyword))
            {
                await LoadLoanBooksAsync();
                return;
            }

            try
            {
                Books.Clear();
                // 특정 회원의 대출 목록 내에서 검색하는 리포지토리 메서드 호출
                var searchResult = await _loanRepository.SearchActiveLoansAsync(_phoneNumber, keyword);

                if (searchResult != null)
                {
                    foreach (var book in searchResult)
                    {
                        Books.Add(book);
                    }
                }

                if (Books.Count == 0)
                {
                    System.Windows.MessageBox.Show("검색 결과가 없습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"도서 검색 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void loanbook_close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}