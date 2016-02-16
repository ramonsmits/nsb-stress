using NServiceBus;
using NServiceBus.Hosting.Profiles;

namespace Receiver
{
    public class Msmq : IProfile
    {
        public class Profile : IHandleProfile<Msmq>
        {
            public void ProfileActivated(BusConfiguration busConfiguration)
            {
                busConfiguration.UseTransport<MsmqTransport>();
            }
        }
    }
}