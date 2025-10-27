using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace library_management_system
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                System.Diagnostics.Debug.WriteLine("이미지 경로가 null이거나 비어있음");
                return null;
            }

            string imagePath = value.ToString();
            System.Diagnostics.Debug.WriteLine($"이미지 경로: {imagePath}");

            try
            {
                // 상대 경로 처리
                if (!imagePath.StartsWith("/") && !Path.IsPathRooted(imagePath))
                {
                    // 현재 실행 디렉토리 기준으로 상대 경로 처리
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
                    System.Diagnostics.Debug.WriteLine($"전체 경로: {fullPath}");

                    if (File.Exists(fullPath))
                    {
                        System.Diagnostics.Debug.WriteLine($"이미지 파일 발견: {fullPath}");
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(fullPath);
                        bitmap.EndInit();
                        return bitmap;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"이미지 파일이 존재하지 않음: {fullPath}");
                    }
                }
                // 리소스에서 이미지 로드 시도
                else if (imagePath.StartsWith("/"))
                {
                    var uri = new Uri($"pack://application:,,,{imagePath}");
                    System.Diagnostics.Debug.WriteLine($"리소스 URI: {uri}");
                    return new BitmapImage(uri);
                }
                // 절대 경로 처리
                else if (File.Exists(imagePath))
                {
                    System.Diagnostics.Debug.WriteLine($"절대 경로 이미지 발견: {imagePath}");
                    return new BitmapImage(new Uri(imagePath));
                }
            }
            catch (Exception ex)
            {
                // 디버깅을 위해 예외 정보 출력
                System.Diagnostics.Debug.WriteLine($"이미지 로드 실패: {imagePath}, 오류: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("이미지를 로드할 수 없음");
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageByteArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            byte[] imageBytes = value as byte[];
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return null;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(imageBytes);
                bitmap.EndInit();

                return bitmap;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return Visibility.Visible; // 기본 아이콘 표시
            }

            string imagePath = value.ToString();

            try
            {
                if (!imagePath.StartsWith("/") && !Path.IsPathRooted(imagePath))
                {
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
                    if (File.Exists(fullPath))
                    {
                        return Visibility.Collapsed; // 이미지가 있으면 아이콘 숨김
                    }
                }
                else if (imagePath.StartsWith("/"))
                {
                    var uri = new Uri($"pack://application:,,,{imagePath}");
                    var bitmap = new BitmapImage(uri);
                    return Visibility.Collapsed; // 이미지가 있으면 아이콘 숨김
                }
                else if (File.Exists(imagePath))
                {
                    return Visibility.Collapsed; // 이미지가 있으면 아이콘 숨김
                }
            }
            catch (Exception ex)
            {
                // 디버깅을 위해 예외 정보 출력
                System.Diagnostics.Debug.WriteLine($"이미지 가시성 확인 실패: {imagePath}, 오류: {ex.Message}");
            }

            return Visibility.Visible; // 기본 아이콘 표시
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToLoanStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool loanStatus)
            {
                return loanStatus ? "대출 가능" : "대출 불가";
            }
            return "대출 불가";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}