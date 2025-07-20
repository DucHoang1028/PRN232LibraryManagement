using BusinessObjects;

namespace Services
{
    public interface IFineService
    {
        List<Fine> GetFines();
        Fine? GetFineById(Guid fineId);
        List<Fine> GetFinesByMember(Guid memberId);
        List<Fine> GetUnpaidFines();
        List<Fine> GetPaidFines();
        Fine CreateFine(Guid loanId, Guid memberId, decimal amount, string description);
        bool PayFine(Guid fineId);
        decimal CalculateFineAmount(DateTime dueDate, DateTime returnDate);
        decimal GetTotalFinesForMember(Guid memberId);
        decimal GetUnpaidFinesForMember(Guid memberId);
        void ProcessOverdueFines();
        List<Fine> GetFinesCreatedToday();
        bool HasUnpaidFines(Guid memberId);
    }
} 