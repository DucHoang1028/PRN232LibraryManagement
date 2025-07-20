using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class Fine
    {
        public Guid FineId { get; set; }
        
        [Required]
        public Guid LoanId { get; set; }
        
        [Required]
        public Guid MemberId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        
        public bool Paid { get; set; }
        
        public bool IsPaid { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? PaidDate { get; set; }
        
        public DateTime? UpdatedDate { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public Loan? Loan { get; set; }
        
        public Member? Member { get; set; }
    }
} 