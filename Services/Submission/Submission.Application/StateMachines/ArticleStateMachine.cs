using Blocks.Core.Cache;
using Microsoft.Extensions.Caching.Memory;
using Stateless;
using Submission.Domain.Entities;
using Submission.Domain.StateMachines;

namespace Submission.Application.StateMachines;

public class ArticleStateMachine : IArticleStateMachine
{
    private readonly StateMachine<ArticleStage, ArticleActionType> _stateMachine;

    public ArticleStateMachine(ArticleStage articleStage, IMemoryCache cache)
    {
        _stateMachine = new StateMachine<ArticleStage, ArticleActionType>(articleStage);

        var transitions = cache.Get<List<ArticleStageTransition>>();
        foreach (var transition in transitions)
        {
            if (transition.CurrentStage != transition.DestinationStage)
                _stateMachine.Configure(transition.CurrentStage)
                    .Permit(transition.ActionType, transition.DestinationStage);
            else
                _stateMachine.Configure(transition.CurrentStage)
                    .PermitReentry(transition.ActionType);
        }
    }

    public bool CanFire(ArticleActionType actionType)
        => _stateMachine.CanFire(actionType);
}
