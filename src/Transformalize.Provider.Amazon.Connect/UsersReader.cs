using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;
using Amazon.Connect;
using Amazon.Connect.Model ;
using System.Linq;

namespace Transformalize.Providers.Amazon.Connect {

   /*
    * What's different?
    * 1. the request ListUsersRequest, ListInstancesRequest
    * 2. the method you call off the ListUsersAsync, ListInstancesAsync
    * 3. the response you get, ListUsersResponse, ListInstancesResponse
    * 4. the list, UserSummaryList => List<UserSummary>, InstanceSummaryList => List<InstanceSummary>
    * 5. the field name to property mapping (with default for type) used for type checking and capturing values
    */
   public class UsersReader : IRead {

      private readonly InputContext _context;
      private readonly IRowFactory _rowFactory;
      private readonly AmazonConnectClient _client;
      private readonly ListUsersRequest _request;
      private bool _run = true;

      public UsersReader(InputContext context, IRowFactory rowFactory) {
         _context = context;
         _rowFactory = rowFactory;
         _client = new AmazonConnectClient();
         CheckFieldTypes();

         var instanceId = context.Process.Parameters.FirstOrDefault(p => p.Name.ToLower().Replace("-",string.Empty) == "instanceid");

         if(instanceId == null) {
            _run = false;
            context.Error("The aws connect list-instances provider needs an instance-id parameter.");
         } else {
            _request = new ListUsersRequest() { InstanceId = instanceId.Value };
         }
         
      }

      public IEnumerable<IRow> Read() {

         if (!_run) {
            yield break;
         }

         do {
            var response = _client.ListUsersAsync(_request).Result;
            foreach (var inst in response.UserSummaryList) {
               var row = _rowFactory.Create();

               foreach (var field in _context.InputFields) {
                  var name = field.Name.ToLower();
                  switch (name) {
                     case "arn":
                        row[field] = inst.Arn ?? string.Empty;
                        break;
                     case "id":
                        row[field] = inst.Id ?? string.Empty;
                        break;
                     case "username":
                        row[field] = inst.Username ?? string.Empty;
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
               case "id":
                  if (field.Type != "string") {
                     _context.Error("id must be a string.");
                     _run = false;
                  }
                  break;
               case "username":
                  if (field.Type != "string") {
                     _context.Error("username must be a string.");
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
