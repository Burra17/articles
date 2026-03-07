using Articles.Abstractions;
using Articles.Abstractions.Enums;
using Auth.Grpc;
using Blocks.Exceptions;
using Blocks.Redis;
using FastEndpoints;
using Grpc.Core;
using Journals.Domain.Journals;
using Journals.Domain.Journals.Events;
using Mapster;
using Microsoft.AspNetCore.Authorization;

namespace Journals.API.Features.Journals.Create;

[Authorize(Roles = Role.EOF)]
[HttpPost("journals")]
[Tags ("Journals")]
public class CreateJournalEndpoint(Repository<Journal> _journalRepository, Repository<Editor> _editorRepository, IPersonService _personService) 
    : Endpoint<CreateJournalCommand, IdResponse>
{
    public async override Task HandleAsync(CreateJournalCommand command, CancellationToken ct)
    {
        if(_journalRepository.Collection.Any(j => j.Abbrevation == command.Abbrevation || j.Name == command.Name))
                throw new BadRequestException("Journal with the same name or abbrevation already exists.");

        if(!_editorRepository.Collection.Any(e => e.Id == command.ChiefEditorId))
            await CreateEditor(command.ChiefEditorId, ct);

        var journal = command.Adapt<Journal>();

        await _journalRepository.AddAsync(journal);

        // todo publish the JournalCreated event

        await PublishAsync(new JournalCreated(journal));
        await Send.OkAsync(new IdResponse(journal.Id));
    }

    private async Task CreateEditor(int userId, CancellationToken ct)
    {
        // todo get the editor from Auth service
        var response = await _personService.GetPersonByUserIdAsync(
            new GetPersonByUserIdRequest { UserId = userId }, new CallOptions(cancellationToken: ct));
        var editor = new Editor
        {
            Id = userId,
            PersonId = response.PersonInfo.Id,
            Affiliation = response.PersonInfo.ProfessionalProfile!.Affiliation,
            FullName = response.PersonInfo.FirstName + " " + response.PersonInfo.LastName,
        };

        await _editorRepository.AddAsync(editor);
    }
}
