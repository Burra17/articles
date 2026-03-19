using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blocks.EntityFrameworkCore;

public static class BuilderExtensions
{
    public static PropertyBuilder<TEnum> HasEnumConversion<TEnum>(this PropertyBuilder<TEnum> builder)
        where TEnum : Enum
    {
        return builder.HasConversion(
            e => e.ToString(),
            value => (TEnum)Enum.Parse(typeof(TEnum), value));
    }

    public static PropertyBuilder<T> HasJsonCollectionConversion<T>(this PropertyBuilder<T> builder)
    {
        return builder.HasConversion(BuildJsonListConvertor<T>());
    }

    public static ValueConverter<TCollection, string> BuildJsonListConvertor<TCollection>()
    {
        Func<TCollection, string> serializeFunc = v => JsonSerializer.Serialize(v);
        Func<string, TCollection> deserializeFunc = v => JsonSerializer.Deserialize<TCollection>(v ?? "[]");

        return new ValueConverter<TCollection, string>(
            v => serializeFunc(v),
            v => deserializeFunc(v));
    }

    public static ValueConverter<IReadOnlyList<T>, string> BuildJsonReadOnlyListConvertor<T>()
    {
        Func<IReadOnlyList<T>, string> serializeFunc = v => JsonSerializer.Serialize(v);
        Func<string, IReadOnlyList<T>> deserializeFunc = v => JsonSerializer.Deserialize<IReadOnlyList<T>>(v ?? "[]")!;

        return new ValueConverter<IReadOnlyList<T>, string>(
            v => serializeFunc(v),
            v => deserializeFunc(v));
    }

    public static PropertyBuilder<TProperty> HasColumnNameSameAsProperty<TProperty>(this PropertyBuilder<TProperty> builder)
        => builder.HasColumnName(builder.Metadata.PropertyInfo?.Name);

    public static bool SeedFromJsonFile<T>(this EntityTypeBuilder<T> builder, string folder = "Data/Master")
        where T : class
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, folder, $"{typeof(T).Name}.json");
        if (!File.Exists(filePath))
            return false;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        var data = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(filePath), options);
        if (data != null)
            builder.HasData(data);

        return true;
    }
}
