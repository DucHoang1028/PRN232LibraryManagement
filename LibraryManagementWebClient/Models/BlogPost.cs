using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementWebClient.Models
{
    public class BlogPost
    {
        public Guid BlogPostId { get; set; }
        
        [Required]
        [MaxLength(200)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(8000)]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;
        
        [MaxLength(500)]
        [Display(Name = "Summary")]
        public string? Summary { get; set; }
        
        [MaxLength(500)]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }
        
        [MaxLength(200)]
        [Display(Name = "Tags (comma separated)")]
        public string? Tags { get; set; }
        
        [MaxLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Draft";
        
        public Guid AuthorId { get; set; }
        
        [Display(Name = "Published Date")]
        public DateTime PublishedDate { get; set; }
        
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedDate { get; set; }
        
        // Navigation property
        public Member? Author { get; set; }
        
        // Helper properties
        public string AuthorName => Author != null ? $"{Author.FirstName} {Author.LastName}" : "Unknown";
        
        public string FormattedPublishedDate => 
            Status == "Published" ? PublishedDate.ToString("MMMM dd, yyyy") : "Not published yet";
        
        public List<string> TagList => 
            Tags?.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList() ?? new List<string>();
    }
} 