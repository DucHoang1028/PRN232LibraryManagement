using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuinessObjects.Models
{
    public class BookAuthor
    {
        public string BookId { get; set; }
        public Book? Book { get; set; }
        public string AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}
