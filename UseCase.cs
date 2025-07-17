using BranaOS.Opus.Core;

namespace BranaOS.Opus.UseCases;

public interface IRequest<TResponse> { }

public interface IUseCase<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
  Task<Result<TResponse>> Handle(TRequest request);
}

public abstract class UseCase<TRequest, TResponse>
  : IUseCase<TRequest, TResponse>
  where TRequest
    : IRequest<TResponse>
{
  public abstract Task<Result<TResponse>> Handle(TRequest request);
}