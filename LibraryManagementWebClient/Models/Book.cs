using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class Book
    {
        public Guid BookId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Isbn { get; set; } = string.Empty;
        
        [Required]
        public Guid PublisherId { get; set; }
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime PublicationDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Language { get; set; } = string.Empty;
        
        [Required]
        [Range(0, int.MaxValue)]
        public int TotalCopies { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int AvailableCopies { get; set; }
        
        [Required]
        [StringLength(50)]
        public string RackNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Barcode { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedDate { get; set; }
        
        public Publisher? Publisher { get; set; }
        
        public List<BookAuthor> BookAuthors { get; set; } = new();
        
        public List<BookCategory> BookCategories { get; set; } = new();
        
        public List<Loan> Loans { get; set; } = new();
        
        public List<Reservation> Reservations { get; set; } = new();
    }
} 