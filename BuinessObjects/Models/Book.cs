using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuinessObjects.Models
{
    public class Book
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public string PublisherId { get; set; }
        public string Description {  get; set; } = string.Empty;
        public DateTime PublicationDate { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public Publisher? Publisher { get; set; }
        public List<BookAuthor> BookAuthors { get; set; } = new();
        public List<BookCategory> BookCategories { get; set; } = new();
        public List<Loan> Loans { get; set; } = new();
        public List<Reservation> Reservations { get; set; } = new();

    }
}
