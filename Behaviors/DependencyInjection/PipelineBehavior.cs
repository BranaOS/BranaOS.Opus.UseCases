using System;
using BranaOS.Opus.UseCases.Behaviors.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace BranaOS.Opus.UseCases.Behaviors.DependencyInjection;

public static class PipelineExtensions
{
  public static IServiceCollection AddUseCasePipeline(
    this IServiceCollection services,
    Action<PipelineBuilder> configure
  )
  {
    var builder = new PipelineBuilder(services);
    configure(builder);

    builder.RegisterBehaviors();

    return services;
  }
}

public class PipelineBuilder(IServiceCollection _services)
{
  private readonly List<Type> _behaviorTypes = [];

  public PipelineBuilder Add(Type behaviorType)
  {
    if (!behaviorType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)))
    {
      throw new ArgumentException("The behavior type must implement IPipelineBehavior<,>");
    }

    _behaviorTypes.Add(behaviorType);
    return this;
  }

  internal void RegisterBehaviors()
  {
    foreach (var type in _behaviorTypes)
    {
      _services.AddTransient(typeof(IPipelineBehavior<,>), type);
    }
  }
}
