using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface INotificationConsumer
    {
        public Task StartConsuming();
        public void Dispose();
    }
}