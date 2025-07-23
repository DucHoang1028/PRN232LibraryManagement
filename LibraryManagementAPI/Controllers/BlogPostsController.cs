using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IMemberService _memberService;

        public BlogPostsController(IBlogService blogService, IMemberService memberService)
        {
            _blogService = blogService;
            _memberService = memberService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            if (User.IsInRole("Staff"))
            {
                // Staff can see all blog posts including drafts
                var posts = await _blogService.GetAllBlogPostsAsync();
                return Ok(posts);
            }
            else
            {
                // Non-staff users can only see published posts
                var posts = await _blogService.GetPublishedBlogPostsAsync();
                return Ok(posts);
            }
        }

        [HttpGet("published")]
        public async Task<IActionResult> GetPublishedBlogPosts()
        {
            var posts = await _blogService.GetPublishedBlogPostsAsync();
            return Ok(posts);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetBlogPost(Guid id)
        {
            var post = await _blogService.GetBlogPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            if (post.Status != "Published" && !User.IsInRole("Staff"))
            {
                return Forbid();
            }

            return Ok(post);
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetBlogPostsByAuthor(Guid authorId)
        {
            if (User.IsInRole("Staff"))
            {
                var posts = await _blogService.GetBlogPostsByAuthorAsync(authorId);
                return Ok(posts);
            }
            else
            {
                var posts = await _blogService.GetBlogPostsByAuthorAsync(authorId);
                posts.RemoveAll(p => p.Status != "Published");
                return Ok(posts);
            }
        }

        [HttpGet("tag/{tag}")]
        public async Task<IActionResult> GetBlogPostsByTag(string tag)
        {
            var posts = await _blogService.GetBlogPostsByTagAsync(tag);
            return Ok(posts);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBlogPost([FromBody] BlogPost blogPost)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Using a hardcoded member ID for testing instead of getting from user
                // Get the first member from the database
                var members = _memberService.GetMembers();
                if (members == null || members.Count == 0)
                {
                    return BadRequest("No members found in the database.");
                }
                
                // Use the first member's ID
                blogPost.AuthorId = members[0].MemberId;
                blogPost.BlogPostId = Guid.NewGuid();
                
                if (blogPost.Status == "Published")
                {
                    blogPost.PublishedDate = DateTime.UtcNow;
                }

                var createdPost = await _blogService.CreateBlogPostAsync(blogPost);
                return CreatedAtAction(nameof(GetBlogPost), new { id = createdPost.BlogPostId }, createdPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBlogPost(Guid id, [FromBody] BlogPost blogPost)
        {
            try
            {
                if (id != blogPost.BlogPostId)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingPost = await _blogService.GetBlogPostByIdAsync(id);
                if (existingPost == null)
                {
                    return NotFound();
                }

                if (existingPost.Status != "Published" && blogPost.Status == "Published")
                {
                    blogPost.PublishedDate = DateTime.UtcNow;
                }
                else
                {
                    blogPost.PublishedDate = existingPost.PublishedDate;
                }

                blogPost.AuthorId = existingPost.AuthorId;
                blogPost.CreatedDate = existingPost.CreatedDate;

                var updatedPost = await _blogService.UpdateBlogPostAsync(blogPost);
                return Ok(updatedPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBlogPost(Guid id)
        {
            try
            {
                var post = await _blogService.GetBlogPostByIdAsync(id);
                if (post == null)
                {
                    return NotFound();
                }

                await _blogService.DeleteBlogPostAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 