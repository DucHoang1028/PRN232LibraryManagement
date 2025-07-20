using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class BookDAO
    {
        public static List<Book> GetBooks()
        {
            var listBooks = new List<Book>();
            try
            {
                using var context = new ApplicationDbContext();
                listBooks = context.Books
                    .Include(b => b.Publisher)
                    .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                    .Where(b => b.IsActive)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listBooks;
        }

        public static Book? GetBookById(Guid bookId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Books
                    .Include(b => b.Publisher)
                    .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                    .FirstOrDefault(b => b.BookId == bookId && b.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Book> SearchBooks(string? title = null, string? author = null, string? category = null, DateTime? publicationDate = null)
        {
            try
            {
                using var context = new ApplicationDbContext();
                var query = context.Books
                    .Include(b => b.Publisher)
                    .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                    .Where(b => b.IsActive);

                if (!string.IsNullOrEmpty(title))
                    query = query.Where(b => b.Title.Contains(title));

                if (!string.IsNullOrEmpty(author))
                    query = query.Where(b => b.BookAuthors.Any(ba => 
                        ba.Author.FirstName.Contains(author) || 
                        ba.Author.LastName.Contains(author)));

                if (!string.IsNullOrEmpty(category))
                    query = query.Where(b => b.BookCategories.Any(bc => 
                        bc.Category.Name.Contains(category)));

                if (publicationDate.HasValue)
                    query = query.Where(b => b.PublicationDate.Date == publicationDate.Value.Date);

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Book CreateBook(Book book)
        {
            try
            {
                using var context = new ApplicationDbContext();
                book.BookId = Guid.NewGuid();
                book.CreatedDate = DateTime.UtcNow;
                book.IsActive = true;
                context.Books.Add(book);
                context.SaveChanges();
                return book;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Book UpdateBook(Guid bookId, Book book)
        {
            try
            {
                using var context = new ApplicationDbContext();
                var existingBook = context.Books.Find(bookId);
                if (existingBook == null)
                    throw new Exception("Book not found");

                existingBook.Title = book.Title;
                existingBook.Isbn = book.Isbn;
                existingBook.PublisherId = book.PublisherId;
                existingBook.Description = book.Description;
                existingBook.PublicationDate = book.PublicationDate;
                existingBook.Language = book.Language;
                existingBook.TotalCopies = book.TotalCopies;
                existingBook.AvailableCopies = book.AvailableCopies;
                existingBook.RackNumber = book.RackNumber;
                existingBook.Barcode = book.Barcode;
                existingBook.UpdatedDate = DateTime.UtcNow;

                context.SaveChanges();
                return existingBook;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool DeleteBook(Guid bookId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                var book = context.Books.Find(bookId);
                if (book == null)
                    return false;

                book.IsActive = false;
                book.UpdatedDate = DateTime.UtcNow;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Book> GetBooksByAuthor(Guid authorId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Books
                    .Include(b => b.Publisher)
                    .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                    .Where(b => b.IsActive && b.BookAuthors.Any(ba => ba.AuthorId == authorId))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Book> GetBooksByCategory(Guid categoryId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Books
                    .Include(b => b.Publisher)
                    .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                    .Where(b => b.IsActive && b.BookCategories.Any(bc => bc.CategoryId == categoryId))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Book> GetBooksByPublisher(Guid publisherId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Books
                    .Include(b => b.Publisher)
                    .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                    .Where(b => b.IsActive && b.PublisherId == publisherId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool UpdateBookAvailability(Guid bookId, int changeInCopies)
        {
            try
            {
                using var context = new ApplicationDbContext();
                var book = context.Books.Find(bookId);
                if (book == null)
                    return false;

                book.AvailableCopies += changeInCopies;
                if (book.AvailableCopies < 0)
                    book.AvailableCopies = 0;
                if (book.AvailableCopies > book.TotalCopies)
                    book.AvailableCopies = book.TotalCopies;

                book.UpdatedDate = DateTime.UtcNow;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
