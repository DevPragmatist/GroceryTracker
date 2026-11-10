namespace GroceryTracker.Services.Authentication.Models;

public sealed class RegisterRequestDto
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}
