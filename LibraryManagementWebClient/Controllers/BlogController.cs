using LibraryManagementWebClient.Models;
using LibraryManagementWebClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibraryManagementWebClient.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILibraryApiService _apiService;
        private readonly ILogger<BlogController> _logger;

        public BlogController(ILibraryApiService apiService, ILogger<BlogController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // Blog post list - accessible to everyone
        public async Task<IActionResult> Index(string? tag = null)
        {
            try
            {
                List<BlogPost> blogPosts;
                
                if (!string.IsNullOrEmpty(tag))
                {
                    blogPosts = await _apiService.GetBlogPostsByTagAsync(tag);
                }
                else
                {
                    blogPosts = await _apiService.GetPublishedBlogPostsAsync();
                }
                
                var viewModel = new BlogViewModel
                {
                    BlogPosts = blogPosts,
                    SelectedTag = tag
                };

                // Extract all tags from posts
                var allTags = new Dictionary<string, int>();
                var authors = new Dictionary<string, int>();
                
                foreach (var post in blogPosts)
                {
                    // Count tags
                    foreach (var postTag in post.TagList)
                    {
                        if (allTags.ContainsKey(postTag))
                        {
                            allTags[postTag]++;
                        }
                        else
                        {
                            allTags[postTag] = 1;
                        }
                    }
                    
                    // Count authors
                    var authorKey = post.AuthorId.ToString();
                    if (authors.ContainsKey(authorKey))
                    {
                        authors[authorKey]++;
                    }
                    else
                    {
                        authors[authorKey] = 1;
                    }
                }
                
                viewModel.Tags = allTags;
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog posts");
                TempData["ErrorMessage"] = "Error retrieving blog posts. Please try again later.";
                return View(new BlogViewModel());
            }
        }

        // Blog post details - accessible to everyone
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var blogPost = await _apiService.GetBlogPostByIdAsync(id);
                
                if (blogPost == null)
                {
                    return NotFound();
                }
                
                // Only staff can view unpublished posts
                if (blogPost.Status != "Published" && !User.IsInRole("Staff"))
                {
                    return Forbid();
                }
                
                return View(blogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog post details");
                TempData["ErrorMessage"] = "Error retrieving blog post details. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Staff-only actions
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Manage()
        {
            try
            {
                var blogPosts = await _apiService.GetAllBlogPostsAsync();
                return View(blogPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog posts for management");
                TempData["ErrorMessage"] = "Error retrieving blog posts. Please try again later.";
                return View(new List<BlogPost>());
            }
        }

        [Authorize(Roles = "Staff")]
        public IActionResult Create()
        {
            return View(new BlogPost());
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPost blogPost)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(blogPost);
                }

                var newPost = await _apiService.CreateBlogPostAsync(blogPost);
                TempData["SuccessMessage"] = "Blog post created successfully.";
                return RedirectToAction(nameof(Manage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blog post");
                TempData["ErrorMessage"] = "Error creating blog post. Please try again.";
                return View(blogPost);
            }
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var blogPost = await _apiService.GetBlogPostByIdAsync(id);
                
                if (blogPost == null)
                {
                    return NotFound();
                }
                
                return View(blogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog post for editing");
                TempData["ErrorMessage"] = "Error retrieving blog post. Please try again later.";
                return RedirectToAction(nameof(Manage));
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BlogPost blogPost)
        {
            try
            {
                if (id != blogPost.BlogPostId)
                {
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    return View(blogPost);
                }

                await _apiService.UpdateBlogPostAsync(id, blogPost);
                TempData["SuccessMessage"] = "Blog post updated successfully.";
                return RedirectToAction(nameof(Manage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blog post");
                TempData["ErrorMessage"] = "Error updating blog post. Please try again.";
                return View(blogPost);
            }
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var blogPost = await _apiService.GetBlogPostByIdAsync(id);
                
                if (blogPost == null)
                {
                    return NotFound();
                }
                
                return View(blogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog post for deletion");
                TempData["ErrorMessage"] = "Error retrieving blog post. Please try again later.";
                return RedirectToAction(nameof(Manage));
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _apiService.DeleteBlogPostAsync(id);
                TempData["SuccessMessage"] = "Blog post deleted successfully.";
                return RedirectToAction(nameof(Manage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog post");
                TempData["ErrorMessage"] = "Error deleting blog post. Please try again.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
} 