﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Events.Redis
{
    public class RedisDomainEventsConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RedisDomainEventsConsumer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var initialScope = _serviceScopeFactory.CreateScope();

            var domainEventJsonDeserializer = initialScope.ServiceProvider.GetRequiredService<DomainEventJsonDeserializer>();

            var domainEventMediator = initialScope.ServiceProvider.GetRequiredService<DomainEventMediator>();

            var redisSubscriber = initialScope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>().GetSubscriber();

            await redisSubscriber.SubscribeAsync("*", async (_, value) =>
            {
                var @event = domainEventJsonDeserializer.Deserialize(value);

                var subscribers = DomainEventSubscriberInformationService.GetAllEventsSubscribers(@event);

                await Task.WhenAll(subscribers.Select(subscriber =>
                    domainEventMediator.ExecuteOn(@event, subscriber, stoppingToken)));
            });
        }
    }
}
