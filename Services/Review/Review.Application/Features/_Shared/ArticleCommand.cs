using Articles.Abstractions;
using Blocks.Core.FluenValidation;
using Blocks.MediatR;
using FluentValidation;
using Review.Domain.Shared.Enums;

namespace Review.Application.Features.Shared;

public abstract record ArticleCommand<TActionType, TResponse> : ArticleCommandBase<TActionType>, ICommand<TResponse>
        where TActionType : Enum;

public abstract record ArticleCommand : ArticleCommand<ArticleActionType, IdResponse>;

public abstract class ArticleCommandValidator<TFileActionCommand> : AbstractValidator<TFileActionCommand>
    where TFileActionCommand : IArticleAction
{
    public ArticleCommandValidator()
    {
        RuleFor(c => c.ArticleId).GreaterThan(0).WithMessageForInvalidId(nameof(ArticleCommand.ArticleId));
    }
}

