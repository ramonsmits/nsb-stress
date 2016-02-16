using NServiceBus;
using NServiceBus.Hosting.Profiles;

namespace Receiver
{
    public class SqlTransport : IProfile
    {
        public class SqlTransportProfile : IHandleProfile<SqlTransport>
        {
            public void ProfileActivated()
            {
                Configure.Serialization.Json();
                var configuration = Configure.Instance;
                configuration.InMemorySubscriptionStorage();
                configuration.InMemoryFaultManagement();
                configuration.InMemorySagaPersister();
                configuration.UseInMemoryTimeoutPersister();
                //configuration.UseNHibernateTimeoutPersister();
                //configuration.UseTransport<SqlServer>();
                configuration.UseTransport<Msmq>();
            }
        }
    }
}