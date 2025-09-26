using System;
using System.Windows;

namespace library_management_system
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // --- XAML 로딩 오류를 잡기 위한 try-catch 블록 ---
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                // XAML 리소스 로딩 실패 등 숨겨진 오류를 메시지 박스로 보여줍니다.
                string errorMessage = $"오류 타입: {ex.GetType().Name}\n\n메시지: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\n내부 예외: {ex.InnerException.Message}";
                }

              
                return; // 오류 발생 시 더 이상 진행하지 않음
            }

            // ViewModel이나 다른 초기화 코드가 있다면 여기에 둡니다.
            // DataContext = new MainViewModel();
        }

        // ... (나머지 버튼 클릭 이벤트 핸들러들은 그대로 둡니다) ...
        private void Add_book(object sender, RoutedEventArgs e) { }
        private void Modify_book(object sender, RoutedEventArgs e) { }
        private void Delete_book(object sender, RoutedEventArgs e) { }
        private void Search_book(object sender, RoutedEventArgs e) { }
        private void Refresh(object sender, RoutedEventArgs e) { }
        private void Add_Member(object sender, RoutedEventArgs e) { }
        private void ResignedMember_Click(object sender, RoutedEventArgs e) { }
        private void loan_book(object sender, RoutedEventArgs e) { }
        private void Info_click(object sender, RoutedEventArgs e) { }
        private void Search_member(object sender, RoutedEventArgs e) { }
    }
}

