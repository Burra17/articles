using Articles.Abstractions.Events;
using MassTransit;

namespace Review.Application.Features.Articles.InitializeFromSubmission;

public class ArticleApprovedForReviewEventHandler : IConsumer<ArticleApprovedForReviewEvent>
{
    public async Task Consume(ConsumeContext<ArticleApprovedForReviewEvent> context)
    {
        var articleDto = context.Message.Article;

        throw new NotImplementedException();
    }
}
