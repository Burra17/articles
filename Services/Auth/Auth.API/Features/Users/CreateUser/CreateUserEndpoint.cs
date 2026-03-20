using Articles.Security;
using Auth.Domain.Persons;
using Auth.Domain.Users.Events;
using Auth.Persistence;
using Auth.Persistence.Repositories;
using Blocks.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Features.Users.CreateUser;

[Authorize(Roles = Role.USERADMIN)]
[HttpPost("users")]
public class CreateUserEndpoint(PersonRepository _personRepository, AuthDBContext _dbContext, UserManager<User> userManager) 
    : Endpoint<CreateUserCommand, CreateUserResponse>
{
    public override async Task HandleAsync(CreateUserCommand command, CancellationToken ct)
    {
        var person = await _personRepository.GetByEmailAsync(command.Email);
        if (person?.User != null) 
            throw new BadRequestException($"User with email {command.Email} already exists.");

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

        if (person is null) // create person if it doesnt already exists
            person = await CreatePersonAsync(command, ct);

        var user = Domain.Users.User.Create(command);
        user.PersonId = person.Id;

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new BadRequestException($"Unable to create user: {errorMessages}");
        }

        foreach (var roleDto in command.UserRoles)
        {
            var addRoleResult = await userManager.AddToRoleAsync(user, roleDto.Type.ToString());
            if (!addRoleResult.Succeeded)
            {
                var errorMessages = string.Join(" | ", addRoleResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                throw new BadRequestException($"Unable to assign role {roleDto.Type}: {errorMessages}");
            }
        }

        person.AssignUser(user);

        var resetPasswordToken = await userManager.GeneratePasswordResetTokenAsync(user);

        await _personRepository.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);

        await PublishAsync(new UserCreated(user, resetPasswordToken));
        await Send.OkAsync(new CreateUserResponse(command.Email, user.Id, resetPasswordToken));
    }

    private async Task<Person> CreatePersonAsync(CreateUserCommand command, CancellationToken ct)
    {
        var person = Person.Create(command);
        await _personRepository.AddAsync(person);
        await _personRepository.SaveChangesAsync(ct);

        return person;
    }
}
