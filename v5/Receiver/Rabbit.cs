using NServiceBus;
using NServiceBus.Hosting.Profiles;

namespace Receiver
{
    public class Rabbit : IProfile
    {
        public class Profile : IHandleProfile<Rabbit>
        {
            public void ProfileActivated(BusConfiguration busConfiguration)
            {
                busConfiguration.UseTransport<RabbitMQTransport>()
                    .DisableCallbackReceiver()
                    .ConnectionStringName("NServiceBus/Transport/Rabbit");
            }
        }
    }
}