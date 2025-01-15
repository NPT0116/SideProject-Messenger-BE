
using Application.Data;
using Domain.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        // Extension methods 
        public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IUnitOfWork>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IUserRepository, UserRepository>();

            //services.AddScoped<IOrderRepository, OrderRepository>();

            //services.AddScoped<IOrderSummaryRepository, OrderSummariesRepository>();

            //services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
