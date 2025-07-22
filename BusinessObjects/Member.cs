using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Member
    {
        public Guid MemberId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime JoinDate { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "Member";
        public string LibraryCardNumber { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public List<Loan> Loans { get; set; } = new();
        public List<Reservation> Reservations { get; set; } = new();
        public List<Fine> Fines { get; set; } = new();
    }

    //Note: co 3 role guest, staff, admin
}
