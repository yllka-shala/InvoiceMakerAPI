using Data.Models;

namespace InvoiceMakerAPI.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}
