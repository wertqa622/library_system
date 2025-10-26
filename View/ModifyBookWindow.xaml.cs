using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using library_management_system.Models;
using library_management_system.ViewModels;
using Application = System.Windows.Application;
using library_management_system.Repository;

namespace library_management_system
{
    public partial class ModifyBookWindow : Window
    {
        private readonly IBookRepository _bookRepository;
        private readonly MainViewModel _mainViewModel;
        private readonly Book _originalBook;
        private ModifyBookViewModel _viewModel;

        public ModifyBookWindow(IBookRepository bookRepository, MainViewModel mainViewModel, Book bookToEdit)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
            _originalBook = bookToEdit;

            _viewModel = new ModifyBookViewModel(bookToEdit);
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

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 버튼 비활성화 (중복 클릭 방지)
                var updateButton = sender as System.Windows.Controls.Button;
                var cancelButton = this.FindName("CancelButton") as System.Windows.Controls.Button;

                if (updateButton != null) updateButton.IsEnabled = false;
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

                // 수정된 도서 객체 생성
                var updatedBook = new Book
                {
                    ISBN = _viewModel.ISBN.Trim(),
                    BookName = _viewModel.Title.Trim(),
                    Author = _viewModel.Author.Trim(),
                    Publisher = _viewModel.Publisher.Trim(),
                    Price = _viewModel.Price,
                    BookImage = new byte[0], // 이미지 파일을 바이트 배열로 변환 필요
                    Description = _viewModel.Description?.Trim() ?? "",
                    BookUrl = _viewModel.ImagePath?.Trim() ?? "",
                    IsAvailable = true // 기본값으로 대여 가능 설정
                };

                // 도서 수정 해야함
                var result = await _bookRepository.UpdateBookAsync(updatedBook);

                // 컬렉션에 직접 수정하지 않고 DialogResult만 설정
                // MainViewModel에서 DB 재조회로 갱신됨

                System.Windows.MessageBox.Show("도서가 성공적으로 수정되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"도서 수정 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // 버튼 다시 활성화
                var updateButton = sender as System.Windows.Controls.Button;
                var cancelButton = this.FindName("CancelButton") as System.Windows.Controls.Button;

                if (updateButton != null) updateButton.IsEnabled = true;
                if (cancelButton != null) cancelButton.IsEnabled = true;
            }
        }

        private void wndqhr_click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("이미 등록된 도서입니다.", "오류");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow main)
            {
                main.hdgd();
            }
            DialogResult = false;
            Close();
        }

        // ESC 키로 창 닫기
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                DialogResult = false;
                Close();
            }
            base.OnKeyDown(e);
        }
    }
}