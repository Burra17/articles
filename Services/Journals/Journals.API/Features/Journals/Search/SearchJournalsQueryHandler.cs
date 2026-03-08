using Blocks.Core.Extensions;
using Blocks.Redis;
using FastEndpoints;
using Journals.API.Features.Journals.Shared;
using Journals.Domain.Journals;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Redis.OM;
using Role = Articles.Security.Role;

namespace Journals.API.Features.Journals.Search;

[Authorize(Roles = $"{Role.EOF},{Role.REVED}")]
[HttpGet("journals")]
[Tags("Journals")]
public class SearchJournalsQueryHandler(Repository<Journal> _journalRepository, Repository<Editor> _editorRepository)
    : Endpoint<SearchJournalsQuery, SearchJournalsResponse>
{
    public override async Task HandleAsync(SearchJournalsQuery query, CancellationToken ct)
    {
        var collection = _journalRepository.Collection;
        if(!query.Search.IsNullOrEmpty())
        {
            var search = query.Search.Trim().ToLowerInvariant();

            var queryString =
                $"(@Abbrevation:{{{search}}}) | " +
                $"(@Name:*{search}* | @Description:*{search}*)";

            collection =_journalRepository.Collection.Raw(queryString);
        }

        var totalCount = collection.Count();

        var items = collection
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var response = new SearchJournalsResponse(
            query.Page,
            query.PageSize,
            totalCount,
            items.Select(j =>
            {
                var dto = j.Adapt<JournalDto>();
                dto.Editor = _editorRepository.GetById(j.ChiefEditorId).Adapt<EditorDto>();
                return dto;
            })
        );

        await Send.OkAsync(response, cancellation: ct);
    }
}
