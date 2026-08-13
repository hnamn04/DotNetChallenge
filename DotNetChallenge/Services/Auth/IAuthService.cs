using DotNetChallenge.DTOs.Auth;

namespace DotNetChallenge.Services.Auth
{
    public interface IAuthService
    {
        Task<ProfileResponse> RegisterAsync(RegisterRequest request);

        Task<LoginResponse> LoginAsync(LoginRequest request);

        Task<ProfileResponse> GetProfileAsync(Guid userId);
    }
}
