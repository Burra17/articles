using Auth.Domain.Persons;
using Auth.Domain.Persons.ValueObjects;
using Auth.Domain.Roles;
using Auth.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Articles.Abstractions.Enums;

namespace Auth.Persistence.Data.Test;

public static class Seed
{
    public static void SeedTestData(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDBContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        if (context.Users.Any()) return;

        // Seed roles
        var roles = new[]
        {
            new Role { Name = "EOF",       NormalizedName = "EOF",       Type = UserRoleType.EOF,       Description = "Editorial Office" },
            new Role { Name = "AUT",       NormalizedName = "AUT",       Type = UserRoleType.AUT,       Description = "Author" },
            new Role { Name = "CORAUT",    NormalizedName = "CORAUT",    Type = UserRoleType.CORAUT,    Description = "Corresponding Author" },
            new Role { Name = "REVED",     NormalizedName = "REVED",     Type = UserRoleType.REVED,     Description = "Review Editor" },
            new Role { Name = "REV",       NormalizedName = "REV",       Type = UserRoleType.REV,       Description = "Reviewer" },
            new Role { Name = "USERADMIN", NormalizedName = "USERADMIN", Type = UserRoleType.USERADMIN, Description = "User Admin" },
        };
        foreach (var role in roles)
        {
            if (!roleManager.RoleExistsAsync(role.Name!).GetAwaiter().GetResult())
                roleManager.CreateAsync(role).GetAwaiter().GetResult();
        }

        // Seed admin person + user
        var person = new Person
        {
            FirstName = "DotNet",
            LastName = "LabX",
            Gender = Gender.Male,
            Email = EmailAddress.Create("dotnetlabx@articles.test"),
        };
        context.Persons.Add(person);
        context.SaveChanges();

        var user = new User
        {
            UserName = "dotnetlabx@articles.test",
            Email = "dotnetlabx@articles.test",
            PersonId = person.Id,
        };
        userManager.CreateAsync(user, "Pass.123!").GetAwaiter().GetResult();
        userManager.AddToRoleAsync(user, "USERADMIN").GetAwaiter().GetResult();

        person.UserId = user.Id;
        context.SaveChanges();
    }
}
