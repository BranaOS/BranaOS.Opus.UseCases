using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BranaOS.Opus.UseCases;

public static class DependencyInjection
{
  public static IServiceCollection RegisterUseCases(this IServiceCollection services, Assembly assembly)
  {
    services.AddScoped<IUseCaseDispatcher, UseCaseDispatcher>();

    static bool IsSubclassOfUseCase(Type type)
    {
      while (type != null && type != typeof(object))
      {
        var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        if (cur == typeof(IUseCase<,>))
        {
          return true;
        }

        type = type.BaseType!;
      }

      return false;
    }

    var useCaseTypes = assembly.GetTypes()
      .Where(t => t.IsClass && !t.IsAbstract && IsSubclassOfUseCase(t))
      .ToList();

    foreach (var useCaseType in useCaseTypes)
    {
      var interfaceType = useCaseType.GetInterfaces()
        .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUseCase<,>));

      services.AddTransient(interfaceType, useCaseType);
    }

    return services;
  }
}
