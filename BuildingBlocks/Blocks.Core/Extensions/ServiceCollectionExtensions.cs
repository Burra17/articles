using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Blocks.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConcreteImplementationsOfGeneric(
        this IServiceCollection services,
        Type genericBaseType,
        Assembly[]? assemblies = null,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        assemblies ??= new[] { Assembly.GetCallingAssembly() };

        var implementations = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                InheritsFromGeneric(t, genericBaseType));

        foreach ( var impl in implementations )
        {
            services.Add(new ServiceDescriptor(impl, impl, lifetime));
        }

        return services;
    }

    private static bool InheritsFromGeneric(Type type, Type genericBase)
    {
        while (type != null && type != typeof(object))
        {
            var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            if(cur == genericBase )
                return true;
            type = type.BaseType;
        }
        return false;
    }
}
