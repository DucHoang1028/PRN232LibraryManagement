using BusinessObjects;

namespace Repositories
{
    public interface IMemberRepository
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
        int GetActiveLoanCount(Guid memberId);
    }
} 