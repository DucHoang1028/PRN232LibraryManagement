using Contracts;

namespace LibraryManagementWebClient.Services
{
    public interface IApiService
    {
        Task<LoginResponse> AuthenticateAsync(string email, string password);
    }
}
