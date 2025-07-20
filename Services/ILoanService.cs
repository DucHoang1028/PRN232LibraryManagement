using BusinessObjects;

namespace Services
{
    public interface ILoanService
    {
        List<Loan> GetLoans();
        Loan? GetLoanById(Guid loanId);
        List<Loan> GetActiveLoans();
        List<Loan> GetOverdueLoans();
        List<Loan> GetLoansByMember(Guid memberId);
        List<Loan> GetLoansByBook(Guid bookId);
        Loan CheckoutBook(Guid bookId, Guid memberId);
        bool ReturnBook(Guid loanId);
        bool RenewBook(Guid loanId);
        Loan UpdateLoan(Guid loanId, Loan loan);
        bool DeleteLoan(Guid loanId);
        bool IsBookAvailable(Guid bookId);
        bool IsMemberEligibleForCheckout(Guid memberId);
        DateTime CalculateDueDate();
        void ProcessOverdueLoans();
        List<Loan> GetLoansDueToday();
        List<Loan> GetLoansDueInDays(int days);
    }
} 