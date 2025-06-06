using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuinessObjects.Models
{
    public class Author
    {
        public Guid AuthorId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<BookAuthor> BookAuthors { get; set; } = new();
    }
}
