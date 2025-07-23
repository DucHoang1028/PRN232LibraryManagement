using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;

        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            return await _blogRepository.GetAllBlogPostsAsync();
        }

        public async Task<List<BlogPost>> GetPublishedBlogPostsAsync()
        {
            return await _blogRepository.GetPublishedBlogPostsAsync();
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(Guid blogPostId)
        {
            return await _blogRepository.GetBlogPostByIdAsync(blogPostId);
        }

        public async Task<List<BlogPost>> GetBlogPostsByAuthorAsync(Guid authorId)
        {
            return await _blogRepository.GetBlogPostsByAuthorAsync(authorId);
        }

        public async Task<List<BlogPost>> GetBlogPostsByTagAsync(string tag)
        {
            return await _blogRepository.GetBlogPostsByTagAsync(tag);
        }

        public async Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost)
        {
            return await _blogRepository.CreateBlogPostAsync(blogPost);
        }

        public async Task<BlogPost> UpdateBlogPostAsync(BlogPost blogPost)
        {
            var existingPost = await _blogRepository.GetBlogPostByIdAsync(blogPost.BlogPostId);
            if (existingPost == null)
            {
                throw new Exception($"Blog post with ID {blogPost.BlogPostId} not found.");
            }
            
            return await _blogRepository.UpdateBlogPostAsync(blogPost);
        }

        public async Task DeleteBlogPostAsync(Guid blogPostId)
        {
            var existingPost = await _blogRepository.GetBlogPostByIdAsync(blogPostId);
            if (existingPost == null)
            {
                throw new Exception($"Blog post with ID {blogPostId} not found.");
            }
            
            await _blogRepository.DeleteBlogPostAsync(blogPostId);
        }
    }
} 