using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class Reservation
    {
        public Guid ReservationId { get; set; }
        
        [Required]
        public Guid BookId { get; set; }
        
        [Required]
        public Guid MemberId { get; set; }
        
        [Required]
        public DateTime ReservationDate { get; set; }
        
        [Required]
        public DateTime ExpiryDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedDate { get; set; }
        
        public Book? Book { get; set; }
        
        public Member? Member { get; set; }
    }
} 