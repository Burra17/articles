using Blocks.EntityFrameworkCore;

namespace Submission.Application.Features.CreateAndAssignAuthor;

public class CreateAndAssignAuthorCommandHandler(ArticleRepository _articleRepository)
    : IRequestHandler<CreateAndAssignAuthorCommand, IdResponse>
{
    public async Task<IdResponse> Handle(CreateAndAssignAuthorCommand command, CancellationToken ct)
    {
        var article = await _articleRepository.GetByIdOrThrowAsync(command.ArticleId);

        Author? author = null;
        if (command.UserId is null) // Author is not an User
                                     // Author = command.adapt<Author>();
            author = Author.Create(command.Email!, command.FirstName!, command.LastName!, command.Title!, command.Affiliation!);
        else { }                    // todo - Author is an User

        article.AssignAuthor(author, command.ContributionAreas, command.IsCorrespondingAuthor);

        await _articleRepository.SaveChangesAsync();

        return new IdResponse(article.Id);
    }
}
