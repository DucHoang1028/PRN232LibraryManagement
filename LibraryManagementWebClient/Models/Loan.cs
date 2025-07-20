using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class Loan
    {
        public Guid LoanId { get; set; }
        
        [Required]
        public Guid BookId { get; set; }
        
        [Required]
        public Guid MemberId { get; set; }
        
        [Required]
        public DateTime CheckoutDate { get; set; }
        
        [Required]
        public DateTime DueDate { get; set; }
        
        public DateTime? ReturnDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";
        
        public bool IsRenewed { get; set; } = false;
        
        public int RenewalCount { get; set; } = 0;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedDate { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; }
        
        public Book? Book { get; set; }
        
        public Member? Member { get; set; }
        
        public Fine? Fine { get; set; }
    }
} 