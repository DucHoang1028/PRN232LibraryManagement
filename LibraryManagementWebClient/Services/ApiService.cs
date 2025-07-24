using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Contracts;
using LibraryManagementWebClient.Models;
using LibraryManagementWebClient.ViewModels;

namespace LibraryManagementWebClient.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;


        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private HttpRequestMessage CreateRequestWithJwt(HttpMethod method, string url)
        {
            var context = _httpContextAccessor?.HttpContext;
            var request = new HttpRequestMessage(method, url);

            if (context != null)
            {
                var token = context.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return request;
        }



        public async Task<LoginResponse?> AuthenticateAsync(string email,  string password)
        {
            var loginRequest = new
            {
                Email = email,
                Password = password
            };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginRequest);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>(); 
                return result;
            }

            return null;
        }

        public async Task<List<Member>> GetAllUsersAsync()
        {
            var request = CreateRequestWithJwt(HttpMethod.Get, "api/Members/list");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<List<Member>>();

            return users ?? new();
        }

        public async Task<Member?> FindUserByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/Members/get/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Member>(_jsonOptions);
        }


        public async Task<bool> UpdateUserAsync(Guid id, Member member)
        {
            var json = JsonSerializer.Serialize(member, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Members/update/{id}", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<HttpResponseMessage> RegisterAsync(RegisterViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return await _httpClient.PostAsync("api/auth/register", content); // Adjust URL to your actual API route
        }

    }
}
