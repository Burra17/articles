using Articles.Abstractions.Events;
using MassTransit;

namespace ArticleHub.API.Articles.Consumers;

public class ArticleApprovedForReviewConsumer : IConsumer<ArticleApprovedForReviewEvent>
{
    public Task Consume(ConsumeContext<ArticleApprovedForReviewEvent> context)
    {
        throw new NotImplementedException();
    }
}
