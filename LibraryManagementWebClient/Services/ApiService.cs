using Contracts;

namespace LibraryManagementWebClient.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
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
    }
}
