namespace LibraryManagementWebClient.ViewModels
{
    public class UserManagementViewModel
    {
        public int MemberCount { get; set; }
        public int StaffCount { get; set; }
        public string SelectedRoleFilter { get; set; } = null; // Default

        public List<UserSummaryViewModel> Users { get; set; } = new();
    }

    public class UserSummaryViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsBanned { get; set; }

        // Only for Member role
        public int? TotalLoans { get; set; }
        public int? TotalBorrows { get; set; }
        public decimal? TotalFines { get; set; }

        public string SortBy { get; set; }
        public string SortDirection { get; set; } // "asc" or "desc"
        public string SearchQuery { get; set; }
        public bool? ShowBannedOnly { get; set; }

    }
}
