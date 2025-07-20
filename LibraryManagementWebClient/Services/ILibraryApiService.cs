using LibraryManagementWebClient.Models;

namespace LibraryManagementWebClient.Services
{
    public interface ILibraryApiService
    {
        // Books
        Task<List<Book>> GetBooksAsync();
        Task<Book?> GetBookAsync(Guid id);
        Task<Book?> GetBookByIdAsync(Guid id);
        Task<Book> CreateBookAsync(Book book);
        Task<Book> UpdateBookAsync(Guid id, Book book);
        Task<bool> DeleteBookAsync(Guid id);
        Task<List<Book>> SearchBooksAsync(string? title = null, string? author = null, string? category = null, DateTime? publicationDate = null);

        // Publishers
        Task<List<Publisher>> GetPublishersAsync();
        Task<Publisher?> GetPublisherAsync(Guid id);
        Task<Publisher> CreatePublisherAsync(Publisher publisher);
        Task<Publisher> UpdatePublisherAsync(Guid id, Publisher publisher);
        Task<bool> DeletePublisherAsync(Guid id);

        // Members
        Task<List<Member>> GetMembersAsync();
        Task<Member?> GetMemberAsync(Guid id);
        Task<Member?> GetMemberByEmailAsync(string email);
        Task<Member> CreateMemberAsync(Member member);
        Task<Member> UpdateMemberAsync(Guid id, Member member);
        Task<bool> DeleteMemberAsync(Guid id);
        Task<List<Loan>> GetMemberLoansAsync(Guid memberId);
        Task<List<Fine>> GetMemberFinesAsync(Guid memberId);
        Task<bool> CanMemberCheckoutAsync(Guid memberId);
        Task<int> GetMemberActiveLoanCountAsync(Guid memberId);

        // Loans
        Task<List<Loan>> GetLoansAsync();
        Task<Loan?> GetLoanAsync(Guid id);
        Task<List<Loan>> GetActiveLoansAsync();
        Task<List<Loan>> GetOverdueLoansAsync();
        Task<Loan> CheckoutBookAsync(Guid bookId, Guid memberId);
        Task<bool> ReturnBookAsync(Guid id);
        Task<bool> RenewBookAsync(Guid id);
        Task<Loan> UpdateLoanAsync(Guid id, Loan loan);
        Task<bool> DeleteLoanAsync(Guid id);
    }
} 