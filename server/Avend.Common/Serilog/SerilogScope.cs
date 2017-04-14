using System;
using System.Collections.Generic;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace Avend.API.Infrastructure
{
    class SerilogScope : ILogEventEnricher
    {
        private readonly SerilogScope _parent;
        private readonly object _state;

        private readonly LinkedList<object> _scopesChain;

        public SerilogScope(SerilogScope parent, object state)
        {
            _parent = parent;
            _state = state;
            var scope = GetScopeValue(state);
            _scopesChain = parent?._scopesChain ?? new LinkedList<object>();
            _scopesChain.AddLast(scope);
        }


#if NET451
        private static readonly string FieldKey = $"{typeof(SerilogScope).FullName}.Value.{AppDomain.CurrentDomain.Id}";
        public static SerilogScope Current
        {
            get
            {
                var handle = CallContext.LogicalGetData(FieldKey) as ObjectHandle;
                if (handle == null)
                {
                    return default(SerilogScope);
                }

                return (SerilogScope)handle.Unwrap();
            }
            set
            {
                CallContext.LogicalSetData(FieldKey, new ObjectHandle(value));
            }
        }
#else
        private static AsyncLocal<SerilogScope> _value = new AsyncLocal<SerilogScope>();
        public static SerilogScope Current
        {
            get
            {
                return _value.Value;
            }
            set
            {
                _value.Value = value;
            }
        }
#endif

        public static IDisposable Push(object state)
        {
            Current = new SerilogScope(Current, state);
            return new DisposableScope();
        }

        public override string ToString()
        {
            return _state?.ToString();
        }

        private class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                if (Current._scopesChain.Last != null)
                {
                    Current._scopesChain.RemoveLast();
                }
                Current = Current._parent;
            }
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            SerilogScope scope = this;
            while (scope != null)
            {
                var properties = scope._state as IEnumerable<KeyValuePair<string, object>>;
                if (properties != null)
                {
                    foreach (var kv in properties)
                    {
                        if (kv.Key == SerilogLogger.OriginalFormatPropertyName)
                        {
                            continue;
                        }
                        var key = kv.Key;
                        var destructureObject = false;
                        if (key.StartsWith("@"))
                        {
                            key = key.Substring(1);
                            destructureObject = true;
                        }
                        var property = propertyFactory.CreateProperty(key, kv.Value, destructureObject);
                        logEvent.AddPropertyIfAbsent(property);
                    }
                }

                scope = scope._parent;
            }

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Scope", _scopesChain));
        }

        private object GetScopeValue(object state)
        {
            var properties = state as IEnumerable<KeyValuePair<string, object>>;
            if (properties != null)
            {
                foreach (var kv in properties)
                {
                    if (kv.Key == SerilogLogger.OriginalFormatPropertyName)
                    {
                        return kv.Value;
                    }
                }

            }
            return state;
        }
    }
}