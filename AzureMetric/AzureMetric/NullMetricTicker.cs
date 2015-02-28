using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureMetric
{
    public class NullMetricTicker<T, U> : BaseMetricTicker<T, U> where U : IMetric, new()
    {
        public NullMetricTicker() : base(Timeout.InfiniteTimeSpan) { }

        public override Task PublishAsync(IDictionary<T, U> data)
        {
            return Task.Delay(0);
        }

        public override void Tick(T key, int value = 1) { }
    }
}
