using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class Review
    {
        public Guid ReviewId { get; set; }
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }
        
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Rating { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        // Navigation properties
        public Book? Book { get; set; }
        public Member? Member { get; set; }
        
        // Display properties
        public string MemberName => Member != null ? $"{Member.FirstName} {Member.LastName}" : "Unknown";
        public string FormattedDate => CreatedDate.ToString("MMM dd, yyyy");
        public string Stars => new string('★', Rating) + new string('☆', 5 - Rating);
    }
} 