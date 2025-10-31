using System;

namespace library_management_system.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public byte[] BookImage { get; set; } = new byte[0];
        public string BookName { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string BookUrl { get; set; } = string.Empty;
        
        // UI에서 사용할 추가 속성들
        public string ImagePath { get; set; } = string.Empty; // 파일 경로용
        public bool IsAvailable { get; set; } = true; // 대출 가능 여부
    }
}