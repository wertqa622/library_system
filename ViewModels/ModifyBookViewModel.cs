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
            Title = book.BookName;
            Author = book.Author;
            Publisher = book.Publisher;
            ISBN = book.ISBN;
            Price = book.Price;
            ImagePath = book.ImagePath;
            Description = book.Description;
        }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Publisher { get; set; }

        public string ISBN { get; set; }

        public decimal Price { get; set; }

        public string ImagePath { get; set; }

        public string Description { get; set; }
    }
}