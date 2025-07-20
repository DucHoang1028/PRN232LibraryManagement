using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class SearchResultsViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public List<Book> Results { get; set; } = new();
        public int TotalResults { get; set; }
    }
} 