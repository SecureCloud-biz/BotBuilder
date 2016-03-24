﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Bot.Builder.Fibers
{
    public static class Serialization
    {
        [Serializable]
        public sealed class ObjectReference : IObjectReference
        {
            private readonly Type type;
            public ObjectReference(SerializationInfo info, StreamingContext context)
            {
                Field.SetNotNullFrom(out this.type, nameof(type), info);
            }

            public static void GetObjectData(SerializationInfo info, Type type)
            {
                info.SetType(typeof(ObjectReference));
                info.AddValue(nameof(type), type);
            }

            object IObjectReference.GetRealObject(StreamingContext context)
            {
                var provider = (IServiceProvider)context.Context;
                return provider.GetService(this.type);
            }
        }

        public sealed class ReferenceSurrogate : ISerializationSurrogate
        {
            void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                ObjectReference.GetObjectData(info, obj.GetType());
            }

            object ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                throw new NotImplementedException();
            }
        }

        public sealed class ReflectionSurrogate : ISerializationSurrogate
        {
            public const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                var type = obj.GetType();
                var fields = type.GetFields(Flags);
                foreach (var field in fields)
                {
                    var value = field.GetValue(obj);
                    info.AddValue(field.Name, value);
                }
            }

            object ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                var type = obj.GetType();
                var fields = type.GetFields(Flags);
                foreach (var field in fields)
                {
                    var value = info.GetValue(field.Name, field.FieldType);
                    field.SetValue(obj, value);
                }

                return obj;
            }
        }

        public sealed class LogSurrogate : ISerializationSurrogate
        {
            private readonly HashSet<Type> visited = new HashSet<Type>();
            private readonly ISerializationSurrogate inner;
            // TOOD: better tracing interface
            private readonly TraceListener trace;

            public LogSurrogate(ISerializationSurrogate inner, TraceListener trace)
            {
                Field.SetNotNull(out this.inner, nameof(inner), inner);
                Field.SetNotNull(out this.trace, nameof(trace), trace);
            }

            void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                this.Visit(obj);
                this.inner.GetObjectData(obj, info, context);
            }

            object ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                this.Visit(obj);
                return this.inner.SetObjectData(obj, info, context, selector);
            }

            private void Visit(object obj)
            {
                var type = obj.GetType();
                if (this.visited.Add(type))
                {
                    var message = $"{this.inner.GetType().Name}: visiting {type}";
                    this.trace.WriteLine(message);
                }
            }
        }

        public interface ISerializeAsReference
        {
        }

        public sealed class SurrogateSelector : ISurrogateSelector
        {
            private readonly ISerializationSurrogate reference;
            private readonly ISerializationSurrogate reflection;
            public SurrogateSelector(ISerializationSurrogate reference, ISerializationSurrogate reflection)
            {
                this.reference = reference;
                this.reflection = reflection;
            }

            void ISurrogateSelector.ChainSelector(ISurrogateSelector selector)
            {
                throw new NotImplementedException();
            }

            ISurrogateSelector ISurrogateSelector.GetNextSelector()
            {
                throw new NotImplementedException();
            }

            ISerializationSurrogate ISurrogateSelector.GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
            {
                if (this.reference != null && typeof(ISerializeAsReference).IsAssignableFrom(type))
                {
                    selector = this;
                    return this.reference;
                }

                if (this.reflection != null && !type.IsSerializable)
                {
                    selector = this;
                    return this.reflection;
                }

                selector = null;
                return null;
            }
        }

        public sealed class SimpleServiceLocator : IServiceProvider, IEnumerable<object>
        {
            private readonly Dictionary<Type, object> instanceByType;

            public SimpleServiceLocator(IEnumerable<object> instances = null)
            {
                instances = instances ?? Enumerable.Empty<object>();
                this.instanceByType = instances.ToDictionary(o => o.GetType(), o => o);
            }

            public void Add(object instance)
            {
                var type = instance.GetType();
                this.instanceByType.Add(type, instance);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.instanceByType.Values.GetEnumerator();
            }

            IEnumerator<object> IEnumerable<object>.GetEnumerator()
            {
                return this.instanceByType.Values.GetEnumerator();
            }

            object IServiceProvider.GetService(Type serviceType)
            {
                return this.instanceByType[serviceType];
            }
        }
    }
}
