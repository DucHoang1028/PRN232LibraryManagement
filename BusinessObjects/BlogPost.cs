using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class BlogPost
    {
        public Guid BlogPostId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(8000)]
        public string Content { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Summary { get; set; }
        
        // Optional featured image URL
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        // Tags for categorizing blog posts (comma-separated)
        [MaxLength(200)]
        public string? Tags { get; set; }
        
        // Publishing status: Draft, Published
        [MaxLength(20)]
        public string Status { get; set; } = "Draft";
        
        public Guid AuthorId { get; set; }
        
        public DateTime PublishedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        
        // Navigation property
        public Member? Author { get; set; }
    }
} 