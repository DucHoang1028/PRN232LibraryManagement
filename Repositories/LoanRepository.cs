using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class LoanRepository : ILoanRepository
    {
        public List<Loan> GetLoans() => LoanDAO.GetLoans();
        public Loan? GetLoanById(Guid loanId) => LoanDAO.GetLoanById(loanId);
        public List<Loan> GetActiveLoans() => LoanDAO.GetActiveLoans();
        public List<Loan> GetOverdueLoans() => LoanDAO.GetOverdueLoans();
        public List<Loan> GetLoansByMember(Guid memberId) => LoanDAO.GetLoansByMember(memberId);
        public List<Loan> GetLoansByBook(Guid bookId) => LoanDAO.GetLoansByBook(bookId);
        public Loan CreateLoan(Loan loan) => LoanDAO.CreateLoan(loan);
        public Loan UpdateLoan(Guid loanId, Loan loan) => LoanDAO.UpdateLoan(loanId, loan);
        public bool DeleteLoan(Guid loanId) => LoanDAO.DeleteLoan(loanId);
        public List<Loan> GetLoansDueToday() => LoanDAO.GetLoansDueToday();
        public List<Loan> GetLoansDueInDays(int days) => LoanDAO.GetLoansDueInDays(days);
    }
} 