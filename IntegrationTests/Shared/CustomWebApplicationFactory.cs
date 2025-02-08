using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Shared
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("messengerdb")
            .WithUsername("postgres")
            .WithPassword("admin")
            .Build(); 
        public ApplicationDbContext _dbContext { get; private set; }
        public IFriendshipRepository _friendshipRepository { get; private set; }

        public Task InitializeAsync()
        {
            return _dbContainer.StartAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IHttpContextAccessor));
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                services.AddAuthentication("TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
                services.AddAuthorization();


                // Remove real database context
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(_dbContainer.GetConnectionString());
                });

                var serviceProvider = services.BuildServiceProvider();
                _dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

                descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDistributedCache));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory cache instead of Redis
                services.AddDistributedMemoryCache();

                var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;

                _friendshipRepository = scopedServices.GetRequiredService<IFriendshipRepository>();
                
            });
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return _dbContainer.StopAsync();
        }
    }
}