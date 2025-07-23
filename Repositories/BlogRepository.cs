using BusinessObjects;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetPublishedBlogPostsAsync()
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .Where(b => b.Status == "Published")
                .OrderByDescending(b => b.PublishedDate)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(Guid blogPostId)
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.BlogPostId == blogPostId);
        }

        public async Task<List<BlogPost>> GetBlogPostsByAuthorAsync(Guid authorId)
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .Where(b => b.AuthorId == authorId)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetBlogPostsByTagAsync(string tag)
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .Where(b => b.Status == "Published" && b.Tags != null && b.Tags.Contains(tag))
                .OrderByDescending(b => b.PublishedDate)
                .ToListAsync();
        }

        public async Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost> UpdateBlogPostAsync(BlogPost blogPost)
        {
            blogPost.UpdatedDate = DateTime.UtcNow;
            
            if (blogPost.Status == "Published" && blogPost.PublishedDate == default)
            {
                blogPost.PublishedDate = DateTime.UtcNow;
            }
            
            _context.Entry(blogPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return blogPost;
        }

        public async Task DeleteBlogPostAsync(Guid blogPostId)
        {
            var blogPost = await _context.BlogPosts.FindAsync(blogPostId);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
                await _context.SaveChangesAsync();
            }
        }
    }
} 