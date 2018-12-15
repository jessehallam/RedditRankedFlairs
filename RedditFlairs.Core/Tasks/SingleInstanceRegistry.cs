using System;
using System.Collections.Generic;
using System.Threading;

namespace RedditFlairs.Core.Tasks
{
    internal class SingleInstanceRegistry
    {
        public static readonly SingleInstanceRegistry Instance = new SingleInstanceRegistry();

        private readonly Dictionary<Type, Semaphore> exclusiveLockDictionary = new Dictionary<Type, Semaphore>();

        private SingleInstanceRegistry() { }

        public Semaphore GetExclusiveLock(Type key)
        {
            lock (exclusiveLockDictionary)
            {
                if (!exclusiveLockDictionary.TryGetValue(key, out var locker))
                {
                    locker = new Semaphore(1, 1);
                    exclusiveLockDictionary.Add(key, locker);
                }

                return locker;
            }
        }
    }
}