namespace GroceryTracker.Services.Authentication.Models;

public sealed class ForgotPasswordRequestDto
{
    public required string Email { get; set; }
}
