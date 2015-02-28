using System.Threading;

namespace AzureMetric
{
    public class Metric : IMetric
    {
        private int max = int.MinValue;
        public int Max
        {
            get { return Thread.VolatileRead(ref max); }
            set { Thread.VolatileWrite(ref max, value); }
        }

        private int min = int.MaxValue;
        public int Min
        {
            get { return Thread.VolatileRead(ref min); }
            set { Thread.VolatileWrite(ref min, value); }
        }

        private int count;
        public int Count
        {
            get { return Thread.VolatileRead(ref count); }
            set { Thread.VolatileWrite(ref count, value); }
        }

        private int sum;
        public int Sum
        {
            get { return Thread.VolatileRead(ref sum); }
            set { Thread.VolatileWrite(ref sum, value); }
        }

        public void AddValue(int value)
        {
            for (int current = Thread.VolatileRead(ref this.max); current < value; current = Thread.VolatileRead(ref this.max))
            {
                var previous = Interlocked.CompareExchange(ref this.max, value, current);
                if (previous >= current)
                {
                    break;
                }
            }

            for (int current = Thread.VolatileRead(ref this.min); current > value; current = Thread.VolatileRead(ref this.min))
            {
                var previous = Interlocked.CompareExchange(ref this.min, value, current);
                if (previous <= current)
                {
                    break;
                }
            }

            Interlocked.Increment(ref this.count);
            Interlocked.Add(ref this.sum, value);
        }
    }
}