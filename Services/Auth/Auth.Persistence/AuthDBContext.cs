using Auth.Domain.Persons;
using Auth.Domain.Roles;
using Auth.Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Persistence;

public class AuthDBContext(DbContextOptions<AuthDBContext> options)
    : IdentityDbContext<User, Role, int>(options)
{
    public virtual DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
}
