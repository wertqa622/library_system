using System;
using System.Text.RegularExpressions;
using System.Windows.Input;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using library_management_system.Models;
using library_management_system.Repository;
using library_management_system.View;
using library_management_system.ViewModels;
using Microsoft.Win32;
using Application = System.Windows.Application;

namespace library_management_system
{
    public partial class AddBookWindow : Window
    {
        private readonly IBookRepository _bookRepository;
        private readonly MainViewModel _mainViewModel;
        private AddBookViewModel _viewModel;

        public AddBookWindow(ObservableCollection<Book> existingBooks, IBookRepository bookRepository)
        {
            InitializeComponent();

            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));

            // AddBookViewModel 생성 시 기존 도서 컬렉션과 Repository를 전달
            _viewModel = new AddBookViewModel(existingBooks, _bookRepository);
            DataContext = _viewModel;

            // ViewModel의 RequestClose 이벤트 구독
            _viewModel.RequestClose += OnRequestClose;
        }

        private void OnRequestClose()
        {
            this.DialogResult = true; // 성공적으로 추가됨을 표시
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.RequestClose -= OnRequestClose;
            }
            base.OnClosed(e);
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    Title = "도서 이미지 선택",
                    Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp;*.gif|모든 파일|*.*",
                    FilterIndex = 1,
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    // 선택된 이미지를 Images/books 폴더로 복사
                    string destinationPath = CopyImageToProjectFolder(selectedFilePath);

                    // ViewModel에 경로 설정
                    _viewModel.ImagePath = destinationPath;

                    // 이미지 파일을 바이트 배열로 변환하여 저장
                    try
                    {
                        byte[] imageBytes = System.IO.File.ReadAllBytes(selectedFilePath);
                        _viewModel.BookImageBytes = imageBytes;

                        System.Diagnostics.Debug.WriteLine($"이미지 로드 완료: {selectedFilePath}, 크기: {imageBytes.Length} bytes");
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"이미지 로드 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"이미지 선택 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string CopyImageToProjectFolder(string sourceFilePath)
        {
            try
            {
                // 프로젝트의 Images/books 폴더 경로
                string projectImagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "books");

                // 폴더가 없으면 생성
                if (!System.IO.Directory.Exists(projectImagesFolder))
                {
                    System.IO.Directory.CreateDirectory(projectImagesFolder);
                }

                // 파일명 생성 (중복 방지)
                string fileName = System.IO.Path.GetFileName(sourceFilePath);
                string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
                string extension = System.IO.Path.GetExtension(fileName);

                string destinationPath = System.IO.Path.Combine(projectImagesFolder, fileName);

                // 파일명이 중복되면 번호 추가
                int counter = 1;
                while (System.IO.File.Exists(destinationPath))
                {
                    string newFileName = $"{fileNameWithoutExt}_{counter}{extension}";
                    destinationPath = System.IO.Path.Combine(projectImagesFolder, newFileName);
                    counter++;
                }

                // 파일 복사
                System.IO.File.Copy(sourceFilePath, destinationPath, true);

                // 상대 경로 반환 (Images/books/파일명)
                return $"Images/books/{System.IO.Path.GetFileName(destinationPath)}";
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"이미지 복사 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return sourceFilePath; // 원본 경로 반환
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow main)
            {
                main.hdgd();
            }
            DialogResult = false;
            this.Close();
        }

        // ESC 키로 창 닫기
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                DialogResult = false;
                MainWindow main = new MainWindow();
                main.hdgd();
                Close();
            }
            base.OnKeyDown(e);
        }

        private void wndqhr_click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainViewModel viewModel)
            {
                string _lsbn = GetNewIsbnFromUI();

                bool isDuplicate = viewModel.Books.Any(book => book.ISBN == _lsbn);

                // 4. 결과에 따라 사용자에게 알림을 줍니다.
                if (isDuplicate)
                {
                    System.Windows.MessageBox.Show("이미 존재하는 ISBN입니다.", "중복 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    System.Windows.MessageBox.Show("사용 가능한 ISBN입니다.", "확인 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void IsbnTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 정규식을 사용하여 입력된 텍스트가 숫자인지 확인합니다.
            // 숫자가 아니면 입력을 무시합니다.
            e.Handled = !IsIsbnTextAllowed(e.Text);
        }

        private void IsbnTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            // 붙여넣으려는 데이터가 텍스트 형식인지 확인합니다.
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));

                // 붙여넣으려는 텍스트에 숫자가 아닌 문자가 포함되어 있다면,
                if (!IsIsbnTextAllowed(text))
                {
                    // 붙여넣기 작업을 취소합니다.
                    e.CancelCommand();
                }
            }
            else
            {
                // 텍스트가 아닌 다른 형식을 붙여넣으려고 하면 무조건 취소합니다.
                e.CancelCommand();
            }
        }

        private bool IsIsbnTextAllowed(string str)
        {
            // [^0-9-] : 숫자(0-9)와 하이픈(-)을 제외한 모든 문자를 찾는 정규식
            Regex reg = new Regex("[^0-9-]");

            // 위에서 정의한 문자가 포함되어 있지 않으면 true를 반환
            return !reg.IsMatch(str);
        }

        private string GetNewIsbnFromUI()
        {
            // 예를 들어, TextBox의 x:Name이 'isbnTextBox'인 경우
            TextBox isbntxt = (TextBox)this.FindName("isbntxt");
            return isbntxt?.Text;

            return "isbntxt";
        }
    }
}