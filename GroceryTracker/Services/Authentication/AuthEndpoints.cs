using FastEndpoints;
using GroceryTracker.Services.Authentication.Models;

namespace GroceryTracker.Services.Authentication;

public class RegisterEndpoint : Endpoint<RegisterRequestDto>
{
    private readonly IAuthService _authService;

    public RegisterEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequestDto req, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(req, ct);
        await HttpContext.Response.SendAsync(result, cancellation: ct);
    }
}

public class LoginEndpoint : Endpoint<LoginRequestDto>
{
    private readonly IAuthService _authService;

    public LoginEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequestDto req, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(req, ct);
        await HttpContext.Response.SendAsync(result, cancellation: ct);
    }
}

public class ForgotPasswordEndpoint : Endpoint<ForgotPasswordRequestDto>
{
    private readonly IAuthService _authService;

    public ForgotPasswordEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/auth/forgot-password");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ForgotPasswordRequestDto req, CancellationToken ct)
    {
        await _authService.ForgotPasswordAsync(req, ct);
        await HttpContext.Response.SendAsync(new { message = "If an account exists for that email, a password reset email has been sent." }, cancellation: ct);
    }
}

public class ResetPasswordEndpoint : Endpoint<ResetPasswordRequestDto>
{
    private readonly IAuthService _authService;

    public ResetPasswordEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/auth/reset-password");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ResetPasswordRequestDto req, CancellationToken ct)
    {
        await _authService.ResetPasswordAsync(req, ct);
        await HttpContext.Response.SendAsync(new { message = "Password reset successful." }, cancellation: ct);
    }
}
