using BusinessObjects;

namespace Services
{
    public interface IMemberService
    {
        List<Member> GetMembers();
        Member? GetMemberById(Guid memberId);
        Member? GetMemberByEmail(string email);
        Member? GetMemberByLibraryCard(string libraryCardNumber);
        Member CreateMember(Member member);
        Member UpdateMember(Guid memberId, Member member);
        bool DeleteMember(Guid memberId);
        bool DeactivateMember(Guid memberId);
        List<Loan> GetMemberLoans(Guid memberId);
        List<Reservation> GetMemberReservations(Guid memberId);
        List<Fine> GetMemberFines(Guid memberId);
        bool CanCheckoutBook(Guid memberId);
        int GetActiveLoanCount(Guid memberId);
        string GenerateLibraryCardNumber();
        string GenerateBarcode();
    }
} 