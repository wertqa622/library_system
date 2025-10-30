using System;
<<<<<<< HEAD
=======
using System.Text.RegularExpressions;
using System.Windows.Input;

>>>>>>> acb1365 ([홍서진] 수정)
using System.Collections.Generic;
using System.Windows;

<<<<<<< HEAD
using System;

=======
using library_management_system.Models;
using library_management_system.Repository;
using library_management_system.View;
using library_management_system.ViewModels;
>>>>>>> acb1365 ([홍서진] 수정)
using Microsoft.Win32;
using library_management_system.View;
using library_management_system.Models;

using library_management_system.ViewModels;
using Application = System.Windows.Application;
using library_management_system.Repository;

namespace library_management_system
{
    public partial class AddBookWindow : Window
    {
        private readonly MainViewModel _mainViewModel;
        private AddBookViewModel _viewModel;
        private IBookRepository bookRepository;

        public AddBookWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            _mainViewModel = mainViewModel;

            _viewModel = new AddBookViewModel();
            DataContext = _viewModel;

<<<<<<< HEAD
            // 창이 로드될 때 첫 번째 입력 필드에 포커스
            Loaded += (s, e) =>
=======
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
>>>>>>> acb1365 ([홍서진] 수정)
            {
                // 제목 TextBox에 포커스
                var titleTextBox = this.FindName("TitleTextBox") as System.Windows.Controls.TextBox;
                titleTextBox?.Focus();
            };
        }

        public AddBookWindow(IBookRepository bookRepository, MainViewModel mainViewModel)
        {
            this.bookRepository = bookRepository;
            _mainViewModel = mainViewModel;
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
<<<<<<< HEAD
=======

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
>>>>>>> acb1365 ([홍서진] 수정)
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
<<<<<<< HEAD
            try
            {
                // 버튼 비활성화 (중복 클릭 방지)
                var addButton = sender as System.Windows.Controls.Button;
                var cancelButton = this.FindName("CancelButton") as System.Windows.Controls.Button;

                if (addButton != null) addButton.IsEnabled = false;
                if (cancelButton != null) cancelButton.IsEnabled = false;

                // 입력 검증
                if (string.IsNullOrWhiteSpace(_viewModel.Title))
                {
                    System.Windows.MessageBox.Show("도서 제목을 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_viewModel.Author))
                {
                    System.Windows.MessageBox.Show("저자를 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_viewModel.Publisher))
                {
                    System.Windows.MessageBox.Show("출판사를 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_viewModel.ISBN))
                {
                    System.Windows.MessageBox.Show("ISBN을 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 가격 검증
                if (_viewModel.Price < 0)
                {
                    System.Windows.MessageBox.Show("가격은 0 이상이어야 합니다.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 새 도서 객체 생성
                var newBook = new Book
                {
                    Title = _viewModel.Title.Trim(),
                    Author = _viewModel.Author.Trim(),
                    Publisher = _viewModel.Publisher.Trim(),
                    ISBN = _viewModel.ISBN.Trim(),
                    Price = _viewModel.Price,
                    ImagePath = _viewModel.ImagePath?.Trim() ?? "",
                    Description = _viewModel.Description?.Trim() ?? "",
                };

                System.Windows.MessageBox.Show("도서가 성공적으로 추가되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                if (Application.Current.MainWindow is MainWindow main)
                {
                    main.hdgd();
                }
                Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"도서 추가 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // 버튼 다시 활성화
                var addButton = sender as System.Windows.Controls.Button;
                var cancelButton = this.FindName("CancelButton") as System.Windows.Controls.Button;

                if (addButton != null) addButton.IsEnabled = true;
                if (cancelButton != null) cancelButton.IsEnabled = true;
            }
=======
>>>>>>> acb1365 ([홍서진] 수정)
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
<<<<<<< HEAD
            System.Windows.MessageBox.Show("이미 등록된 도서입니다", "오류");
=======
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
>>>>>>> acb1365 ([홍서진] 수정)
        }
    }
}