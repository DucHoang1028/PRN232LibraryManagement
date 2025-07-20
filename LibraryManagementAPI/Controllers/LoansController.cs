using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Loan>>> ListLoans()
        {
            try
            {
                var loans = _loanService.GetLoans();
                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Loan>> GetLoan(Guid id)
        {
            try
            {
                var loan = _loanService.GetLoanById(id);
                if (loan == null)
                    return NotFound(new { message = "Loan not found" });

                return Ok(loan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetActiveLoans()
        {
            try
            {
                var loans = _loanService.GetActiveLoans();
                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Loans/overdue
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetOverdueLoans()
        {
            try
            {
                var loans = _loanService.GetOverdueLoans();
                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Loans/member/{memberId}
        [HttpGet("member/{memberId}")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoansByMember(Guid memberId)
        {
            try
            {
                var loans = _loanService.GetLoansByMember(memberId);
                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Loans/book/{bookId}
        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoansByBook(Guid bookId)
        {
            try
            {
                var loans = _loanService.GetLoansByBook(bookId);
                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Loans/due-today
        [HttpGet("due-today")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoansDueToday()
        {
            try
            {
                var loans = _loanService.GetLoansDueToday();
                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Loans/due-in/{days}
        [HttpGet("due-in/{days}")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoansDueInDays(int days)
        {
            try
            {
                var loans = _loanService.GetLoansDueInDays(days);
                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/Loans/checkout
        [HttpPost("checkout")]
        public async Task<ActionResult<Loan>> CheckoutBook([FromBody] CheckoutRequest request)
        {
            try
            {
                var loan = _loanService.CheckoutBook(request.BookId, request.MemberId);
                return CreatedAtAction(nameof(GetLoan), new { id = loan.LoanId }, loan);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("return/{id}")]
        public async Task<IActionResult> ReturnBook(Guid id)
        {
            try
            {
                var result = _loanService.ReturnBook(id);
                if (!result)
                    return NotFound(new { message = "Loan not found" });

                return Ok(new { message = "Book returned successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("renew/{id}")]
        public async Task<IActionResult> RenewBook(Guid id)
        {
            try
            {
                var result = _loanService.RenewBook(id);
                if (!result)
                    return NotFound(new { message = "Loan not found" });

                return Ok(new { message = "Book renewed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateLoan(Guid id, [FromBody] Loan loan)
        {
            try
            {
                var updatedLoan = _loanService.UpdateLoan(id, loan);
                return Ok(updatedLoan);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLoan(Guid id)
        {
            try
            {
                var result = _loanService.DeleteLoan(id);
                if (!result)
                    return NotFound(new { message = "Loan not found" });

                return Ok(new { message = "Loan deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("bookavailable/{bookId}")]
        public async Task<ActionResult<bool>> IsBookAvailable(Guid bookId)
        {
            try
            {
                var isAvailable = _loanService.IsBookAvailable(bookId);
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("membereligible/{memberId}")]
        public async Task<ActionResult<bool>> IsMemberEligibleForCheckout(Guid memberId)
        {
            try
            {
                var isEligible = _loanService.IsMemberEligibleForCheckout(memberId);
                return Ok(isEligible);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/Loans/process-overdue
        [HttpPost("process-overdue")]
        public async Task<IActionResult> ProcessOverdueLoans()
        {
            try
            {
                _loanService.ProcessOverdueLoans();
                return Ok(new { message = "Overdue loans processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class CheckoutRequest
    {
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }
    }
} 