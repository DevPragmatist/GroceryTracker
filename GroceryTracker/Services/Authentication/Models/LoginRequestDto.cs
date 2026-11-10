namespace GroceryTracker.Services.Authentication.Models;

public sealed class LoginRequestDto
{
    public required string UserNameOrEmail { get; set; }
    public required string Password { get; set; }
}
