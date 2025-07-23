using BusinessObjects;
using Repositories;

namespace Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ILoanRepository _loanRepository;

        public MemberService(IMemberRepository memberRepository, ILoanRepository loanRepository)
        {
            _memberRepository = memberRepository;
            _loanRepository = loanRepository;
        }

        public List<Member> GetMembers() => _memberRepository.GetMembers();

        public Member? GetMemberById(Guid memberId) => _memberRepository.GetMemberById(memberId);

        public Member? GetMemberByEmail(string email) => _memberRepository.GetMemberByEmail(email);

        public Member? GetMemberByLibraryCard(string libraryCardNumber) => _memberRepository.GetMemberByLibraryCard(libraryCardNumber);

        public Member CreateMember(Member member)
        {
            // Validate member data
            if (string.IsNullOrEmpty(member.FirstName))
                throw new ArgumentException("First name is required");

            if (string.IsNullOrEmpty(member.LastName))
                throw new ArgumentException("Last name is required");

            if (string.IsNullOrEmpty(member.Email))
                throw new ArgumentException("Email is required");

            // Check if email already exists
            var existingMember = _memberRepository.GetMemberByEmail(member.Email);
            if (existingMember != null)
                throw new InvalidOperationException("Email already exists");

            return _memberRepository.CreateMember(member);
        }

        public Member UpdateMember(Guid memberId, Member member)
        {
            var existingMember = _memberRepository.GetMemberById(memberId);
            if (existingMember == null)
                throw new ArgumentException("Member not found");

            // Validate member data
            if (string.IsNullOrEmpty(member.FirstName))
                throw new ArgumentException("First name is required");

            if (string.IsNullOrEmpty(member.LastName))
                throw new ArgumentException("Last name is required");

            if (string.IsNullOrEmpty(member.Email))
                throw new ArgumentException("Email is required");

            var memberWithEmail = _memberRepository.GetMemberByEmail(member.Email);
            if (memberWithEmail != null && memberWithEmail.MemberId != memberId)
                throw new InvalidOperationException("Email already exists");
            existingMember.FirstName = member.FirstName;
            existingMember.LastName = member.LastName;
            existingMember.Email = member.Email;
            existingMember.IsActive = member.IsActive;

            return _memberRepository.UpdateMember(memberId, existingMember);
        }


        public bool DeleteMember(Guid memberId)
        {
            var member = _memberRepository.GetMemberById(memberId);
            if (member == null)
                return false;

            // Check if member has active loans
            var activeLoans = _loanRepository.GetLoansByMember(memberId).Where(l => l.Status == "Active");
            if (activeLoans.Any())
                throw new InvalidOperationException("Cannot delete member with active loans");

            return _memberRepository.DeleteMember(memberId);
        }

        public bool DeactivateMember(Guid memberId)
        {
            var member = _memberRepository.GetMemberById(memberId);
            if (member == null)
                return false;

            return _memberRepository.DeactivateMember(memberId);
        }

        public List<Loan> GetMemberLoans(Guid memberId) => _memberRepository.GetMemberLoans(memberId);

        public List<Reservation> GetMemberReservations(Guid memberId) => _memberRepository.GetMemberReservations(memberId);

        public List<Fine> GetMemberFines(Guid memberId) => _memberRepository.GetMemberFines(memberId);

        public bool CanCheckoutBook(Guid memberId)
        {
            var member = _memberRepository.GetMemberById(memberId);
            if (member == null || !member.IsActive)
                return false;

            var activeLoanCount = _memberRepository.GetActiveLoanCount(memberId);
            return activeLoanCount < 5; // Maximum 5 books per member
        }

        public int GetActiveLoanCount(Guid memberId) => _memberRepository.GetActiveLoanCount(memberId);

        public string GenerateLibraryCardNumber()
        {
            return $"LC{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
        }

        public string GenerateBarcode()
        {
            return $"BC{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";
        }
    }
} 