using Microsoft.Extensions.DependencyInjection;
using Stonks.Core.Rewrite.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Service
{
    public class TrackerService
    {
        private readonly Setting _setting;
        private IServiceProvider _service;

        public TrackerService(IServiceProvider service)
        {
            _service = service;
            _setting = service.GetRequiredService<Setting>();
        }
    }
}