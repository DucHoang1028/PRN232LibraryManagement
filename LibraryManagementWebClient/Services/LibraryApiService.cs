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
            var response = await _httpClient.GetAsync($"api/Members/cancheckout/{memberId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(content, _jsonOptions);
        }

        public async Task<int> GetMemberActiveLoanCountAsync(Guid memberId)
        {
            var response = await _httpClient.GetAsync($"api/Members/activeloancount/{memberId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<int>(content, _jsonOptions);
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
            var checkoutRequest = new { BookId = bookId, MemberId = memberId };
            var json = JsonSerializer.Serialize(checkoutRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Loans/checkout", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Loan>(responseContent, _jsonOptions) ?? new Loan();
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
    }
} 