using System.Collections;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Book>>> ListBooks()
        {
            try
            {
                var books = _bookService.GetBooks();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Books/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(Guid id)
        {
            try
            {
                var book = _bookService.GetBookById(id);
                if (book == null)
                    return NotFound(new { message = "Book not found" });

                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Books/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks(
            [FromQuery] string? title = null,
            [FromQuery] string? author = null,
            [FromQuery] string? category = null,
            [FromQuery] DateTime? publicationDate = null)
        {
            try
            {
                var books = _bookService.SearchBooks(title, author, category, publicationDate);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Books/author/{authorId}
        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAuthor(Guid authorId)
        {
            try
            {
                var books = _bookService.GetBooksByAuthor(authorId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Books/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByCategory(Guid categoryId)
        {
            try
            {
                var books = _bookService.GetBooksByCategory(categoryId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Books/publisher/{publisherId}
        [HttpGet("publisher/{publisherId}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByPublisher(Guid publisherId)
        {
            try
            {
                var books = _bookService.GetBooksByPublisher(publisherId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/hasactiveloans")]
        public async Task<ActionResult<bool>> HasActiveLoans(Guid id)
        {
            try
            {
                var hasLoans = _bookService.HasActiveLoans(id);
                return Ok(hasLoans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
        {
            try
            {
                var createdBook = _bookService.CreateBook(book);
                return CreatedAtAction(nameof(GetBook), new { id = createdBook.BookId }, createdBook);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] Book book)
        {
            try
            {
                var updatedBook = _bookService.UpdateBook(id, book);
                return Ok(updatedBook);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message, hasActiveLoans = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            try
            {
                var result = _bookService.DeleteBook(id);
                if (!result)
                    return NotFound(new { message = "Book not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message, hasActiveLoans = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
