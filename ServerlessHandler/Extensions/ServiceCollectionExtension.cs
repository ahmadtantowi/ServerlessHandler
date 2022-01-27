using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using ServerlessHandler.Abstractions;

namespace ServerlessHandler.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServerlessHandler(this IServiceCollection services, Type functionApp)
        {
            var handlers = functionApp.Assembly
                .GetTypes()
                .Where(handler 
                    => typeof(IHandler).IsAssignableFrom(handler) 
                    && handler != typeof(IHandler) 
                    && !handler.IsInterface 
                    && !handler.IsAbstract);
            
            foreach (var handler in handlers)
                services.AddTransient(handler);
            
            return services;
        }
    }
}