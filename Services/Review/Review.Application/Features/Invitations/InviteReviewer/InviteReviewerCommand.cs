using Blocks.Core.Constraints;
using Blocks.Core.FluenValidation;
using FluentValidation;
using Review.Application.Features.Shared;
using Review.Domain.Shared.Enums;

namespace Review.Application.Features.Invitations.InviteReviewer;

public record InviteReviewerCommand(int? UserId, string Email, string FirstName, string LastnName)
    : ArticleCommand<ArticleActionType, InviteReviewerResponse>
{
    public override ArticleActionType ActionType => ArticleActionType.InviteReviewer;
}

public record InviteReviewerResponse(int ArticleId, int InvitationId, string Token);

public class InviteReviewerCommandValidator : AbstractValidator<InviteReviewerCommand>
{
    public InviteReviewerCommandValidator()
    {
        When(c => c.UserId == null, () =>
        {
            RuleFor(x => x.Email)
                .NotEmptyWithMessage(nameof(InviteReviewerCommand.Email))
                .MaximumLengthWithMessage(MaxLength.C64, nameof(InviteReviewerCommand.Email))
                .EmailAddress();

            RuleFor(x => x.FirstName)
                .NotEmptyWithMessage(nameof(InviteReviewerCommand.FirstName))
                .MaximumLengthWithMessage(MaxLength.C64, nameof(InviteReviewerCommand.FirstName));

            RuleFor(x => x.LastnName)
                .NotEmpty()
                .MaximumLength(MaxLength.C256);
        });
    }
}
