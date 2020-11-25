using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using SharedKernel.Application.Reflection;
using SharedKernel.Domain.Events;

namespace SharedKernel.Infrastructure.Events
{
    public class DomainEventJsonDeserializer
    {
        private readonly DomainEventsInformation _information;

        public DomainEventJsonDeserializer(DomainEventsInformation information)
        {
            _information = information;
        }

        public DomainEvent Deserialize(string body)
        {
            var eventData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(body);

            var data = eventData["data"];
            var attributesString = data["attributes"].ToString();

            var attributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(attributesString!);

            var domainEventType = _information.ForName((string) data["type"]);

            var instance = ReflectionHelper.CreateInstance<DomainEvent>(domainEventType);

            var domainEvent = (DomainEvent) domainEventType
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(DomainEvent.FromPrimitives))
                ?.Invoke(instance, new object[]
                {
                    attributes["id"],
                    attributes,
                    data["id"].ToString(),
                    data["occurred_on"].ToString()
                });

            return domainEvent;
        }
    }
}