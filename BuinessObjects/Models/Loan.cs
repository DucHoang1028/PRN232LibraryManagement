using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuinessObjects.Models
{
    public class Loan
    {
        public Guid LoanId { get; set; }
        public string BookId { get; set; }
        public string MemberId { get; set; }
        public DateTime CheckoutDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public Book? Book { get; set; }
        public Member? Member { get; set; }
        public Fine? Fine { get; set; }
    }
}
