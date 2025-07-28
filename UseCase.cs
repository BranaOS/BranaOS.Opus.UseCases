using BranaOS.Opus.Core;

namespace BranaOS.Opus.UseCases;

public interface IRequest<TResponse> { }

public interface IUseCase<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
  Task<Result<Nothing>> Validate(TRequest request);
  Task<Result<IRequest<TResponse>>> Before(TRequest request);
  Task<Result<TResponse>> After(TRequest request, TResponse response);
  Task<Result<TResponse>> Handle(TRequest request);
}

public abstract class UseCase<TRequest, TResponse>
  : IUseCase<TRequest, TResponse>
  where TRequest
    : IRequest<TResponse>
{
  public virtual Task<Result<Nothing>> Validate(TRequest request)
  {
    return Task.FromResult(Result.Ok(Nothing._));
  }

  public virtual Task<Result<IRequest<TResponse>>> Before(TRequest request)
  {
    return Task.FromResult(Result.Ok((IRequest<TResponse>)request));
  }

  public virtual Task<Result<TResponse>> After(TRequest request, TResponse response)
  {
    return Task.FromResult(Result.Ok(response));
  }

  internal async Task<Result<TResponse>> Execute(TRequest request)
  {
    var validationResult = await Validate(request);

    if (validationResult.IsFailure)
    {
      return Result.Fail<TResponse>(validationResult.Errors);
    }

    var beforeResult = await Before(request);

    if (beforeResult.IsFailure)
    {
      return Result.Fail<TResponse>(beforeResult.Errors);
    }

    TRequest newRequest = (TRequest)beforeResult.Value;

    var response = await Handle(newRequest);

    if (response.IsFailure)
    {
      return response;
    }

    var afterResult = await After(newRequest, response.Value);

    if (afterResult.IsFailure)
    {
      return afterResult;
    }

    return Result.Ok(afterResult.Value);
  }

  public abstract Task<Result<TResponse>> Handle(TRequest request);
}