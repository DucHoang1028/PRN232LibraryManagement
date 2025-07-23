using System;
using System.Collections.Generic;

namespace LibraryManagementWebClient.Models
{
    public class BlogViewModel
    {
        public List<BlogPost> BlogPosts { get; set; } = new();
        public Dictionary<string, int> Tags { get; set; } = new();
        public Dictionary<string, int> Authors { get; set; } = new();
        public string? SelectedTag { get; set; }
        public Guid? SelectedAuthorId { get; set; }
    }
} 