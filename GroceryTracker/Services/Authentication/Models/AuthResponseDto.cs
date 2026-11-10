namespace GroceryTracker.Services.Authentication.Models;

public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt,
    string UserName,
    string Email);
