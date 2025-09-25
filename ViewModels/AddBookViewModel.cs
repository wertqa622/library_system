using System;
using System.Collections.ObjectModel;
using library_management_system.ViewModels;

namespace library_management_system.ViewModels
{
    public class AddBookViewModel : ViewModelBase
    {
        public string Title { get; set; }

        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public DateTime? PublishDate { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string ImagePath { get; set; }
        public bool IsAvailable { get; set; }

        public AddBookViewModel()
        {
            // 기본값 설정
            PublishDate = DateTime.Now;
            IsAvailable = true;
            Price = 0;
        }
    }
}