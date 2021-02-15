using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using System.Linq;

namespace Transformalize.Providers.Aws.CloudWatch {
   public class LogGroupsReader : IRead {

      private readonly InputContext _context;
      private readonly IRowFactory _rowFactory;
      private readonly AmazonCloudWatchLogsClient _client;
      private readonly DescribeLogGroupsRequest _request;
      private bool _run = true;

      public LogGroupsReader(InputContext context, IRowFactory rowFactory) {
         _context = context;
         _rowFactory = rowFactory;
         _client = new AmazonCloudWatchLogsClient();
         CheckFieldTypes();
         _request = new DescribeLogGroupsRequest() { Limit = GetLimit(), LogGroupNamePrefix = GetPrefix() };
      }

      public IEnumerable<IRow> Read() {

         if (!_run) {
            yield break;
         }

         do {
            var response = _client.DescribeLogGroupsAsync().Result;
            foreach (var logGroup in response.LogGroups) {
               var row = _rowFactory.Create();

               foreach (var field in _context.InputFields) {
                  var name = field.Name.ToLower();
                  switch (name) {
                     case "arn":
                        row[field] = logGroup.Arn ?? string.Empty;
                        break;
                     case "creationtime":
                        row[field] = logGroup.CreationTime;
                        break;
                     case "kmskeyid":
                        row[field] = logGroup.KmsKeyId ?? string.Empty;
                        break;
                     case "loggroupname":
                        row[field] = logGroup.LogGroupName ?? string.Empty;
                        break;
                     case "metricfiltercount":
                        row[field] = logGroup.MetricFilterCount;
                        break;
                     case "retentionindays":
                        row[field] = logGroup.RetentionInDays.GetValueOrDefault();
                        break;
                     case "storedbytes":
                        row[field] = logGroup.StoredBytes;
                        break;
                     default:
                        break;
                  }
               }

               yield return row;
            }
            _request.NextToken = response.NextToken;
         } while (!string.IsNullOrEmpty(_request.NextToken));
      }

      private void CheckFieldTypes() {
         foreach (var field in _context.InputFields) {
            var name = field.Name.ToLower();
            switch (name) {
               case "arn":
                  if (field.Type != "string") {
                     _context.Error("arn must be a string.");
                     _run = false;
                  }
                  break;
               case "creationtime":  // type datetime
                  if (field.Type != "datetime") {
                     _context.Error("creationtime must be a datetime.");
                     _run = false;
                  }
                  break;
               case "kmskeyid":
                  if (field.Type != "string") {
                     _context.Error("kmskeyid must be a string.");
                     _run = false;
                  }
                  break;
               case "loggroupname":
                  if (field.Type != "string") {
                     _context.Error("loggroupname must be a string.");
                     _run = false;
                  }
                  break;
               case "metricfiltercount":  // type int
                  if (field.Type != "int") {
                     _context.Error("loggroupname must be an int.");
                     _run = false;
                  }
                  break;
               case "retentionindays":  // type int
                  if (field.Type != "int") {
                     _context.Error("retentionindays must be an int.");
                     _run = false;
                  }
                  break;
               case "storedbytes":  // type long
                  if (field.Type != "long") {
                     _context.Error("storedbytes must be a long.");
                     _run = false;
                  }
                  break;
               default:
                  break;
            }
         }
      }

      private string GetPrefix() {
         if (_context.Entity.Filter.Any()) {
            if (_context.Entity.Filter.Count > 1) {
               _context.Warn("Only one filter expression allowed for prefix.");
            }
            var expression = _context.Entity.Filter[0].Expression;
            if (expression.Length > 512) {
               _context.Warn("The prefix can not exceed 512 characters.");
            } else if (expression != string.Empty) {
               return expression;
            }
         }
         return null;
      }

      private int GetLimit() {
         if (_context.Entity.ReadSize > 0) {
            if (_context.Entity.ReadSize > 50) {
               _context.Warn("Maximum page size is 50.");
               _context.Entity.ReadSize = 50;
            }
            return _context.Entity.ReadSize;
         }

         return 50;
      }
      
   }
}
