using System;
using BranaOS.Opus.Core;

namespace BranaOS.Opus.UseCases.Behaviors.Abstract;

public interface IPipelineBehavior<TRequest, TResponse>
{
  Task<Result<TResponse>> Execute(TRequest request, Func<Task<Result<TResponse>>> next);
}
