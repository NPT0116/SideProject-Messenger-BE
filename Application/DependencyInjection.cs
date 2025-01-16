// filepath: /C:/Users/Admin/Desktop/web_messenger/Application/DependencyInjection.cs
using FluentValidation;
using MediatR;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
                config.NotificationPublisher = new TaskWhenAllPublisher();
            });

            services.AddValidatorsFromAssembly(assembly);
            services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
            // Register the ValidationBehavior
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}