using Contracts;
using LibraryManagementWebClient.Models;
using LibraryManagementWebClient.ViewModels;

namespace LibraryManagementWebClient.Services
{
    public interface IApiService
    {
        Task<LoginResponse> AuthenticateAsync(string email, string password);
        Task<List<Member>> GetAllUsersAsync();

        Task<Member?> FindUserByIdAsync(Guid id);
        Task<bool> UpdateUserAsync(Guid id, Member member);
        Task<HttpResponseMessage> RegisterAsync(RegisterViewModel model);
    }
}
