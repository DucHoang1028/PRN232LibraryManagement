using System.Text;
using System.Text.Json;
using LibraryManagementWebClient.Models;

namespace LibraryManagementWebClient.Services
{
    public class LibraryApiService : ILibraryApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public LibraryApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // Books
        public async Task<List<Book>> GetBooksAsync()
        {
            var response = await _httpClient.GetAsync("api/Books/list");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Book>>(content, _jsonOptions) ?? new List<Book>();
        }

        public async Task<Book?> GetBookAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/Books/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Book>(content, _jsonOptions);
            }
            return null;
        }

        public async Task<Book?> GetBookByIdAsync(Guid id)
        {
            return await GetBookAsync(id);
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            var json = JsonSerializer.Serialize(book, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Books/create", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Book>(responseContent, _jsonOptions) ?? book;
        }

        public async Task<Book> UpdateBookAsync(Guid id, Book book)
        {
            var json = JsonSerializer.Serialize(book, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Books/update/{id}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Book>(responseContent, _jsonOptions) ?? book;
        }

        public async Task<bool> DeleteBookAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/Books/delete/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Book>> SearchBooksAsync(string? title = null, string? author = null, string? category = null, DateTime? publicationDate = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(title)) queryParams.Add($"title={Uri.EscapeDataString(title)}");
            if (!string.IsNullOrEmpty(author)) queryParams.Add($"author={Uri.EscapeDataString(author)}");
            if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={Uri.EscapeDataString(category)}");
            if (publicationDate.HasValue) queryParams.Add($"publicationDate={publicationDate.Value:yyyy-MM-ddTHH:mm:ssZ}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"api/Books/search{queryString}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Book>>(content, _jsonOptions) ?? new List<Book>();
        }

        public async Task<bool> HasActiveLoansAsync(Guid bookId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Books/{bookId}/hasactiveloans");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<bool>(content, _jsonOptions);
            }
            catch
            {
                // If there's an error, assume there are no active loans
                return false;
            }
        }

        // Publishers
        public async Task<List<Publisher>> GetPublishersAsync()
        {
            var response = await _httpClient.GetAsync("api/Publishers/list");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Publisher>>(content, _jsonOptions) ?? new List<Publisher>();
        }

        public async Task<Publisher?> GetPublisherAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/Publishers/get/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Publisher>(content, _jsonOptions);
            }
            return null;
        }

        public async Task<Publisher> CreatePublisherAsync(Publisher publisher)
        {
            var json = JsonSerializer.Serialize(publisher, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Publishers/create", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Publisher>(responseContent, _jsonOptions) ?? publisher;
        }

        public async Task<Publisher> UpdatePublisherAsync(Guid id, Publisher publisher)
        {
            var json = JsonSerializer.Serialize(publisher, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Publishers/update/{id}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Publisher>(responseContent, _jsonOptions) ?? publisher;
        }

        public async Task<bool> DeletePublisherAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/Publishers/delete/{id}");
            return response.IsSuccessStatusCode;
        }

        // Members
        public async Task<List<Member>> GetMembersAsync()
        {
            var response = await _httpClient.GetAsync("api/Members/list");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Member>>(content, _jsonOptions) ?? new List<Member>();
        }

        public async Task<Member?> GetMemberAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/Members/get/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Member>(content, _jsonOptions);
            }
            return null;
        }

        public async Task<Member?> GetMemberByEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"api/Members/email/{Uri.EscapeDataString(email)}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Member>(content, _jsonOptions);
            }
            return null;
        }

        public async Task<List<Loan>> GetMemberLoansAsync(Guid memberId)
        {
            var response = await _httpClient.GetAsync($"api/Members/loans/{memberId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Loan>>(content, _jsonOptions) ?? new List<Loan>();
        }

        public async Task<List<Fine>> GetMemberFinesAsync(Guid memberId)
        {
            var response = await _httpClient.GetAsync($"api/Members/fines/{memberId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Fine>>(content, _jsonOptions) ?? new List<Fine>();
        }

        public async Task<bool> CanMemberCheckoutAsync(Guid memberId)
        {
            var response = await _httpClient.GetAsync($"api/Loans/membereligible/{memberId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(content, _jsonOptions);
        }

        public async Task<int> GetMemberActiveLoanCountAsync(Guid memberId)
        {
            var loans = await GetMemberLoansAsync(memberId);
            return loans.Count(l => l.Status == "Active");
        }

        public async Task<bool> HasOverdueBooksAsync(Guid memberId)
        {
            var response = await _httpClient.GetAsync($"api/Loans/member/{memberId}/hasoverduebooks");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(content, _jsonOptions);
        }

        public async Task<bool> HasUnreturnedBooksAsync(Guid memberId)
        {
            var response = await _httpClient.GetAsync($"api/Loans/member/{memberId}/hasunreturnedbooks");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(content, _jsonOptions);
        }

        public async Task<bool> HasBookCheckedOutAsync(Guid memberId, Guid bookId)
        {
            var response = await _httpClient.GetAsync($"api/Loans/member/{memberId}/hasbook/{bookId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(content, _jsonOptions);
        }

        public async Task<Member> CreateMemberAsync(Member member)
        {
            var json = JsonSerializer.Serialize(member, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Members/create", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Member>(responseContent, _jsonOptions) ?? member;
        }

        public async Task<Member> UpdateMemberAsync(Guid id, Member member)
        {
            var json = JsonSerializer.Serialize(member, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Members/update/{id}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Member>(responseContent, _jsonOptions) ?? member;
        }

        public async Task<bool> DeleteMemberAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/Members/delete/{id}");
            return response.IsSuccessStatusCode;
        }

        // Loans
        public async Task<List<Loan>> GetLoansAsync()
        {
            var response = await _httpClient.GetAsync("api/Loans/list");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Loan>>(content, _jsonOptions) ?? new List<Loan>();
        }

        public async Task<Loan?> GetLoanAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/Loans/get/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Loan>(content, _jsonOptions);
            }
            return null;
        }

        public async Task<List<Loan>> GetActiveLoansAsync()
        {
            var response = await _httpClient.GetAsync("api/Loans/active");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Loan>>(content, _jsonOptions) ?? new List<Loan>();
        }

        public async Task<List<Loan>> GetOverdueLoansAsync()
        {
            var response = await _httpClient.GetAsync("api/Loans/overdue");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Loan>>(content, _jsonOptions) ?? new List<Loan>();
        }

        public async Task<Loan> CheckoutBookAsync(Guid bookId, Guid memberId)
        {
            try
            {
                var checkoutRequest = new { BookId = bookId, MemberId = memberId };
                var json = JsonSerializer.Serialize(checkoutRequest, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Loans/checkout", content);

                // If the API returns an error status code, extract the error message and throw a more informative exception
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    // Try to extract error message from JSON response
                    try
                    {
                        var errorObj = JsonSerializer.Deserialize<ErrorResponse>(errorContent, _jsonOptions);
                        if (!string.IsNullOrEmpty(errorObj?.Message))
                        {
                            throw new InvalidOperationException(errorObj.Message);
                        }
                    }
                    catch (JsonException)
                    {
                        // If we can't parse the JSON, just use the raw content
                    }

                    // If we couldn't parse a specific message, throw a generic one with the status code
                    throw new HttpRequestException(
                        $"Failed to check out book. Status: {(int)response.StatusCode} {response.StatusCode}",
                        null,
                        response.StatusCode);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Loan>(responseContent, _jsonOptions) ?? new Loan();
            }
            catch (HttpRequestException ex)
            {
                // Let these propagate up with their status code
                throw;
            }
            catch (InvalidOperationException ex)
            {
                // Let these propagate up with the API error message
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking out book: {ex.Message}", ex);
            }
        }

        public async Task<bool> ReturnBookAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"api/Loans/return/{id}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RenewBookAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"api/Loans/renew/{id}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<Loan> UpdateLoanAsync(Guid id, Loan loan)
        {
            var json = JsonSerializer.Serialize(loan, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Loans/update/{id}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Loan>(responseContent, _jsonOptions) ?? loan;
        }

        public async Task<bool> DeleteLoanAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/Loans/delete/{id}");
            return response.IsSuccessStatusCode;
        }

        // Reviews
        public async Task<(List<Review> Reviews, double AverageRating, int RatingCount)> GetBookReviewsAsync(Guid bookId)
        {
            var response = await _httpClient.GetAsync($"api/Reviews/list/book/{bookId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ReviewsResponseModel>(content, _jsonOptions);
            return (result?.Reviews ?? new List<Review>(), result?.AverageRating ?? 0, result?.RatingCount ?? 0);
        }

        public async Task<Review?> GetReviewAsync(Guid reviewId)
        {
            var response = await _httpClient.GetAsync($"api/Reviews/details/{reviewId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Review>(content, _jsonOptions);
            }
            return null;
        }

        public async Task<Review?> GetMemberBookReviewAsync(Guid memberId, Guid bookId)
        {
            var response = await _httpClient.GetAsync($"api/Reviews/findBy/member/{memberId}/book/{bookId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Review>(content, _jsonOptions);
            }
            return null;
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            try
            {
                var json = JsonSerializer.Serialize(review, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Reviews/create", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create review. Status: {response.StatusCode}, Error: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Review>(responseContent, _jsonOptions) ?? review;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating review: {ex.Message}", ex);
            }
        }

        public async Task<Review> UpdateReviewAsync(Guid reviewId, Review review)
        {
            var json = JsonSerializer.Serialize(review, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Reviews/update/{reviewId}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Review>(responseContent, _jsonOptions) ?? review;
        }

        public async Task<bool> DeleteReviewAsync(Guid reviewId)
        {
            var response = await _httpClient.DeleteAsync($"api/Reviews/delete/{reviewId}");
            return response.IsSuccessStatusCode;
        }

        // Blog Posts
        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            var response = await _httpClient.GetAsync("api/BlogPosts/list");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<BlogPost>>(content, _jsonOptions) ?? new List<BlogPost>();
        }
        
        public async Task<List<BlogPost>> GetPublishedBlogPostsAsync()
        {
            var response = await _httpClient.GetAsync("api/BlogPosts/published");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<BlogPost>>(content, _jsonOptions) ?? new List<BlogPost>();
        }
        
        public async Task<BlogPost?> GetBlogPostByIdAsync(Guid blogPostId)
        {
            var response = await _httpClient.GetAsync($"api/BlogPosts/details/{blogPostId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<BlogPost>(content, _jsonOptions);
            }
            return null;
        }
        
        public async Task<List<BlogPost>> GetBlogPostsByAuthorAsync(Guid authorId)
        {
            var response = await _httpClient.GetAsync($"api/BlogPosts/author/{authorId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<BlogPost>>(content, _jsonOptions) ?? new List<BlogPost>();
        }
        
        public async Task<List<BlogPost>> GetBlogPostsByTagAsync(string tag)
        {
            var response = await _httpClient.GetAsync($"api/BlogPosts/tag/{Uri.EscapeDataString(tag)}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<BlogPost>>(content, _jsonOptions) ?? new List<BlogPost>();
        }
        
        public async Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost)
        {
            var json = JsonSerializer.Serialize(blogPost, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/BlogPosts/create", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BlogPost>(responseContent, _jsonOptions) ?? blogPost;
        }
        
        public async Task<BlogPost> UpdateBlogPostAsync(Guid blogPostId, BlogPost blogPost)
        {
            var json = JsonSerializer.Serialize(blogPost, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/BlogPosts/update/{blogPostId}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BlogPost>(responseContent, _jsonOptions) ?? blogPost;
        }
        
        public async Task<bool> DeleteBlogPostAsync(Guid blogPostId)
        {
            var response = await _httpClient.DeleteAsync($"api/BlogPosts/delete/{blogPostId}");
            return response.IsSuccessStatusCode;
        }

        internal class ReviewsResponseModel
        {
            public List<Review> Reviews { get; set; } = new();
            public double AverageRating { get; set; }
            public int RatingCount { get; set; }
        }

        internal class ErrorResponse
        {
            public string? Message { get; set; }
        }
    }
}