using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuinessObjects.Models
{
    public class Fine
    {
        public Guid FineId { get; set; }
        public string LoanId { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; }
        public Loan? Loan { get; set; }
    }
}
