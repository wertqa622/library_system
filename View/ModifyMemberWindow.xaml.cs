using System;
using System.Windows;
using library_management_system.Models;
using library_management_system.Repository;
using library_management_system.ViewModels;
using MessageBox = System.Windows.MessageBox;

namespace library_management_system.View
{
    /// <summary>
    /// ModifyMemberWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModifyMemberWindow : Window
    {
        private readonly IMemberRepository _memberRepository;
        private ModifyMemberViewModel _viewModel;

        public ModifyMemberWindow(Member selectedMember, IMemberRepository memberRepository)
        {
            InitializeComponent();
            _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));

            _viewModel = new ModifyMemberViewModel(selectedMember, _memberRepository);
            _viewModel.RequestClose += OnRequestClose;
            DataContext = _viewModel;

            Window_Loaded();
        }

        private void OnRequestClose()
        {
            this.DialogResult = true;
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

        private void Window_Loaded()
        {
        }

        private void wndqhr_click(object sender, RoutedEventArgs e)
        {
        }

        private void SelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                openFileDialog.Title = "사진 선택";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    // 이미지 파일을 바이트 배열로 변환하여 ViewModel에 저장
                    try
                    {
                        byte[] imageBytes = System.IO.File.ReadAllBytes(selectedFilePath);
                        _viewModel.PhotoBytes = imageBytes;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"이미지 로드 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"사진 선택 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Application.Current.MainWindow is MainWindow main)
            {
                main.hdgd();
            }
            DialogResult = false;
            Close();
        }
    }
}