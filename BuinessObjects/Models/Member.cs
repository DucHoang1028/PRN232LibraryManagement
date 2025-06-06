using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuinessObjects.Models
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
        public List<Loan> Loans { get; set; } = new();
        public List<Reservation> Reservations { get; set; } = new();
    }
}
