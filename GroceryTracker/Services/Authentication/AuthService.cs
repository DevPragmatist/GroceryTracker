using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GroceryTracker.Domain;
using GroceryTracker.Domain.Entities;
using GroceryTracker.Infrastructure.Authentication;
using GroceryTracker.Services.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GroceryTracker.Services.Authentication;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
    Task ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken);
    Task ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken);
}

public sealed class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly GroceryContext _context;
    private readonly JwtOptions _jwtOptions;
    private readonly IEmailSender _emailSender;

    public AuthService(
        UserManager<IdentityUser> userManager,
        GroceryContext context,
        IOptions<JwtOptions> jwtOptions,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _context = context;
        _jwtOptions = jwtOptions.Value;
        _emailSender = emailSender;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("Username, email, and password are required.");
        }

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Password confirmation does not match.");
        }

        var normalizedUserName = request.UserName.Trim();
        var normalizedEmail = request.Email.Trim();

        if (await _userManager.FindByNameAsync(normalizedUserName) is not null)
        {
            throw new InvalidOperationException("A user with this username already exists.");
        }

        if (await _userManager.FindByEmailAsync(normalizedEmail) is not null)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        var user = new IdentityUser
        {
            UserName = normalizedUserName,
            Email = normalizedEmail,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }

        return await BuildTokenResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserNameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new UnauthorizedAccessException("Username/email and password are required.");
        }

        var user = await _userManager.FindByNameAsync(request.UserNameOrEmail.Trim())
            ?? await _userManager.FindByEmailAsync(request.UserNameOrEmail.Trim());

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedAccessException("Invalid username/email or password.");
        }

        return await BuildTokenResponseAsync(user, cancellationToken);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new InvalidOperationException("An email address is required.");
        }

        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            return;
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
        await _emailSender.SendPasswordResetEmailAsync(user.Email ?? request.Email, encodedToken, cancellationToken);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new InvalidOperationException("Email, token, and a new password are required.");
        }

        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            throw new InvalidOperationException("The reset request is invalid.");
        }

        var resetToken = DecodeResetToken(request.Token);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }
    }

    private async Task<AuthResponseDto> BuildTokenResponseAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        var accessToken = CreateAccessToken(user);
        var refreshToken = CreateRefreshToken();
        var refreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenDays);

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            User = user,
            TokenHash = HashToken(refreshToken),
            ExpiresAt = refreshTokenExpiresAt,
            RevokedAt = null,
            ReplacedByTokenHash = null
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes),
            RefreshTokenExpiresAt: refreshTokenExpiresAt,
            UserName: user.UserName ?? string.Empty,
            Email: user.Email ?? string.Empty);
    }

    private string CreateAccessToken(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string CreateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string DecodeResetToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        try
        {
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch
        {
            return token;
        }
    }

    private static string HashToken(string token)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hashBytes);
    }
}
