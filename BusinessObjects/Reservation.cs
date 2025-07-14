using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Reservation
    {
        public Guid ReservationId { get; set; }
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Book? Book { get; set; }
        public Member? Member { get; set; }
    }
}
