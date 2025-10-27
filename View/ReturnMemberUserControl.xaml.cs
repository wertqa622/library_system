using library_management_system.DataBase;
using library_management_system.Models;
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
                        m.NAME          AS Name,
                        l.PHONENUMBER   AS PhoneNumber,
                        l.ISBN AS Isbn,
                        l.LOANDATE      AS LoanDate,
                        l.DUEDATE       AS DueDate,
                        l.RETURNDATE    AS ReturnDate
                    FROM LOAN l
                    INNER JOIN
                    MEMBER m ON l.PHONENUMBER = m.PHONENUMBER";

            var members = _dbHelper.Query<dynamic>(sql).ToList();

            Loans.Clear();
            foreach (var m in members)
            {
                Loans.Add(m);
            }
        }

        private void return_click(object sender, RoutedEventArgs e)
        {
            var selectedMember = (sender as FrameworkElement)?.DataContext as Member;
            if (selectedMember == null) return;

            string phoneNumber = selectedMember.Phone;

            // Loan_Book 생성자에 전화번호(string)만 전달하도록 수정
            Loan_Book returnWindow = new Loan_Book(phoneNumber);
            returnWindow.ShowDialog();
        }
    }
}