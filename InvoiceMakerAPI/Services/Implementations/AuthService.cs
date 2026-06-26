using Data.Models;
using InvoiceMakerAPI.DTOs;
using InvoiceMakerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace InvoiceMakerAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResult> RegisterAsync(RegisterDTO registerDTO)
        {
            var user = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                FullName = registerDTO.FullName
            };

            var checkUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (checkUser != null)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = new[] { "User with this email already exists" }
                };
            }

            var register = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!register.Succeeded)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = register.Errors.Select(e => e.Description)
                };
            }

            var token = _tokenService.GenerateToken(user);

            var result = new AuthResult
            {
                Succeeded = register.Succeeded,
                Token = token
            };

            return result;
        }

        public async Task<AuthResult> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user is null)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = ["Invalid credentials"]
                };
            }

            var validPassword = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!validPassword)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = ["Invalid credentials"]
                };
            }

            var token = _tokenService.GenerateToken(user);

            return new AuthResult
            {
                Succeeded = true,
                Token = token
            };
        }

        public async Task<AuthResult> ChangePasswordAsync(string userId, ChangePasswordDTO request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = ["User not found"]
                };
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword);

            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(x => x.Description)
                };
            }

            return new AuthResult
            {
                Succeeded = true
            };
        }
    }
}
