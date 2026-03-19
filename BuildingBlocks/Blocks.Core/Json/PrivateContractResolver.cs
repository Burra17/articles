using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Blocks.Core.Json;

/// Includes private and init-only members in JSON (de)serialization.
/// Also supports types with only private/protected constructors (e.g. value objects).
public class PrivateContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Select(p => base.CreateProperty(p, memberSerialization))
                    .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                 .Select(f => base.CreateProperty(f, memberSerialization)))
                    .ToList();
        props.ForEach(p => { p.Writable = true; p.Readable = true; });
        return props;
    }

    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        var contract = base.CreateObjectContract(objectType);

        // If no public constructor found, try private/protected ones
        if (contract.DefaultCreator == null && contract.OverrideCreator == null)
        {
            var ctor = objectType
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .OrderBy(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (ctor != null)
            {
                contract.DefaultCreator = () =>
                {
                    var args = ctor.GetParameters()
                        .Select(p => p.ParameterType.IsValueType
                            ? Activator.CreateInstance(p.ParameterType)
                            : (object?)null)
                        .ToArray();
                    return ctor.Invoke(args);
                };
            }
        }

        return contract;
    }
}
