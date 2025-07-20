using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Loan
    {
        public Guid LoanId { get; set; }
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }
        public DateTime CheckoutDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = "Active"; // Active, Returned, Overdue
        public bool IsRenewed { get; set; } = false;
        public int RenewalCount { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public Book? Book { get; set; }
        public Member? Member { get; set; }
        public Fine? Fine { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
    }
}
