using Blocks.Core;
using Blocks.Redis;
using Journals.Domain.Journals;
using Journals.Grpc;
using ProtoBuf.Grpc;

namespace Journals.API.Features.Journals;

public class JournalGrpcService(Repository<Journal> _journalRepository) : IJournalService
{
    public ValueTask<GetJournalResponse> GetJournalByIdAsync(GetJournalByIdRequest request, CallContext context = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<IsEditorAssignedToJournalResponse> IsEditorAssignedToJournalAsync(IsEditorAssignedToJournalRequest request, CallContext context = default)
    {
        var journal = await _journalRepository.GetByIdOrThrowAsync(request.JournalId);

        return new IsEditorAssignedToJournalResponse
        {
            IsAssigned = journal.ChiefEditorId == request.UserId
        };
    }
}
