using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Shared
{
    public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly IServiceScope _scope;
        private readonly ISender _sender;
        protected readonly ITokenService _tokenService;
        protected BaseIntegrationTest(CustomWebApplicationFactory factory)
        {
            _scope = factory.Services.CreateScope();

            _sender = _scope.ServiceProvider.GetRequiredService<ISender>();
            _tokenService = _scope.ServiceProvider.GetRequiredService<ITokenService>();
        }
    }
}