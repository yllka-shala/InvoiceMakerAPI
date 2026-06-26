using InvoiceMakerAPI.DTOs;

namespace InvoiceMakerAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthResult> LoginAsync(LoginDTO loginDTO);
        Task<AuthResult> ChangePasswordAsync(string userId, ChangePasswordDTO request);
    }
}
