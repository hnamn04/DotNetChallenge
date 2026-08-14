using DotNetChallenge.Configuration;
using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Auth;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotNetChallenge.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthService(AppDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        // Register
        public async Task<ProfileResponse> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var emailExists = await _context.Users
                .AnyAsync(x => x.Email.ToLower() == email);

            if (emailExists)
            {
                throw new DuplicateEmailException($"Email '{request.Email}' is already registered.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return MapToProfileResponse(user);
        }

        // Login
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            // Retrieve the user from the database, including their roles
            var user = await _context.Users
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email);

            // Validate the user's credentials
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new InvalidCredentialsException("Invalid email or password.");
            }

            // Retrieve the user's roles
            var roles = user.UserRoles
                .Select(x => x.Role.Name)
                .ToList();

            // Check if the user is active
            if (!user.IsActive)
            {
                throw new InvalidCredentialsException("User account is inactive.");
            }

            // Generate a JWT token for the authenticated user
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            // Create claims for the JWT token
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.Username)
            };

            // Add role claims to the JWT token
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Create a symmetric security key using the JWT secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            // Create signing credentials using the security key and HMAC SHA256 algorithm
            var credentials = new SigningCredentials
            (
                key,
                SecurityAlgorithms.HmacSha256
            );

            // Create the JWT token with the specified issuer, audience, claims, expiration, and signing credentials
            var token = new JwtSecurityToken
            (
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return new LoginResponse
            {
                Token = new JwtSecurityTokenHandler()
                    .WriteToken(token),

                ExpiresAt = expiresAt
            };
        }

        public async Task<ProfileResponse> GetProfileAsync(Guid userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
            {
                throw new NotFoundException($"User with id '{userId}' was not found.");
            }

            return MapToProfileResponse(user);
        }

        private static ProfileResponse MapToProfileResponse(User user)
        {
            return new ProfileResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
            };
        }
    }
}
