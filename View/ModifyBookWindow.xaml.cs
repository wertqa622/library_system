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
            _bookRepository = bookRepository;
            _mainViewModel = mainViewModel;
            _originalBook = bookToEdit;

            _viewModel = new ModifyBookViewModel(bookToEdit, bookRepository);
            DataContext = _viewModel;
            
            // ViewModel의 RequestClose 이벤트 구독
            _viewModel.RequestClose += OnRequestClose;

            // 창이 로드될 때 첫 번째 입력 필드에 포커스
            Loaded += (s, e) =>
            {
                // 제목 TextBox에 포커스
                var titleTextBox = this.FindName("TitleTextBox") as System.Windows.Controls.TextBox;
                titleTextBox?.Focus();
            };
        }

        private void OnRequestClose(bool result)
        {
            this.DialogResult = result;
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
                if (_viewModel == null)
                {
                    System.Windows.MessageBox.Show("ViewModel이 초기화되지 않았습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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