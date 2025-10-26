using library_management_system.Models;
using library_management_system.Repository;
using library_management_system.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using MessageBox = System.Windows.MessageBox;

namespace library_management_system.View
{
    /// <summary>
    /// AddMemberWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddMemberWindow : Window
    {
        private readonly IMemberRepository _MemberRepository;
        private AddMemberViewModel _viewModel;

        public AddMemberWindow(ObservableCollection<Member> existingMembers, IMemberRepository memberRepository)
        {
            InitializeComponent();

            _MemberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));

            // AddMemberViewModel 생성 시 기존 멤버 컬렉션과 Repository를 전달
            _viewModel = new AddMemberViewModel(existingMembers, _MemberRepository);
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

                    // 선택된 이미지를 Images/members 폴더로 복사
                    string destinationPath = CopyImageToProjectFolder(selectedFilePath);

                    // ViewModel에 경로 설정
                    _viewModel.ImagePath = destinationPath;

                    // 이미지 파일을 바이트 배열로 변환하여 저장
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

        private string CopyImageToProjectFolder(string sourceFilePath)
        {
            try
            {
                // 프로젝트의 Images/members 폴더 경로
                string projectImagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "members");

                // 폴더가 없으면 생성
                if (!System.IO.Directory.Exists(projectImagesFolder))
                {
                    System.IO.Directory.CreateDirectory(projectImagesFolder);
                }

                // 파일 이름과 확장자 추출
                string fileName = System.IO.Path.GetFileName(sourceFilePath);
                string destinationPath = System.IO.Path.Combine(projectImagesFolder, fileName);

                // 파일 복사 (덮어쓰기)
                System.IO.File.Copy(sourceFilePath, destinationPath, true);

                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 복사 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return sourceFilePath;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}