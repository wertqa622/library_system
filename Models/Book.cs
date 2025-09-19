namespace library_management_system
{
    public class Book
    {
        public string ImagePath { get; set; } 
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Price { get; set; }
        public string Description { get; set; } 
        public string Publisher { get; set; }
        public string IsAvailable { get; set; }
    }
}