using Auth.Domain.Persons;
using Auth.Grpc;
using Auth.Persistence.Repositories;
using Mapster;
using ProtoBuf.Grpc;

namespace Auth.API.Features.Persons;

public class PersonGrpcService(PersonRepository _personRepository, GrpcTypeAdapterConfig _typeAdapterConfig) : IPersonService
{
    public async ValueTask<GetPersonResponse> GetPersonByIdAsync(GetPersonRequest request, CallContext context = default)
        => await GetPersonResponseAsync(() => _personRepository.GetByIdAsync(request.PersonId, context.CancellationToken));

    public async ValueTask<GetPersonResponse> GetPersonByUserIdAsync(GetPersonByUserIdRequest request, CallContext context = default)
        => await GetPersonResponseAsync(() => _personRepository.GetByUserIdAsync(request.UserId, context.CancellationToken));

    public async ValueTask<GetPersonResponse> GetPersonByEmailAsync(GetPersonByEmailRequest request, CallContext context = default)
        => await GetPersonResponseAsync(() => _personRepository.GetByEmailAsync(request.Email, context.CancellationToken));

    public async ValueTask<CreatePersonResponse> GetOrCreatePersonAsync(CreatePersonRequest request, CallContext context = default)
    {
        var person = await _personRepository.GetByEmailAsync(request.Email, context.CancellationToken);
        if (person is null)
        {
            person = Person.Create(request);
            await _personRepository.AddAsync(person, context.CancellationToken);
            await _personRepository.SaveChangesAsync(context.CancellationToken);
        }

        return new CreatePersonResponse
        {
            PersonInfo = person.Adapt<PersonInfo>(_typeAdapterConfig)
        };
    }

    private async ValueTask<GetPersonResponse> GetPersonResponseAsync(Func<Task<Person?>> fetch)
    {
        var person = Guard.NotFound(await fetch());
        return new GetPersonResponse
        {
            PersonInfo = person.Adapt<PersonInfo>(_typeAdapterConfig)
        };
    }
}
