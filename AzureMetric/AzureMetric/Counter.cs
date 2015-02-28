using System.Threading;

namespace AzureMetric
{
    public class Counter : IMetric
    {
        private int count;
        public int Count
        {
            get { return Thread.VolatileRead(ref count); }
            set { Thread.VolatileWrite(ref count, value); }
        }

        public void AddValue(int value)
        {
            Interlocked.Add(ref this.count, value);
        }
    }
}