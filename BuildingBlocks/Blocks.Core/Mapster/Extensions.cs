using Mapster;

namespace Blocks.Mapster;

public static class Extensions
{
    public static TDestination AdaptWith<TDestination>(this object source, Action<TDestination> afterMapping)
    {
        var destination = source.Adapt<TDestination>();

        // Apply additional property settings
        afterMapping?.Invoke(destination);

        return destination;
    }
}
