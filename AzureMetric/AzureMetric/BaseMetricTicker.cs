using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureMetric
{
    public interface IMetricTicker<in T>
    {
        bool IsWorking { get; }
        void Tick(T key, int value = 1);
    }
    public abstract class BaseMetricTicker<T, U> : IDisposable, IMetricTicker<T> where U : IMetric, new()
    {
        protected readonly Timer timer;
        volatile ConcurrentDictionary<T, U> stores = new ConcurrentDictionary<T, U>();
        protected BaseMetricTicker(TimeSpan interval)
        {
            if (interval != Timeout.InfiniteTimeSpan)
                this.timer = new Timer(Elapse, null, interval, interval);
        }

        public bool IsWorking
        {
            get { return timer != null; }
        }

        public abstract Task PublishAsync(IDictionary<T, U> data);

        internal void Elapse(object state)
        {
            this.FlushAsync().ConfigureAwait(false);
        }

        public virtual void Tick(T key, int value = 1)
        {
            var metric = this.stores.GetOrAdd(key, k => new U());
            metric.AddValue(value);
        }

        async Task FlushAsync()
        {
            var localStores = this.stores;
            if (localStores.Count == 0)
            {
                return;
            }
            this.stores = new ConcurrentDictionary<T, U>();

            //wait for pending tick method
            await Task.Delay(50);

            await this.PublishAsync(localStores);
        }

        public void Dispose()
        {
            //flush data before close
            this.FlushAsync().Wait();

            this.timer?.Dispose();
        }
    }
}