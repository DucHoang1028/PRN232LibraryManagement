using LibraryManagementWebClient.Models;

namespace LibraryManagementWebClient.Models
{
    public class LoanHistoryViewModel
    {
        public Member? Member { get; set; }
        public List<Loan> Loans { get; set; } = new();
        public List<Fine> Fines { get; set; } = new();
        public bool CanCheckout { get; set; }
        public int ActiveLoanCount { get; set; }
        public int TotalLoans { get; set; }
        public List<Loan> ActiveLoans { get; set; } = new();
        public List<Loan> OverdueLoans { get; set; } = new();
        public decimal TotalFines { get; set; }
        public decimal UnpaidFines { get; set; }
    }
} 