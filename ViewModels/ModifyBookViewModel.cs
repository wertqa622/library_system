using System.ComponentModel;
using System.Runtime.CompilerServices;
using library_management_system.Models;

namespace library_management_system.ViewModels
{
    public class ModifyBookViewModel : ViewModelBase
    {
        private string _title;
        private string _author;
        private string _publisher;
        private string _isbn;
        private int _price;
        private string _imagePath;
        private string _description;

        public ModifyBookViewModel(Book book)
        {
            // 기존 도서 정보로 초기화
            Title = book.Title;
            Author = book.Author;
            Publisher = book.Publisher;
            ISBN = book.ISBN;
            Price = book.Price;
            ImagePath = book.ImagePath;
            Description = book.Description;
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

        public string Publisher
        {
            get => _publisher;
            set => SetProperty(ref _publisher, value);
        }

        public string ISBN
        {
            get => _isbn;
            set => SetProperty(ref _isbn, value);
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

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}