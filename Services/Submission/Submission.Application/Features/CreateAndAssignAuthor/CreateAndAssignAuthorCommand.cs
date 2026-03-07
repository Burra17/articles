using Blocks.Core.Constraints;
using Blocks.Core.FluenValidation;

namespace Submission.Application.Features.CreateAndAssignAuthor;

public record CreateAndAssignAuthorCommand(int? UserId, string? FirstName, string? LastName, string? Email, string? Title, string? Affiliation, int? AuthorId, HashSet<ContributionArea> ContributionAreas, bool IsCorrespondingAuthor)
    : ArticleCommand
{
    public override ArticleActionType ActionType => ArticleActionType.AssignAuthor;
}

public class CreateAndAssignAuthorCommandValidator : ArticleCommandValidator<CreateAndAssignAuthorCommand>
{
    public CreateAndAssignAuthorCommandValidator()
    {
        When(c => c.UserId == null, () =>
        {
            RuleFor(x => x.Email)
                .NotEmptyWithMessage(nameof(CreateAndAssignAuthorCommand.Email))
                .MaximumLengthWithMessage(MaxLength.C64, nameof(CreateAndAssignAuthorCommand.Email));

            RuleFor(x => x.FirstName)
                .NotEmptyWithMessage(nameof(CreateAndAssignAuthorCommand.FirstName))
                .MaximumLengthWithMessage(MaxLength.C64, nameof(CreateAndAssignAuthorCommand.FirstName));

            RuleFor(x => x.LastName)
                .NotEmptyWithMessage(nameof(CreateAndAssignAuthorCommand.LastName))
                .MaximumLengthWithMessage(MaxLength.C256, nameof(CreateAndAssignAuthorCommand.LastName));

            RuleFor(x => x.Title)
                .MaximumLengthWithMessage(MaxLength.C32, nameof(CreateAndAssignAuthorCommand.Title));

            RuleFor(x => x.Affiliation)
                .NotEmptyWithMessage(nameof(CreateAndAssignAuthorCommand.Affiliation))
                .MaximumLengthWithMessage(MaxLength.C512, nameof(CreateAndAssignAuthorCommand.Affiliation));
        });

            RuleFor(x => x.ContributionAreas)
                .NotEmptyWithMessage(nameof(CreateAndAssignAuthorCommand.ContributionAreas));
    }
}


