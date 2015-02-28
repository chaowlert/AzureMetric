using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureMetric
{
    public class MetricItem<T, U> : ITableEntity where U : IMetric, new()
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }

        public T Dimensions { get; set; }
        public U Metric { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            this.Dimensions = Activator.CreateInstance<T>();
            TableEntity.ReadUserObject(this.Dimensions, properties, operationContext);
            this.Metric = new U();
            TableEntity.ReadUserObject(this.Metric, properties, operationContext);
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var dict1 = TableEntity.WriteUserObject(this.Dimensions, operationContext);
            var dict2 = TableEntity.WriteUserObject(this.Metric, operationContext);
            foreach (var kvp in dict2)
            {
                dict1.Add(kvp.Key, kvp.Value);
            }
            return dict1;
        }
    }
}
