using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Member>>> ListMembers()
        {
            try
            {
                var members = _memberService.GetMembers();
                return Ok(members);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Member>> GetMember(Guid id)
        {
            try
            {
                var member = _memberService.GetMemberById(id);
                if (member == null)
                    return NotFound(new { message = "Member not found" });

                return Ok(member);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Members/email/{email}
        [HttpGet("email/{email}")]
        public async Task<ActionResult<Member>> GetMemberByEmail(string email)
        {
            try
            {
                var member = _memberService.GetMemberByEmail(email);
                if (member == null)
                    return NotFound(new { message = "Member not found" });

                return Ok(member);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Members/card/{libraryCardNumber}
        [HttpGet("card/{libraryCardNumber}")]
        public async Task<ActionResult<Member>> GetMemberByLibraryCard(string libraryCardNumber)
        {
            try
            {
                var member = _memberService.GetMemberByLibraryCard(libraryCardNumber);
                if (member == null)
                    return NotFound(new { message = "Member not found" });

                return Ok(member);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("loans/{id}")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetMemberLoans(Guid id)
        {
            try
            {
                var loans = _memberService.GetMemberLoans(id);
                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("reservations/{id}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetMemberReservations(Guid id)
        {
            try
            {
                var reservations = _memberService.GetMemberReservations(id);
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("fines/{id}")]
        public async Task<ActionResult<IEnumerable<Fine>>> GetMemberFines(Guid id)
        {
            try
            {
                var fines = _memberService.GetMemberFines(id);
                return Ok(fines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("cancheckout/{id}")]
        public async Task<ActionResult<bool>> CanCheckoutBook(Guid id)
        {
            try
            {
                var canCheckout = _memberService.CanCheckoutBook(id);
                return Ok(canCheckout);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("activeloancount/{id}")]
        public async Task<ActionResult<int>> GetActiveLoanCount(Guid id)
        {
            try
            {
                var count = _memberService.GetActiveLoanCount(id);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<Member>> CreateMember([FromBody] Member member)
        {
            try
            {
                var createdMember = _memberService.CreateMember(member);
                return CreatedAtAction(nameof(GetMember), new { id = createdMember.MemberId }, createdMember);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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
        public async Task<IActionResult> UpdateMember(Guid id, [FromBody] Member member)
        {
            try
            {
                var updatedMember = _memberService.UpdateMember(id, member);
                return Ok(updatedMember);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMember(Guid id)
        {
            try
            {
                var result = _memberService.DeleteMember(id);
                if (!result)
                    return NotFound(new { message = "Member not found" });

                return NoContent();
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

        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateMember(Guid id)
        {
            try
            {
                var result = _memberService.DeactivateMember(id);
                if (!result)
                    return NotFound(new { message = "Member not found" });

                return Ok(new { message = "Member deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
} 