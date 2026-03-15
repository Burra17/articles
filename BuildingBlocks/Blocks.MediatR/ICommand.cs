using MediatR;

namespace Blocks.MediatR;

public interface ICommand<out TResponse> : IRequest<TResponse>;

