using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Castle.Windsor;
using NServiceBus.ObjectBuilder.Common;
using NServiceBus.Utils;

namespace NServiceBus.ObjectBuilder.CastleWindsor
{
    ///<summary>
    /// Releases all transient and pooled objects when disposed
    ///</summary>
    public class TrackingContainer : IContainer
    {
        private readonly IWindsorContainer _parent;
        private readonly IList<object> _tracked = new List<object>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private bool _disposed = false;

        ///<summary>
        /// Creates a new tracking container linked to the parent
        ///</summary>
        ///<param name="parent"></param>
        public TrackingContainer(IWindsorContainer parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Disposes the container and all resources instantiated by the container.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            using (_lock.WriteLock())
            {
                foreach (var obj in _tracked)
                    _parent.Release(obj);
                _tracked.Clear();
            }
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public object Build(Type typeToBuild)
        {
            var obj = _parent.Resolve(typeToBuild);
            using (_lock.WriteLock())
            {
                _tracked.Add(obj);
            }
            return obj;
        }

        public IContainer BuildChildContainer()
        {
            return new TrackingContainer(_parent);
        }

        public bool HasComponent(Type componentType)
        {
            return _parent.Kernel.HasComponent(componentType);                       
        }

        public void RegisterSingleton(Type lookupType, object instance)
        {
            throw new NotImplementedException();
        }

        public void ConfigureProperty(Type component, string property, object value)
        {
            throw new NotImplementedException();
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            var items = _parent.ResolveAll(typeToBuild);

            using (_lock.WriteLock())
            {
                foreach (var item in items)
                    _tracked.Add(item);
            }

            return items.Cast<object>();
        }
    }
}