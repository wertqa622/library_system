using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using library_management_system.Models;
using library_management_system.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace library_management_system.View
{
    public partial class Return_Book : Window
    {
        private readonly IReturnRepository _repository;
        private readonly Member _selectedMember;

        // XAML의 DataGrid와 바인딩될 검색 결과 컬렉션
        public ObservableCollection<Book> SearchResults { get; set; }

        // 대출 성공 후, 부모 창(ReturnMemberUserControl)에 갱신을 요청하는 이벤트
        public event EventHandler? BookLoaned;

        public Return_Book(Member selectedMember)
        {
            InitializeComponent();
            _selectedMember = selectedMember;

            // 1. 의존성 주입을 통해 Repository 인스턴스를 가져옵니다.
            _repository = App.AppHost!.Services.GetRequiredService<IReturnRepository>();

            // 2. ObservableCollection을 초기화하고 DataContext를 설정합니다.
            SearchResults = new ObservableCollection<Book>();
            this.DataContext = this;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAvailableBooksAsync();
        }

        // 👇 데이터를 불러오는 로직을 별도 메서드로 분리
        private async Task LoadAvailableBooksAsync()
        {
            try
            {
                var books = await _repository.GetAvailableBooksAsync();
                SearchResults.Clear();
                if (books != null)
                {
                    foreach (var book in books)
                    {
                        SearchResults.Add(book);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"도서 목록을 불러오는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        // 검색 버튼 클릭 이벤트
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchTextBox.Text;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                await LoadAvailableBooksAsync();
                return;
            }
            if (string.IsNullOrWhiteSpace(keyword))
            {
                System.Windows.MessageBox.Show("검색어를 입력해주세요.");
                return;
            }

            try
            {
                var books = await _repository.SearchBooksAsync(keyword);

                // 3. 컬렉션을 직접 수정하여 UI를 갱신합니다.
                SearchResults.Clear();
                if (books != null)
                {
                    foreach (var book in books)
                    {
                        SearchResults.Add(book);
                    }
                }

                if (SearchResults.Count == 0)
                {
                    System.Windows.MessageBox.Show("검색 결과가 없습니다.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"도서 검색 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        // 대출 버튼 클릭 이벤트
        private async void LoanButton_Click(object sender, RoutedEventArgs e)
        {
            // 버튼의 DataContext를 통해 선택된 Book 객체를 가져옵니다.
            if (sender is System.Windows.Controls.Button button && button.DataContext is Book selectedBook)
            {
                var result = System.Windows.MessageBox.Show(
                    $"회원: {_selectedMember.Name}\n도서: {selectedBook.BookName}\n\n이 도서를 대출하시겠습니까?",
                    "대출 확인",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _repository.LoanBookAsync(_selectedMember.Phone, selectedBook.ISBN);
                        System.Windows.MessageBox.Show("대출이 완료되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);

                        // 4. 대출 성공 이벤트를 발생시켜 부모 창에 알립니다.
                        BookLoaned?.Invoke(this, EventArgs.Empty);

                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"대출 처리 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // 닫기 버튼 클릭 이벤트
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}