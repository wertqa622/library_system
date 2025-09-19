using System.Collections.ObjectModel;
using System.ComponentModel;

namespace library_management_system
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // 도서 목록 (DataGrid에 바인딩됨)
        public ObservableCollection<Book> Books { get; set; }

        // 회원 목록 (DataGrid에 바인딩됨)
        public ObservableCollection<Member> Members { get; set; }

        // 대출 목록 (DataGrid에 바인딩됨)
        public ObservableCollection<Loan> Loans { get; set; }


        public MainViewModel()
        {
            // 임시(샘플) 데이터를 생성합니다.
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}