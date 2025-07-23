using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Services
{
    public interface IBookService
    {
        List<Book> GetBooks();
        Book? GetBookById(Guid bookId);
        List<Book> SearchBooks(string? title = null, string? author = null, string? category = null, DateTime? publicationDate = null);
        Book CreateBook(Book book);
        Book UpdateBook(Guid bookId, Book book);
        bool DeleteBook(Guid bookId);
        List<Book> GetBooksByAuthor(Guid authorId);
        List<Book> GetBooksByCategory(Guid categoryId);
        List<Book> GetBooksByPublisher(Guid publisherId);
        bool UpdateBookAvailability(Guid bookId, int changeInCopies);
        bool HasActiveLoans(Guid bookId);
    }
}
