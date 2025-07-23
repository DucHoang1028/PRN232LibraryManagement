using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public List<Book> GetBooks() => _bookRepository.GetBooks();

        public Book? GetBookById(Guid bookId) => _bookRepository.GetBookById(bookId);

        public List<Book> SearchBooks(string? title = null, string? author = null, string? category = null, DateTime? publicationDate = null)
        {
            return _bookRepository.SearchBooks(title, author, category, publicationDate);
        }

        public Book CreateBook(Book book)
        {
            // Validate book data
            if (string.IsNullOrEmpty(book.Title))
                throw new ArgumentException("Book title is required");

            if (string.IsNullOrEmpty(book.Isbn))
                throw new ArgumentException("ISBN is required");

            if (book.TotalCopies < 0)
                throw new ArgumentException("Total copies cannot be negative");

            if (book.AvailableCopies > book.TotalCopies)
                book.AvailableCopies = book.TotalCopies;

            return _bookRepository.CreateBook(book);
        }

        public Book UpdateBook(Guid bookId, Book book)
        {
            var existingBook = _bookRepository.GetBookById(bookId);
            if (existingBook == null)
                throw new ArgumentException("Book not found");

            // Check if book has active loans
            if (HasActiveLoans(bookId))
                throw new InvalidOperationException("Cannot modify book details while it has active loans");

            // Validate book data
            if (string.IsNullOrEmpty(book.Title))
                throw new ArgumentException("Book title is required");

            if (string.IsNullOrEmpty(book.Isbn))
                throw new ArgumentException("ISBN is required");

            if (book.TotalCopies < 0)
                throw new ArgumentException("Total copies cannot be negative");

            if (book.AvailableCopies > book.TotalCopies)
                book.AvailableCopies = book.TotalCopies;

            return _bookRepository.UpdateBook(bookId, book);
        }

        public bool DeleteBook(Guid bookId)
        {
            var book = _bookRepository.GetBookById(bookId);
            if (book == null)
                return false;

            // Check if book has active loans
            if (HasActiveLoans(bookId))
                throw new InvalidOperationException("Cannot delete book with active loans");

            return _bookRepository.DeleteBook(bookId);
        }

        public List<Book> GetBooksByAuthor(Guid authorId) => _bookRepository.GetBooksByAuthor(authorId);

        public List<Book> GetBooksByCategory(Guid categoryId) => _bookRepository.GetBooksByCategory(categoryId);

        public List<Book> GetBooksByPublisher(Guid publisherId) => _bookRepository.GetBooksByPublisher(publisherId);

        public bool UpdateBookAvailability(Guid bookId, int changeInCopies)
        {
            var book = _bookRepository.GetBookById(bookId);
            if (book == null)
                return false;

            return _bookRepository.UpdateBookAvailability(bookId, changeInCopies);
        }
        
        public bool HasActiveLoans(Guid bookId)
        {
            var book = _bookRepository.GetBookById(bookId);
            if (book == null)
                return false;
                
            return book.Loans != null && book.Loans.Any(l => l.Status == "Active");
        }
    }
}
