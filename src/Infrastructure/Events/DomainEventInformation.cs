using System;
using System.Collections.Generic;
using System.Linq;
using SharedKernel.Application.Reflection;
using SharedKernel.Domain.Events;

namespace SharedKernel.Infrastructure.Events
{
    public class DomainEventsInformation
    {
        private readonly Dictionary<string, Type> _indexedDomainEvents = new Dictionary<string, Type>();

        public DomainEventsInformation()
        {
            foreach (var eventType in GetDomainTypes())
            {
                if (!_indexedDomainEvents.ContainsKey(GetEventName(eventType)))
                    _indexedDomainEvents.Add(GetEventName(eventType), eventType);
            }
        }

        public Type ForName(string name)
        {
            _indexedDomainEvents.TryGetValue(name, out var value);
            return value;
        }

        public string ForClass(DomainEvent domainEvent)
        {
            return _indexedDomainEvents.FirstOrDefault(x => x.Value == domainEvent.GetType()).Key;
        }

        private string GetEventName(Type eventType)
        {
            var instance = ReflectionHelper.CreateInstance<DomainEvent>(eventType);
            return eventType.GetMethod(nameof(DomainEvent.GetEventName))?.Invoke(instance, null)?.ToString();
        }

        private IEnumerable<Type> GetDomainTypes()
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(DomainEvent).IsAssignableFrom(p) && !p.IsAbstract);
            }
            catch (Exception)
            {
                return AppDomain.CurrentDomain.GetAssemblies()
#if !NETSTANDARD2_1 && !NET461
                    .Where(a => a.FullName != null)
#endif
                    .Where(a => a.FullName.Contains("Domain"))
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(DomainEvent).IsAssignableFrom(p) && !p.IsAbstract);
            }
        }
    }
}