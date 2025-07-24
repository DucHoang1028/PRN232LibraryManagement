namespace Contracts 
{
    public class MemberDto
    {
        public Guid MemberId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime JoinDate { get; set; }
        public string LibraryCardNumber { get; set; }
        public string Barcode { get; set; }
    }


}
