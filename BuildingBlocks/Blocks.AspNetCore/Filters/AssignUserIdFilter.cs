using Blocks.Core.Security;
using Blocks.Domain;
using Microsoft.AspNetCore.Http;

namespace Blocks.AspNetCore.Filters;

public class AssignUserIdFilter(IClaimsProvider _claimsProvider) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (var argument in context.Arguments)
        {
            if (argument is IAuditableAction action)
            {
                var userId = _claimsProvider.TryGetUserId();
                if (userId != null) 
                    action.CreatedById = userId.Value;
            }
        }

        return await next(context);
    }
}
