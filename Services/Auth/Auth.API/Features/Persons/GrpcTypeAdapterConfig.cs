using Auth.Domain.Persons;
using Auth.Grpc;
using Mapster;

namespace Auth.API.Features.Persons;

public class GrpcTypeAdapterConfig : TypeAdapterConfig
{
    public GrpcTypeAdapterConfig()
    {
        this.NewConfig<Person, PersonInfo>()
            .Map(dest => dest.Honorific, src => src.Honorific == null ? null : src.Honorific.Value)
            .IgnoreNullValues(true);

        this.NewConfig<Auth.Domain.Persons.ValueObjects.ProfessionalProfile, ProfessionalProfile>();
    }
}
