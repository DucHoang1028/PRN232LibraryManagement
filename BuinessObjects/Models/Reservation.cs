using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuinessObjects.Models
{
    public class Reservation
    {
        public Guid ReservationId { get; set; }
        public string BookId { get; set; }
        public string MemberId { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Book? Book { get; set; }
        public Member? Member { get; set; }
    }
}
