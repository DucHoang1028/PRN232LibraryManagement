using BusinessObjects;

namespace Repositories
{
    public interface ILoanRepository
    {
        List<Loan> GetLoans();
        Loan? GetLoanById(Guid loanId);
        List<Loan> GetActiveLoans();
        List<Loan> GetOverdueLoans();
        List<Loan> GetLoansByMember(Guid memberId);
        List<Loan> GetLoansByBook(Guid bookId);
        Loan CreateLoan(Loan loan);
        Loan UpdateLoan(Guid loanId, Loan loan);
        bool DeleteLoan(Guid loanId);
        List<Loan> GetLoansDueToday();
        List<Loan> GetLoansDueInDays(int days);
    }
} 