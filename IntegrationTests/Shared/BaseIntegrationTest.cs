using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Shared
{
    public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly IServiceScope _scope;
        protected readonly ISender _sender;
        protected readonly ITokenService _tokenService;
        protected BaseIntegrationTest(CustomWebApplicationFactory factory)
        {
            _scope = factory.Services.CreateScope();

            _sender = _scope.ServiceProvider.GetRequiredService<ISender>();
            _tokenService = _scope.ServiceProvider.GetRequiredService<ITokenService>();
        }

        protected void SetupAuthentication(HttpClient client, Guid? userId)
        {
            if(userId == null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", null);
                return;
            }
            
            var token = _tokenService.CreateUserToken(new User((Guid)userId, "pinkwar123", "Nguyen", "Hong Quan", "abc")); // Fake user ID
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected void ClearAuthentication(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = null; // Remove Bearer token
            client.DefaultRequestHeaders.Remove("Authorization"); // Ensure it's fully cleared
        }

    }
}