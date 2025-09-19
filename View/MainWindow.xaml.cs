using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace library_management_system
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();
        }


        private void Add_book(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("'새 도서 추가' 버튼 클릭됨");
        }

        private void Modify_book(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("'도서 수정' 버튼 클릭됨");
        }

        private void Delete_book(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("'도서 삭제' 버튼 클릭됨");
        }

        private void Search_book(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("'도서 검색' 버튼 클릭됨");
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("'새로고침' 버튼 클릭됨");
        }

        private void Info_click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("'정보' 버튼 클릭됨");
        }

        private void Add_Member(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("'새 회원 추가' 버튼 클릭됨");
        }

        private void ResignedMember_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("'탈퇴 회원 조회' 버튼 클릭됨");
        }

        private void Search_member(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("'회원 검색' 버튼 클릭됨");
        }

        private void loan_book(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("'도서 대출' 버튼 클릭됨");
        }
    }

}