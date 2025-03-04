using System.Threading.Tasks;
using InteriorBackend.Models;

namespace InteriorBackend.Services
{
    public interface IUserService
    {
        Task<User> FindUserByIdAsync(string userId);
        Task<bool> ValidateEmailConfirmationCodeAsync(string userId, string code);
        Task<bool> ConfirmUserEmailAsync(string userId);
        Task<bool> ChangeUserEmailAsync(string userId, string newEmail);
        Task<string> GenerateEmailChangeCodeAsync(string userId);
        Task<bool> VerifyUserCredentialsAsync(string email, string password);
        Task<bool> RegisterUserAsync(User user, string password);
        Task<User> FindUserByEmailAsync(string email);
        Task<string> GeneratePasswordResetTokenAsync(string userId);
        Task<string> GenerateEmailConfirmationCodeAsync(string userId);
    }
}
