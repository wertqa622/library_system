using System;

namespace library_management_system.Models
{
    public class Book
    {
        public string ISBN { get; set; } = string.Empty;
        public byte[] BookImage { get; set; } = new byte[0];
        public string BookName { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string BookUrl { get; set; } = string.Empty;

        // UI���� ����� �߰� �Ӽ���
        public string ImagePath { get; set; } = string.Empty; // ���� ��ο�

        public bool IsAvailable { get; set; } = true; // ���� ���� ����
        public string Title { get; internal set; }
    }
}