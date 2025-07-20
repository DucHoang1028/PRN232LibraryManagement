using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class Category
    {
        public Guid CategoryId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public List<BookCategory> BookCategories { get; set; } = new();
    }
} 