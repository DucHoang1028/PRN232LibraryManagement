using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuinessObjects.Models
{
    public class BookCategory
    {
        public string BookId { get; set; }
        public Book? Book { get; set; }
        public string CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
