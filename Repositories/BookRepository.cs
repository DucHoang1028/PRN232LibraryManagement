using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class BookRepository : IBookRepository
    {
        public List<Book> GetBooks() => BookDAO.GetBooks();
        public Book? GetBookById(Guid bookId) => BookDAO.GetBookById(bookId);
        public List<Book> SearchBooks(string? title = null, string? author = null, string? category = null, DateTime? publicationDate = null) 
            => BookDAO.SearchBooks(title, author, category, publicationDate);
        public Book CreateBook(Book book) => BookDAO.CreateBook(book);
        public Book UpdateBook(Guid bookId, Book book) => BookDAO.UpdateBook(bookId, book);
        public bool DeleteBook(Guid bookId) => BookDAO.DeleteBook(bookId);
        public List<Book> GetBooksByAuthor(Guid authorId) => BookDAO.GetBooksByAuthor(authorId);
        public List<Book> GetBooksByCategory(Guid categoryId) => BookDAO.GetBooksByCategory(categoryId);
        public List<Book> GetBooksByPublisher(Guid publisherId) => BookDAO.GetBooksByPublisher(publisherId);
        public bool UpdateBookAvailability(Guid bookId, int changeInCopies) => BookDAO.UpdateBookAvailability(bookId, changeInCopies);
    }
}
