using event_bus.Consumers;
using event_bus.Context;
using event_bus.Encryption;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NATS.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace event_bus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options => options.UseSqlServer("Server=host.docker.internal,1433;Database=TransferData;User ID=SA;Password=1Secure*Password1;"));
            IConnection _con = AddNatsClient();
            services.AddSingleton<IConnection>(x => _con);
            services.AddHostedService<EventBusCreator>();
            services.AddHostedService<ConsistencyKeeper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        public static IConnection AddNatsClient()
        {
            ConnectionFactory cf = new ConnectionFactory();
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = "nats://host.docker.internal:4222";
            opts.Timeout = 15500;
            IConnection c = cf.CreateConnection(opts);
            return c;
        }
    }
}
