using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IBlogService
    {
        Task<List<BlogPost>> GetAllBlogPostsAsync();
        Task<List<BlogPost>> GetPublishedBlogPostsAsync();
        Task<BlogPost?> GetBlogPostByIdAsync(Guid blogPostId);
        Task<List<BlogPost>> GetBlogPostsByAuthorAsync(Guid authorId);
        Task<List<BlogPost>> GetBlogPostsByTagAsync(string tag);
        Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost);
        Task<BlogPost> UpdateBlogPostAsync(BlogPost blogPost);
        Task DeleteBlogPostAsync(Guid blogPostId);
    }
} 