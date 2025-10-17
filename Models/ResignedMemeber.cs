using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library_management_system.Models
{
    internal class ResignedMemeber
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime? Brithdate { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
    }
}