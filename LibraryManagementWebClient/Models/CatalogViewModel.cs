using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebClient.Models
{
    public class CatalogViewModel
    {
        public List<Book> Books { get; set; } = new();
        public List<Publisher> Publishers { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? SelectedPublisher { get; set; }
    }
} 