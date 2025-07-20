using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Fine
    {
        public Guid FineId { get; set; }
        public Guid LoanId { get; set; }
        public Guid MemberId { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? PaidDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public Loan? Loan { get; set; }
        public Member? Member { get; set; }
    }
}
