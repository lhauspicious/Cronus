﻿using Elders.Cronus.EventStore.Index;
using Elders.Cronus.Logging;
using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Multitenancy;
using System;

namespace Elders.Cronus.InMemory
{
    public class InMemoryPublisher<T> : Publisher<IMessage> where T : IMessage
    {
        private readonly static ILog log = LogProvider.GetLogger(typeof(InMemoryPublisher<>));

        private readonly ISubscriberCollection<IApplicationService> appServiceSubscribers;
        private readonly ISubscriberCollection<IProjection> projectionSubscribers;
        private readonly ISubscriberCollection<IPort> portSubscribers;
        private readonly ISubscriberCollection<IGateway> gatewaySubscribers;
        private readonly ISubscriberCollection<ISaga> sagaSubscribers;
        private readonly ISubscriberCollection<IEventStoreIndex> esIndexSubscribers;
        private readonly ScopedQueues queues = new ScopedQueues();

        public InMemoryPublisher(
            ISubscriberCollection<IApplicationService> appServiceSubscribers,
            ISubscriberCollection<IProjection> projectionSubscribers,
            ISubscriberCollection<IPort> portSubscribers,
            ISubscriberCollection<IGateway> gatewaySubscribers,
            ISubscriberCollection<ISaga> sagaSubscribers,
            ISubscriberCollection<IEventStoreIndex> esIndexSubscribers)
            : base(new DefaultTenantResolver())
        {
            this.appServiceSubscribers = appServiceSubscribers;
            this.projectionSubscribers = projectionSubscribers;
            this.portSubscribers = portSubscribers;
            this.gatewaySubscribers = gatewaySubscribers;
            this.sagaSubscribers = sagaSubscribers;
            this.esIndexSubscribers = esIndexSubscribers;
        }

        protected override bool PublishInternal(CronusMessage message)
        {
            using (var queue = queues.GetQueue(message))
            {
                queue.Enqueue(message);
                while (queue.Any())
                {
                    var msg = queue.Dequeue();
                    NotifySubscribers(msg, appServiceSubscribers);
                    NotifySubscribers(msg, esIndexSubscribers);
                    NotifySubscribers(msg, projectionSubscribers);
                    NotifySubscribers(msg, portSubscribers);
                    NotifySubscribers(msg, gatewaySubscribers);
                    NotifySubscribers(msg, sagaSubscribers);
                }
            }
            return true;
        }

        void NotifySubscribers<TContract>(CronusMessage message, ISubscriberCollection<TContract> subscribers)
        {
            try
            {
                var interestedSubscribers = subscribers.GetInterestedSubscribers(message);
                foreach (var subscriber in interestedSubscribers)
                {
                    subscriber.Process(message);
                }
            }
            catch (Exception ex)
            {
                log.Error("Unable to process message", ex);
            }
        }
    }
}
