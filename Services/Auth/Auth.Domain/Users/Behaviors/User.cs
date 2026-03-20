using Auth.Domain.Persons.ValueObjects;
using Blocks.Core.Extensions;

namespace Auth.Domain.Users;

public partial class User
{
    public static User Create(IUserCreationInfo userInfo)
    {
        if (userInfo.UserRoles.IsNullOrEmpty())
            throw new ArgumentException("User must have at least one role.", nameof(userInfo.UserRoles));

        var user = new User
        {
            UserName = userInfo.Email,
            Email = userInfo.Email,
            PhoneNumber = userInfo.PhoneNumber,
        };

        // todo add domain event
        return user;
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }
}
