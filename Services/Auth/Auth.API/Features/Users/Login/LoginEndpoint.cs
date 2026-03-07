using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Blocks.AspNetCore;
using Auth.Application;
using Microsoft.AspNetCore.Authorization;
using Auth.Persistence.Repositories;

namespace Auth.API.Features.Users.Login;

[AllowAnonymous]
[HttpPost("login")]
public class LoginEndpoint(PersonRepository _personRepository, UserManager<User> _userManager, SignInManager<User> _signInManager, TokenFactory _tokenFactory) : Endpoint<LoginCommand, LoginResponse>
{
    public override async Task HandleAsync(LoginCommand command, CancellationToken ct)
    {
        var person = Guard.NotFound(
            await _personRepository.GetByEmailAsync(command.Email)
            );

        var user = Guard.NotFound(person.User);

        var result = await _signInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            throw new BadHttpRequestException($"Invalid credentials for {command.Email}");

        var userRoles = await _userManager.GetRolesAsync(user);

        // generate token
        var jwtToken = _tokenFactory.GenerateJWTToken(user.Id.ToString(), user.Person.FullName, command.Email, userRoles, Array.Empty<Claim>());
        var refreshToken = _tokenFactory.GenerateRefreshToken(HttpContext.GetClientIpAddress());
        user.AddRefreshToken(refreshToken);

        await Send.OkAsync(new LoginResponse(command.Email, jwtToken, refreshToken.Token));
    }
}
