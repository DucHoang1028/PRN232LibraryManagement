using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class LoanDAO
    {
        public static List<Loan> GetLoans()
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Member)
                    .Include(l => l.Fine)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Loan? GetLoanById(Guid loanId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Member)
                    .Include(l => l.Fine)
                    .FirstOrDefault(l => l.LoanId == loanId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Loan> GetActiveLoans()
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Member)
                    .Include(l => l.Fine)
                    .Where(l => l.Status == "Active")
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Loan> GetOverdueLoans()
        {
            try
            {
                using var context = new ApplicationDbContext();
                var today = DateTime.Today;
                return context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Member)
                    .Include(l => l.Fine)
                    .Where(l => l.Status == "Active" && l.DueDate < today)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Loan> GetLoansByMember(Guid memberId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Member)
                    .Include(l => l.Fine)
                    .Where(l => l.MemberId == memberId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Loan> GetLoansByBook(Guid bookId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Member)
                    .Include(l => l.Fine)
                    .Where(l => l.BookId == bookId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Loan CreateLoan(Loan loan)
        {
            try
            {
                using var context = new ApplicationDbContext();
                loan.LoanId = Guid.NewGuid();
                loan.CreatedDate = DateTime.UtcNow;
                loan.Status = "Active";
                loan.CheckoutDate = DateTime.UtcNow;
                loan.DueDate = CalculateDueDate();
                loan.IsRenewed = false;
                loan.RenewalCount = 0;

                context.Loans.Add(loan);
                context.SaveChanges();
                return loan;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Loan UpdateLoan(Guid loanId, Loan loan)
        {
            try
            {
                using var context = new ApplicationDbContext();
                var existingLoan = context.Loans.Find(loanId);
                if (existingLoan == null)
                    throw new Exception("Loan not found");

                // Update all loan fields
                existingLoan.Status = loan.Status;
                existingLoan.CheckoutDate = loan.CheckoutDate;
                existingLoan.DueDate = loan.DueDate;
                existingLoan.ReturnDate = loan.ReturnDate;
                existingLoan.IsRenewed = loan.IsRenewed;
                existingLoan.RenewalCount = loan.RenewalCount;
                existingLoan.Notes = loan.Notes;
                existingLoan.IsActive = loan.IsActive;
                existingLoan.UpdatedDate = DateTime.UtcNow;

                context.SaveChanges();
                return existingLoan;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool DeleteLoan(Guid loanId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                var loan = context.Loans.Find(loanId);
                if (loan == null)
                    return false;

                context.Loans.Remove(loan);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Loan> GetLoansDueToday()
        {
            try
            {
                using var context = new ApplicationDbContext();
                var today = DateTime.Today;
                return context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Member)
                    .Include(l => l.Fine)
                    .Where(l => l.Status == "Active" && l.DueDate.Date == today)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Loan> GetLoansDueInDays(int days)
        {
            try
            {
                using var context = new ApplicationDbContext();
                var targetDate = DateTime.Today.AddDays(days);
                return context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Member)
                    .Include(l => l.Fine)
                    .Where(l => l.Status == "Active" && l.DueDate.Date == targetDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static DateTime CalculateDueDate()
        {
            // Default loan period is 10 days
            return DateTime.UtcNow.AddDays(10);
        }
    }
} 