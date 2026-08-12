using Custodian.Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Custodian.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //---- Get Assembly ----
        var assembly = Assembly.GetExecutingAssembly();

        //--- Register All Objects implmenting IRequesthandler ----
        services.AddMediatR(cfg => 
        { 
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        //---- Register Fluent Validation ----
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
