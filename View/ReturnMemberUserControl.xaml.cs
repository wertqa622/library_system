using library_management_system.DataBase;
using library_management_system.View;
using Microsoft.Extensions.DependencyInjection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace library_management_system.View
{
    /// <summary>
    /// LoanBookUserControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReturnMemberUserControl : System.Windows.Controls.UserControl
    {
        private readonly OracleDapperHelper _dbHelper;
        public ObservableCollection<dynamic> Loans { get; set; }

        public ReturnMemberUserControl(OracleDapperHelper dbHelper = null)
        {
            InitializeComponent();

            _dbHelper = App.AppHost!.Services.GetRequiredService<OracleDapperHelper>();

            Loans = new ObservableCollection<dynamic>();
            this.DataContext = this;

            LoadLoanMembers(); // 데이터를 따로 로드
        }

        private void LoadLoanMembers()
        {
            string sql = @"
                SELECT 
                    NAME AS Name,
                    EMAIL AS Email,
                    PHONENUMBER AS PhoneNumber,
                    BIRTHDATE AS Birthdate,
                    GENDER AS Gender
                FROM MEMBER
                WHERE LOANSTATUS = 'A'
                ORDER BY NAME";

            var members = _dbHelper.Query<dynamic>(sql).ToList();

            Loans.Clear();
            foreach (var m in members)
            {
                Loans.Add(m);
            }
        }

        private void return_click(object sender, RoutedEventArgs e)
        {
            dynamic selectedMember = (sender as FrameworkElement)?.DataContext;
            if (selectedMember == null) return;

            string phoneNumber = selectedMember.PhoneNumber; // SQL에서 PHONENUMBER AS Number로 가져왔기 때문

            // Loan_Book 생성자도 string phoneNumber를 받도록 구성되어 있어야 함
            Loan_Book a = new Loan_Book(phoneNumber, _dbHelper);
            a.Show();
        }
    }
}