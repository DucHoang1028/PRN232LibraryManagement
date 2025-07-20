using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Publisher>>> ListPublishers()
        {
            try
            {
                var publishers = _publisherService.GetPublishers();
                return Ok(publishers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Publisher>> GetPublisher(Guid id)
        {
            try
            {
                var publisher = _publisherService.GetPublisherById(id);
                if (publisher == null)
                    return NotFound(new { message = "Publisher not found" });

                return Ok(publisher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("byname/{name}")]
        public async Task<ActionResult<Publisher>> GetPublisherByName(string name)
        {
            try
            {
                var publisher = _publisherService.GetPublisherByName(name);
                if (publisher == null)
                    return NotFound(new { message = "Publisher not found" });

                return Ok(publisher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<Publisher>> CreatePublisher([FromBody] Publisher publisher)
        {
            try
            {
                var createdPublisher = _publisherService.CreatePublisher(publisher);
                return CreatedAtAction(nameof(GetPublisher), new { id = createdPublisher.PublisherId }, createdPublisher);
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
        public async Task<IActionResult> UpdatePublisher(Guid id, [FromBody] Publisher publisher)
        {
            try
            {
                var updatedPublisher = _publisherService.UpdatePublisher(id, publisher);
                return Ok(updatedPublisher);
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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePublisher(Guid id)
        {
            try
            {
                var result = _publisherService.DeletePublisher(id);
                if (!result)
                    return NotFound(new { message = "Publisher not found" });

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
    }
} 