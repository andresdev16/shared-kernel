using Microsoft.Extensions.Options;
using RabbitMQ.Client.Exceptions;
using SharedKernel.Domain.Events;
using SharedKernel.Infrastructure.Cqrs.Middlewares;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace SharedKernel.Infrastructure.Events.RabbitMq
{
    public class RabbitMqEventBus : IEventBus
    {
        private const string HeaderReDelivery = "redelivery_count";
        // private readonly MsSqlEventBus _failOverPublisher;
        private readonly ExecuteMiddlewaresService _executeMiddlewaresService;
        private readonly DomainEventJsonSerializer _domainEventJsonSerializer;
        private readonly RabbitMqConnectionFactory _config;
        private readonly IOptions<RabbitMqConfigParams> _rabbitMqParams;

        public RabbitMqEventBus(
            // MsSqlEventBus failOverPublisher,
            ExecuteMiddlewaresService executeMiddlewaresService,
            DomainEventJsonSerializer domainEventJsonSerializer,
            RabbitMqConnectionFactory config,
            IOptions<RabbitMqConfigParams> rabbitMqParams)
        {
            // _failOverPublisher = failOverPublisher;
            _executeMiddlewaresService = executeMiddlewaresService;
            _domainEventJsonSerializer = domainEventJsonSerializer;
            _config = config;
            _rabbitMqParams = rabbitMqParams;
        }

        public Task Publish(List<DomainEvent> events, CancellationToken cancellationToken)
        {
            return Task.WhenAll(events.Select(@event => Publish(@event, cancellationToken)));
        }

        public async Task Publish(DomainEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                await _executeMiddlewaresService.ExecuteAsync(@event, cancellationToken);

                var serializedDomainEvent = _domainEventJsonSerializer.Serialize(@event);

                var channel = _config.Channel();
                channel.ExchangeDeclare(_rabbitMqParams.Value.ExchangeName, ExchangeType.Topic);

                var body = Encoding.UTF8.GetBytes(serializedDomainEvent);
                var properties = channel.CreateBasicProperties();
                properties.Headers = new Dictionary<string, object> { { HeaderReDelivery, 0 } };

                channel.BasicPublish(_rabbitMqParams.Value.ExchangeName, @event.GetEventName(), properties, body);
            }
            catch (RabbitMQClientException)
            {
                //await _failOverPublisher.Publish(new List<DomainEvent> {domainEvent}, cancellationToken);
            }
        }
    }
}