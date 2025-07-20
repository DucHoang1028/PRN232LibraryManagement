using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class Member
    {
        public Guid MemberId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [Required]
        public DateTime JoinDate { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Member";
        
        [Required]
        [StringLength(50)]
        public string LibraryCardNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Barcode { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedDate { get; set; }
        
        public List<Loan> Loans { get; set; } = new();
        
        public List<Reservation> Reservations { get; set; } = new();
        
        public List<Fine> Fines { get; set; } = new();
    }
} 