using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class CustomerDashboardViewModel
    {
        public List<Book> FeaturedBooks { get; set; } = new();
        public List<Book> RecentBooks { get; set; } = new();
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
    }
} 