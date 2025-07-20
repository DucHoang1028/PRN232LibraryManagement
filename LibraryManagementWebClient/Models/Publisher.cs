using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class Publisher
    {
        public Guid PublisherId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [Url]
        [StringLength(500)]
        public string? Website { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public List<Book> Books { get; set; } = new();
    }
} 