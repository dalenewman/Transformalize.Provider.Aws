using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;
using Amazon.Connect;
using Amazon.Connect.Model;

namespace Transformalize.Providers.Amazon.Connect {
   public class InstancesReader : IRead {

      private readonly InputContext _context;
      private readonly IRowFactory _rowFactory;
      private readonly AmazonConnectClient _client;
      private readonly ListInstancesRequest _request;
      private bool _run = true;

      public InstancesReader(InputContext context, IRowFactory rowFactory) {
         _context = context;
         _rowFactory = rowFactory;
         _client = new AmazonConnectClient();
         CheckFieldTypes();
         _request = new ListInstancesRequest();
      }

      public IEnumerable<IRow> Read() {

         if (!_run) {
            yield break;
         }

         do {
            var response = _client.ListInstancesAsync(_request).Result;
            foreach (var inst in response.InstanceSummaryList) {
               var row = _rowFactory.Create();

               foreach (var field in _context.InputFields) {
                  var name = field.Name.ToLower();
                  switch (name) {
                     case "arn":
                        row[field] = inst.Arn ?? string.Empty;
                        break;
                     case "createdtime":
                        row[field] = inst.CreatedTime;
                        break;
                     case "id":
                        row[field] = inst.Id;
                        break;
                     case "identitymanagementtype":
                        row[field] = inst.IdentityManagementType.Value ?? string.Empty;
                        break;
                     case "inboundcallsenabled":
                        row[field] = inst.InboundCallsEnabled;
                        break;
                     case "instancealias":
                        row[field] = inst.InstanceAlias ?? string.Empty;
                        break;
                     case "instancestatus":
                        row[field] = inst.InstanceStatus.Value ?? string.Empty;
                        break;
                     case "outboundcallsenabled":
                        row[field] = inst.OutboundCallsEnabled;
                        break;
                     case "servicerole":
                        row[field] = inst.ServiceRole ?? string.Empty;
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
               case "createdtime":  // type datetime
                  if (field.Type != "datetime") {
                     _context.Error("createdtime must be a datetime.");
                     _run = false;
                  }
                  break;
               case "id":
                  if (field.Type != "string") {
                     _context.Error("id must be a string.");
                     _run = false;
                  }
                  break;
               case "identitymanagementtype":
                  if (field.Type != "string") {
                     _context.Error("identitymanagementtype must be a string.");
                     _run = false;
                  }
                  break;
               case "inboundcallsenabled":  // bool
                  if (field.Type != "bool") {
                     _context.Error("inboundcallsenabled must be a bool.");
                     _run = false;
                  }
                  break;
               case "outboundcallsenabled":  // bool
                  if (field.Type != "bool") {
                     _context.Error("outboundcallsenabled must be a bool.");
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
               case "instancealias":
                  if (field.Type != "string") {
                     _context.Error("instancealias must be a string.");
                     _run = false;
                  }
                  break;
               case "instancestatus":
                  if (field.Type != "string") {
                     _context.Error("instancestatus must be a string.");
                     _run = false;
                  }
                  break;
               case "servicerole":
                  if (field.Type != "string") {
                     _context.Error("servicerole must be a string.");
                     _run = false;
                  }
                  break;

               default:
                  break;
            }
         }
      }
     
   }
}
