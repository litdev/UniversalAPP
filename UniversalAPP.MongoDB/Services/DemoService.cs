using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace UniversalAPP.MongoDB.Services
{
    public class DemoService : BaseService<Models.Demo>
    {
        public DemoService(IConfiguration config) : base(config, nameof(Models.Demo))
        {

        }
    }
}
