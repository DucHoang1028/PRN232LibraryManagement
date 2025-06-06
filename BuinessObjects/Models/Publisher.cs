using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuinessObjects.Models
{
    public class Publisher
    {
        public Guid PublisherId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public List<Book> Books { get; set; } = new();
    }
}
