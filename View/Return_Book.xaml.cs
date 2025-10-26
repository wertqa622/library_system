using library_management_system.DataBase;
using library_management_system.Models;
using library_management_system.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
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
    public partial class Return_Book : Window
    {
        private readonly OracleDapperHelper _dbHelper;
        private readonly string _phoneNumber;

        public Return_Book(string phoneNumber, OracleDapperHelper dbHelper)
        {
            InitializeComponent();
            _dbHelper = dbHelper;
            _phoneNumber = phoneNumber;
            LoadLoanBooks();
        }

        private void LoadLoanBooks()
        {
            string sql = @"
                SELECT 
                    l.LOAN_ID AS LoanId,
                    b.BOOKIMAGE AS BookImage,
                    b.ISBN AS ISBN,
                    b.BOOKNAME AS BookName,
                    b.PUBLISHER AS Publisher,
                    b.AUTHOR AS Author,
                    l.LOANDATE AS LoanDate,
                    l.DUEDATE AS DueDate
                FROM LOAN l
                JOIN BOOK b ON l.ISBN = b.ISBN
                JOIN MEMBER m ON l.PHONENUMBER = m.PHONENUMBER
                WHERE l.PHONENUMBER = :phoneNumber";

            var loanBooks = _dbHelper.Query<dynamic>(sql, new { phoneNumber = _phoneNumber });
            LoanBooksGrid.ItemsSource = new List<dynamic>(loanBooks);
        }

        private void loanbook_close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void returnbook_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            dynamic selectedBook = button?.DataContext;

            if (selectedBook == null)
            {
                System.Windows.MessageBox.Show("선택된 도서 정보를 불러올 수 없습니다.");
                return;
            }

            try
            {
                // DB에서 해당 대출 데이터 삭제
                string sql = "DELETE FROM LOAN WHERE LOAN_ID = :LoanId";
                _dbHelper.Execute(sql, new { LoanId = selectedBook.LoanId });

                // 현재 UI 데이터에서 제거
                var list = LoanBooksGrid.ItemsSource as List<dynamic>;
                list?.Remove(selectedBook);
                LoanBooksGrid.Items.Refresh();

                System.Windows.MessageBox.Show("도서가 반납되었습니다.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"반납 처리 중 오류가 발생했습니다.\n{ex.Message}");
            }
        }
    }
}