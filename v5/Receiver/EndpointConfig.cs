
namespace Receiver
{
    using NServiceBus;

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
    {
        public void Customize(BusConfiguration busConfiguration)
        {
            busConfiguration.PurgeOnStartup(true);

            var endpointName = "memleaktest";
            busConfiguration.EndpointName(endpointName);
            busConfiguration.EnableInstallers();
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.UsePersistence<InMemoryPersistence>();
        }
    }
}
