using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class MemberRepository : IMemberRepository
    {
        public List<Member> GetMembers() => MemberDAO.GetMembers();
        public Member? GetMemberById(Guid memberId) => MemberDAO.GetMemberById(memberId);
        public Member? GetMemberByEmail(string email) => MemberDAO.GetMemberByEmail(email);
        public Member? GetMemberByLibraryCard(string libraryCardNumber) => MemberDAO.GetMemberByLibraryCard(libraryCardNumber);
        public Member CreateMember(Member member) => MemberDAO.CreateMember(member);
        public Member UpdateMember(Guid memberId, Member member) => MemberDAO.UpdateMember(memberId, member);
        public bool DeleteMember(Guid memberId) => MemberDAO.DeleteMember(memberId);
        public bool DeactivateMember(Guid memberId) => MemberDAO.DeactivateMember(memberId);
        public List<Loan> GetMemberLoans(Guid memberId) => MemberDAO.GetMemberLoans(memberId);
        public List<Reservation> GetMemberReservations(Guid memberId) => MemberDAO.GetMemberReservations(memberId);
        public List<Fine> GetMemberFines(Guid memberId) => MemberDAO.GetMemberFines(memberId);
        public int GetActiveLoanCount(Guid memberId) => MemberDAO.GetActiveLoanCount(memberId);
    }
} 