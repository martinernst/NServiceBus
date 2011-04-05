﻿using NServiceBus.Config;
using NServiceBus.ObjectBuilder;
using NServiceBus.Unicast.Queuing.Msmq;

namespace NServiceBus
{
    public static class ConfigureMsmqMessageQueue
    {
        /// <summary>
        /// Indicates that MsmqMessageQueue has been selected.
        /// </summary>
        public static bool Selected { get; set; }

        /// <summary>
        /// Use MSMQ for your queuing infrastructure.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Configure MsmqTransport(this Configure config)
        {
            Selected = true;

            config.Configurer.ConfigureComponent<MsmqMessageReceiver>(DependencyLifecycle.SingleInstance);
            config.Configurer.ConfigureComponent<MsmqMessageSender>(DependencyLifecycle.SingleInstance);


            var cfg = Configure.GetConfigSection<MsmqMessageQueueConfig>();

            if (cfg != null)
            {
                config.Configurer.ConfigureProperty<MsmqMessageSender>(t => t.UseDeadLetterQueue, cfg.UseJournalQueue);
                config.Configurer.ConfigureProperty<MsmqMessageSender>(t => t.UseJournalQueue, cfg.UseJournalQueue);
            }

            return config;
        }

        /// <summary>
        /// Requests that the incoming queue be purged of all messages when the bus is started.
        /// All messages in this queue will be deleted if this is true.
        /// Setting this to true may make sense for certain smart-client applications, 
        /// but rarely for server applications.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Configure PurgeOnStartup(this Configure config, bool value)
        {
            config.Configurer.ConfigureProperty<MsmqMessageReceiver>(t => t.PurgeOnStartup, value);

            return config;
        }
    }
}
