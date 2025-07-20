using System.Collections.Generic;

namespace LibraryManagementWebClient.Models
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public List<Book> RecentBooks { get; set; } = new();
        public List<Loan> RecentLoans { get; set; } = new();
    }
} 