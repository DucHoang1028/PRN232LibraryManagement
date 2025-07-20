using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinesController : ControllerBase
    {
        private readonly IFineService _fineService;

        public FinesController(IFineService fineService)
        {
            _fineService = fineService;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Fine>>> ListFines()
        {
            try
            {
                var fines = _fineService.GetFines();
                return Ok(fines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Fine>> GetFine(Guid id)
        {
            try
            {
                var fine = _fineService.GetFineById(id);
                if (fine == null)
                    return NotFound(new { message = "Fine not found" });

                return Ok(fine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("member/{memberId}")]
        public async Task<ActionResult<IEnumerable<Fine>>> GetFinesByMember(Guid memberId)
        {
            try
            {
                var fines = _fineService.GetFinesByMember(memberId);
                return Ok(fines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("unpaid")]
        public async Task<ActionResult<IEnumerable<Fine>>> GetUnpaidFines()
        {
            try
            {
                var fines = _fineService.GetUnpaidFines();
                return Ok(fines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("paid")]
        public async Task<ActionResult<IEnumerable<Fine>>> GetPaidFines()
        {
            try
            {
                var fines = _fineService.GetPaidFines();
                return Ok(fines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("pay/{id}")]
        public async Task<IActionResult> PayFine(Guid id)
        {
            try
            {
                var result = _fineService.PayFine(id);
                if (!result)
                    return NotFound(new { message = "Fine not found" });

                return Ok(new { message = "Fine paid successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("member/{memberId}/total")]
        public async Task<ActionResult<decimal>> GetTotalFinesForMember(Guid memberId)
        {
            try
            {
                var total = _fineService.GetTotalFinesForMember(memberId);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("member/{memberId}/unpaid")]
        public async Task<ActionResult<decimal>> GetUnpaidFinesForMember(Guid memberId)
        {
            try
            {
                var unpaid = _fineService.GetUnpaidFinesForMember(memberId);
                return Ok(unpaid);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("member/{memberId}/has-unpaid")]
        public async Task<ActionResult<bool>> HasUnpaidFines(Guid memberId)
        {
            try
            {
                var hasUnpaid = _fineService.HasUnpaidFines(memberId);
                return Ok(hasUnpaid);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<Fine>>> GetFinesCreatedToday()
        {
            try
            {
                var fines = _fineService.GetFinesCreatedToday();
                return Ok(fines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("process-overdue")]
        public async Task<IActionResult> ProcessOverdueFines()
        {
            try
            {
                _fineService.ProcessOverdueFines();
                return Ok(new { message = "Overdue fines processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
} 