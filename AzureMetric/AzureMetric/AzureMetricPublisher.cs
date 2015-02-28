using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorageExtensions;
using Base64Url;

namespace AzureMetric
{
    public class AzureMetricTicker<T, U> : BaseMetricTicker<T, U> where U : IMetric, new()
    {
        private readonly CloudClient client;
        private readonly SettingAttribute setting;
        public AzureMetricTicker(TimeSpan interval, string connectionName, string tableName, Period period = Period.NoPeriod, int removeAfter = 0) : base(interval)
        {
            client = CloudClient.Get(connectionName);
            setting = new SettingAttribute {Name = tableName, Period = period, RemoveAfter = removeAfter};
        }

        public override Task PublishAsync(IDictionary<T, U> data)
        {
            var table = client.GetGenericCloudTable<MetricItem<T, U>>(setting.Name, setting);
            var timeId = TimeId.NewSortableId(true);
            var list = from kvp in data
                       select new MetricItem<T, U>
                       {
                           PartitionKey = timeId,
                           RowKey = Base64.NewId(),
                           Dimensions = kvp.Key,
                           Metric = kvp.Value,
                       };
            table.BulkInsert(list, true);
            return Task.Delay(0);
        }
    }
}