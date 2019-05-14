using Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

namespace Sales.ClientUi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            AddServiceBus(services);

            var storageConnString = Configuration.GetValue<string>("StorageConnString");
            services.AddSingleton(new StorageRepository(storageConnString));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }

        #region Private Methods
        
        #region NServiceBus

        private void AddServiceBus(IServiceCollection services)
        {
            var sbConnString = Configuration["AzureServiceBusConnString"];

            var endpointConfiguration = new EndpointConfiguration("Sales.ClientUi");
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.UseContainer<ServicesBuilder>(
                customizations =>
                {
                    customizations.ExistingServices(services);
                });
            endpointConfiguration.SendOnly();
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(sbConnString);
            //transport.UseForwardingTopology();

            var endpoint = Endpoint.Start(endpointConfiguration).ConfigureAwait(false)
                .GetAwaiter().GetResult();

            services.AddSingleton<IMessageSession>(endpoint);
        }

        #endregion //NServiceBus
        
        #endregion //Private Methods
    }
}
