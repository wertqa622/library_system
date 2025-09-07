using System;
using System.Collections.ObjectModel;
using library_management_system.ViewModels;

namespace library_management_system.ViewModels
{
    public class AddBookViewModel : ViewModelBase
    {
        private string _title;
        private string _author;
        private string _isbn;
        private string _publisher;
        private DateTime? _publishDate;
        private string _category;
        private string _description;
        private int _price;
        private string _imagePath;
        private bool _isAvailable;

        public AddBookViewModel()
        {
            // 기본값 설정
            PublishDate = DateTime.Now;
            IsAvailable = true;
            Price = 0;
            
            // 카테고리 목록 초기화
            
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        public string ISBN
        {
            get => _isbn;
            set => SetProperty(ref _isbn, value);
        }

        public string Publisher
        {
            get => _publisher;
            set => SetProperty(ref _publisher, value);
        }

        public DateTime? PublishDate
        {
            get => _publishDate;
            set => SetProperty(ref _publishDate, value);
        }

        

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public int Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set => SetProperty(ref _isAvailable, value);
        }

        
    }
} 