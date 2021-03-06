using SharedKernel.Domain.Events;
using System.Collections.Generic;
using System.Linq;

namespace SharedKernel.Infrastructure.Events
{
    /// <summary>
    /// 
    /// </summary>
    public static class DomainEventExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToPrimitives(this DomainEvent domainEvent)
        {
            var primitives = new Dictionary<string, string>();

            // All public properties
            domainEvent
                .GetType()
                .GetProperties()
                .Where(p => p.Name != nameof(DomainEvent.AggregateId) && p.Name != nameof(DomainEvent.EventId) && p.Name != nameof(DomainEvent.OccurredOn))
                .ToList()
                .ForEach(p => primitives.Add(p.Name, p.GetValue(domainEvent, null)?.ToString()));

            return primitives;
        }
    }
}
