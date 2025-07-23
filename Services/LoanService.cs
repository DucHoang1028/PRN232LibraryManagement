using BusinessObjects;
using Repositories;

namespace Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IFineService _fineService;

        public LoanService(ILoanRepository loanRepository, IBookRepository bookRepository, IMemberRepository memberRepository, IFineService fineService)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _fineService = fineService;
        }

        public List<Loan> GetLoans() => _loanRepository.GetLoans();

        public Loan? GetLoanById(Guid loanId) => _loanRepository.GetLoanById(loanId);

        public List<Loan> GetActiveLoans() => _loanRepository.GetActiveLoans();

        public List<Loan> GetOverdueLoans() => _loanRepository.GetOverdueLoans();

        public List<Loan> GetLoansByMember(Guid memberId) => _loanRepository.GetLoansByMember(memberId);

        public List<Loan> GetLoansByBook(Guid bookId) => _loanRepository.GetLoansByBook(bookId);

        public Loan CheckoutBook(Guid bookId, Guid memberId)
        {
            // Validate book exists and is available
            if (!IsBookAvailable(bookId))
                throw new InvalidOperationException("Book is not available for checkout");

            // Validate member is eligible
            if (!IsMemberEligibleForCheckout(memberId))
                throw new InvalidOperationException("Member is not eligible for checkout");

            // Validate member doesn't have overdue books
            if (HasOverdueBooks(memberId))
                throw new InvalidOperationException("You have overdue books. Please return them before checking out new books.");
                
            // Check if member already has this book checked out
            var memberLoans = _loanRepository.GetLoansByMember(memberId);
            if (memberLoans.Any(l => l.BookId == bookId && l.Status == "Active"))
                throw new InvalidOperationException("You already have this book checked out.");

            // Create loan
            var loan = new Loan
            {
                BookId = bookId,
                MemberId = memberId,
                CheckoutDate = DateTime.UtcNow,
                DueDate = CalculateDueDate(),
                Status = "Active"
            };

            var createdLoan = _loanRepository.CreateLoan(loan);

            // Update book availability
            _bookRepository.UpdateBookAvailability(bookId, -1);

            return createdLoan;
        }

        public bool ReturnBook(Guid loanId)
        {
            var loan = _loanRepository.GetLoanById(loanId);
            if (loan == null)
                return false;

            if (loan.Status != "Active")
                throw new InvalidOperationException("Loan is not active");

            // Update loan status
            loan.Status = "Returned";
            loan.ReturnDate = DateTime.UtcNow;
            _loanRepository.UpdateLoan(loanId, loan);

            // Update book availability
            _bookRepository.UpdateBookAvailability(loan.BookId, 1);

            return true;
        }

        public bool RenewBook(Guid loanId)
        {
            var loan = _loanRepository.GetLoanById(loanId);
            if (loan == null)
                return false;

            if (loan.Status != "Active")
                throw new InvalidOperationException("Loan is not active");

            if (loan.IsRenewed)
                throw new InvalidOperationException("Book has already been renewed");

            // Check if book is overdue
            if (loan.DueDate < DateTime.Today)
                throw new InvalidOperationException("Cannot renew overdue book");

            // Renew loan
            loan.IsRenewed = true;
            loan.RenewalCount++;
            loan.DueDate = CalculateDueDate();
            _loanRepository.UpdateLoan(loanId, loan);

            return true;
        }

        public Loan UpdateLoan(Guid loanId, Loan loan)
        {
            var existingLoan = _loanRepository.GetLoanById(loanId);
            if (existingLoan == null)
                throw new ArgumentException("Loan not found");

            // Update the loan
            loan.LoanId = loanId; // Ensure the ID is preserved
            loan.UpdatedDate = DateTime.UtcNow;
            
            return _loanRepository.UpdateLoan(loanId, loan);
        }

        public bool DeleteLoan(Guid loanId)
        {
            var loan = _loanRepository.GetLoanById(loanId);
            if (loan == null)
                return false;

            // If the loan is active, update book availability
            if (loan.Status == "Active")
            {
                _bookRepository.UpdateBookAvailability(loan.BookId, 1);
            }

            return _loanRepository.DeleteLoan(loanId);
        }

        public bool IsBookAvailable(Guid bookId)
        {
            var book = _bookRepository.GetBookById(bookId);
            return book != null && book.IsActive && book.AvailableCopies > 0;
        }

        public bool IsMemberEligibleForCheckout(Guid memberId)
        {
            var member = _memberRepository.GetMemberById(memberId);
            if (member == null || !member.IsActive)
                return false;

            // Check active loan count (maximum 5 books per member)
            var activeLoanCount = _memberRepository.GetActiveLoanCount(memberId);
            if (activeLoanCount >= 5)
                return false;

            // Check for overdue books
            if (HasOverdueBooks(memberId))
                return false;

            return true;
        }

        public bool HasOverdueBooks(Guid memberId)
        {
            var loans = _loanRepository.GetLoansByMember(memberId);
            
            // Check for any active loans that are past their due date
            return loans.Any(loan => 
                loan.Status == "Active" && 
                loan.DueDate < DateTime.Today);
        }

        public bool HasUnreturnedBooks(Guid memberId)
        {
            // Check if member has any active loans
            var activeLoanCount = _memberRepository.GetActiveLoanCount(memberId);
            return activeLoanCount > 0;
        }

        public DateTime CalculateDueDate()
        {
            // Default loan period is 10 days
            return DateTime.UtcNow.AddDays(10);
        }

        public void ProcessOverdueLoans()
        {
            // Use the FineService to process overdue fines
            _fineService.ProcessOverdueFines();
        }

        public List<Loan> GetLoansDueToday() => _loanRepository.GetLoansDueToday();

        public List<Loan> GetLoansDueInDays(int days) => _loanRepository.GetLoansDueInDays(days);
    }
} 