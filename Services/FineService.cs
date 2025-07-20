using BusinessObjects;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class FineService : IFineService
    {
        public List<Fine> GetFines()
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Fines
                    .Include(f => f.Loan)
                    .Include(f => f.Member)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Fine? GetFineById(Guid fineId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Fines
                    .Include(f => f.Loan)
                    .Include(f => f.Member)
                    .FirstOrDefault(f => f.FineId == fineId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Fine> GetFinesByMember(Guid memberId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Fines
                    .Include(f => f.Loan)
                    .Include(f => f.Member)
                    .Where(f => f.MemberId == memberId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Fine> GetUnpaidFines()
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Fines
                    .Include(f => f.Loan)
                    .Include(f => f.Member)
                    .Where(f => !f.IsPaid)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Fine> GetPaidFines()
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Fines
                    .Include(f => f.Loan)
                    .Include(f => f.Member)
                    .Where(f => f.IsPaid)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Fine CreateFine(Guid loanId, Guid memberId, decimal amount, string description)
        {
            try
            {
                using var context = new ApplicationDbContext();
                var fine = new Fine
                {
                    FineId = Guid.NewGuid(),
                    LoanId = loanId,
                    MemberId = memberId,
                    Amount = amount,
                    Description = description,
                    Paid = false,
                    IsPaid = false,
                    CreatedDate = DateTime.UtcNow
                };

                context.Fines.Add(fine);
                context.SaveChanges();
                return fine;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool PayFine(Guid fineId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                var fine = context.Fines.Find(fineId);
                if (fine == null)
                    return false;

                fine.Paid = true;
                fine.IsPaid = true;
                fine.PaidDate = DateTime.UtcNow;
                fine.UpdatedDate = DateTime.UtcNow;

                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal CalculateFineAmount(DateTime dueDate, DateTime returnDate)
        {
            var daysOverdue = (returnDate - dueDate).Days;
            if (daysOverdue <= 0)
                return 0;

            // Default fine rate: $0.50 per day
            const decimal dailyRate = 0.50m;
            return daysOverdue * dailyRate;
        }

        public decimal GetTotalFinesForMember(Guid memberId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Fines
                    .Where(f => f.MemberId == memberId)
                    .Sum(f => f.Amount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetUnpaidFinesForMember(Guid memberId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Fines
                    .Where(f => f.MemberId == memberId && !f.IsPaid)
                    .Sum(f => f.Amount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ProcessOverdueFines()
        {
            try
            {
                using var context = new ApplicationDbContext();
                var overdueLoans = context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Member)
                    .Where(l => l.Status == "Active" && l.DueDate < DateTime.Today && l.Fine == null)
                    .ToList();

                foreach (var loan in overdueLoans)
                {
                    // Calculate fine amount
                    var fineAmount = CalculateFineAmount(loan.DueDate, DateTime.Today);
                    
                    if (fineAmount > 0)
                    {
                        // Create fine
                        var fine = new Fine
                        {
                            FineId = Guid.NewGuid(),
                            LoanId = loan.LoanId,
                            MemberId = loan.MemberId,
                            Amount = fineAmount,
                            Description = $"Overdue fine for {loan.Book?.Title ?? "Unknown Book"}",
                            Paid = false,
                            IsPaid = false,
                            CreatedDate = DateTime.UtcNow
                        };

                        context.Fines.Add(fine);
                        
                        // Update loan status to Overdue
                        loan.Status = "Overdue";
                        loan.UpdatedDate = DateTime.UtcNow;
                    }
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Fine> GetFinesCreatedToday()
        {
            try
            {
                using var context = new ApplicationDbContext();
                var today = DateTime.Today;
                return context.Fines
                    .Include(f => f.Loan)
                    .Include(f => f.Member)
                    .Where(f => f.CreatedDate.Date == today)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool HasUnpaidFines(Guid memberId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Fines
                    .Any(f => f.MemberId == memberId && !f.IsPaid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
} 