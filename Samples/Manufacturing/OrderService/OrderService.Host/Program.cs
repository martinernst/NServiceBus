using System;
using Common.Logging;
using NHibernate.Cfg;
using NServiceBus;
using NServiceBus.Grid.MessageHandlers;
using NServiceBus.Unicast.Config;
using NServiceBus.Unicast.Subscriptions.Msmq.Config;
using NServiceBus.Unicast.Transport.Msmq.Config;
using NServiceBus.Saga;
using OrderService.Persistence;
using NHibernate;
using ObjectBuilder;

namespace OrderService.Host
{
    class Program
    {
        static void Main()
        {
            LogManager.GetLogger("hello").Debug("Order Started.");
            ObjectBuilder.SpringFramework.Builder builder = new ObjectBuilder.SpringFramework.Builder();

            try
            {
                Configuration config = new Configuration();
                config.Configure();

                ISessionFactory sessionFactory = config.BuildSessionFactory();

                new ConfigMsmqSubscriptionStorage(builder);

                NServiceBus.Serializers.Configure.BinarySerializer.With(builder);
                //NServiceBus.Serializers.Configure.XmlSerializer.With(builder);

                new ConfigMsmqTransport(builder)
                    .IsTransactional(true)
                    .PurgeOnStartup(false);

                new ConfigUnicastBus(builder)
                    .ImpersonateSender(false)
                    .SetMessageHandlersFromAssembliesInOrder(
                        typeof(GridInterceptingMessageHandler).Assembly
                        , typeof(SagaMessageHandler).Assembly
                        , typeof(OrderSagaFinder).Assembly
                        , typeof(OrderSaga).Assembly
                    );

                new NServiceBus.SagaPersisters.NHibernate.Configure(builder, sessionFactory);

                builder.ConfigureComponent<OrderSagaFinder>(ComponentCallModelEnum.Singlecall)
                    .SessionFactory = sessionFactory;

                IBus bServer = builder.Build<IBus>();
                bServer.Start();
            }
            catch (Exception e)
            {
                LogManager.GetLogger("hello").Fatal("Exiting", e);
                Console.Read();
            }

            Console.Read();

        }
    }
}