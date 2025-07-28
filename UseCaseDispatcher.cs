
using BranaOS.Opus.Core;
using BranaOS.Opus.UseCases.Behaviors.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace BranaOS.Opus.UseCases;

public interface IUseCaseDispatcher
{
  Task<Result<TResponse>> Dispatch<TResponse>(IRequest<TResponse> request);
}

public class UseCaseDispatcher(IServiceProvider _serviceProvider) : IUseCaseDispatcher
{
  public Task<Result<TResponse>> Dispatch<TResponse>(IRequest<TResponse> request)
  {
    var requestType = request.GetType();

    var handlerType = typeof(IUseCase<,>).MakeGenericType(requestType, typeof(TResponse));
    var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));

    var handler = _serviceProvider.GetRequiredService(handlerType);
    var pipelines = _serviceProvider.GetServices(pipelineType).Cast<dynamic>();

    Func<Task<Result<TResponse>>> next = () =>
      (Task<Result<TResponse>>)handler.GetType().GetMethod("Execute")!.Invoke(handler, [request])!;

    foreach (var pipeline in pipelines.Reverse())
    {
      var previousNext = next;
      next = () => pipeline.Execute((dynamic)request, previousNext);
    }

    return next();
  }
}
