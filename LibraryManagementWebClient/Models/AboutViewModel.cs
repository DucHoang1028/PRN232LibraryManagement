using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class AboutViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int TotalPublishers { get; set; }
        public int AvailableBooks { get; set; }
    }
} 