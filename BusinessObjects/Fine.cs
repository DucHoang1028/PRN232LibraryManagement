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
        public decimal Amount { get; set; }
        public bool Paid { get; set; }
        public Loan? Loan { get; set; }
    }
}
