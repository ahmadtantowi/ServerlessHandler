using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using ServerlessHandler.Abstractions;

namespace ServerlessHandler.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddHttpHandler(this IServiceCollection services, Type functionApp)
        {
            var handlers = functionApp.Assembly
                .GetTypes()
                .Where(handler 
                    => typeof(IHttpHandler).IsAssignableFrom(handler) 
                    && handler != typeof(IHttpHandler) 
                    && !handler.IsInterface 
                    && !handler.IsAbstract);
            
            foreach (var handler in handlers)
                services.AddTransient(handler);
            
            return services;
        }
    }
}