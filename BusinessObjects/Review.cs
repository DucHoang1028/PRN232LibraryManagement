using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Review
    {
        public Guid ReviewId { get; set; }
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }
        
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [MaxLength(1000)]
        public string? Comment { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        
        // Navigation properties
        public Book? Book { get; set; }
        public Member? Member { get; set; }
    }
} 