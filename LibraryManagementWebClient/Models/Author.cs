using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class Author
    {
        public Guid AuthorId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        public List<BookAuthor> BookAuthors { get; set; } = new();
    }
} 