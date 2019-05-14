using System;
using System.Configuration;
using System.Threading.Tasks;
using Autofac;
using Core;
using NServiceBus;

namespace Billing.Webjob
{
    public class ServiceBusHost
    {
        private IEndpointInstance _endpointInstance;
        
        public async Task<IEndpointInstance> StartAsync()
        {
            var sbConnString = ConfigurationManager.AppSettings["AzureServiceBusConnString"];
            var endpointName = ConfigurationManager.AppSettings["QueueName"];
            var storageConnString = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;

            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.EnableInstallers();

            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(sbConnString);

            endpointConfiguration.SendFailedMessagesTo($"{endpointName}_errors");
            
            /*endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.SendHeartbeatTo("Particular.ServiceControl");
            var metrics = endpointConfiguration.EnableMetrics();
            metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));*/

            transport.UseForwardingTopology();

            var builder = new ContainerBuilder();

            builder.Register(x => _endpointInstance).As<IEndpointInstance>().SingleInstance();
            builder.Register(x => new StorageRepository(storageConnString)).AsSelf();

            var container = builder.Build();
            endpointConfiguration.UseContainer<AutofacBuilder>(
                customizations =>
                {
                    customizations.ExistingLifetimeScope(container);
                });

            _endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            return _endpointInstance;
        }

        public async Task StopAsync()
        {
            await _endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}