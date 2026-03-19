using Articles.Abstractions.Enums;

namespace Articles.IntegrationEvents.Contracts;

public record ActorDto(
    UserRoleType Role,
    HashSet<ContributionArea> ContributionAreas,
    PersonDto Person
    );
