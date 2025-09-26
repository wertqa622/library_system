using System;
using System.Collections.Generic;
using System.Windows;
using System;
using Microsoft.Win32;
using library_management_system.View;
using library_management_system.Models;

using library_management_system.ViewModels;
using Application = System.Windows.Application;

namespace library_management_system
{
    public partial class AddBookWindow : Window
    {

        private readonly MainViewModel _mainViewModel;
        private AddBookViewModel _viewModel;

        public AddBookWindow( MainViewModel mainViewModel)
        {
            InitializeComponent();

            _mainViewModel = mainViewModel;

            _viewModel = new AddBookViewModel();
            DataContext = _viewModel;

            // 창이 로드될 때 첫 번째 입력 필드에 포커스
            Loaded += (s, e) =>
            {
                // 제목 TextBox에 포커스
                var titleTextBox = this.FindName("TitleTextBox") as System.Windows.Controls.TextBox;
                titleTextBox?.Focus();
            };
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
            System.Windows.MessageBox.Show("이미 등록된 도서입니다", "오류");
        }
    }
} 