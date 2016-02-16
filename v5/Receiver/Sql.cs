using NServiceBus;
using NServiceBus.Hosting.Profiles;

namespace Receiver
{
    public class Sql : IProfile
    {
        public class Profile : IHandleProfile<Sql>
        {
            public void ProfileActivated(BusConfiguration busConfiguration)
            {
                busConfiguration.UseTransport<SqlServerTransport>()
                    .DisableCallbackReceiver();
            }
        }
    }
}