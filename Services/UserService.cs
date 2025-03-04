using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InteriorBackend.Models;
using InteriorBackend.Data;
using InteriorBackend.Services;

namespace InteriorBackend.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> FindUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<bool> ValidateEmailConfirmationCodeAsync(string userId, string code)
        {
            var user = await FindUserByIdAsync(userId);
            if (user == null || user.EmailConfirmationCode != code)
            {
                return false;
            }

            return true;
        }
        public async Task<string> GenerateEmailConfirmationCodeAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return null;
            }

            var code = GenerateEmailConfirmationToken();
            user.EmailConfirmationCode = code;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return code;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private string GenerateEmailConfirmationToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public async Task<bool> ConfirmUserEmailAsync(string userId)
        {
            var user = await FindUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null; // Clear the code

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangeUserEmailAsync(string userId, string newEmail)
        {
            var user = await FindUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Email = newEmail;
            user.EmailConfirmationCode = null; // Clear confirmation code

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<string> GenerateEmailChangeCodeAsync(string userId)
        {
            var user = await FindUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var code = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.EmailConfirmationCode = code;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return code;
        }


        public async Task<bool> VerifyUserCredentialsAsync(string email, string password)
        {
            var user = await FindUserByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            var hashedPassword = HashPassword(password);
            return user.PasswordHash == hashedPassword;
        }

        public async Task<bool> RegisterUserAsync(User user, string password)
        {
            user.PasswordHash = HashPassword(password);

            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<User> FindUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return null;
            }

            // Generate a secure reset token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.EmailConfirmationCode = token; // Store token for validation

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return token;
        }
    }
}
