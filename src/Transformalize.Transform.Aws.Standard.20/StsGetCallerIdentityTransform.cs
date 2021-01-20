#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Aws {

   public class StsGetCallerIdentityTransform : BaseTransform {

      private readonly Func<object, string> _serializer;
      private string _answer;
      public StsGetCallerIdentityTransform(IContext context = null, Func<object, string> serializer = null) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }

         if (serializer == null) {
            Error($"The {Context.Operation.Method} transform requires a JSON serializer.");
            Run = false;
            return;
         }

         _serializer = serializer;

      }

      public override IRow Operate(IRow row) {
         row[Context.Field] = GetCallerIdentity();
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[] { new OperationSignature("stsgetcalleridentity") };
      }

      public string GetCallerIdentity() {
         if (_answer != null) {
            return _answer;
         }

         if (Run) {
            var client = new Amazon.SecurityToken.AmazonSecurityTokenServiceClient();
            var task = client.GetCallerIdentityAsync(new Amazon.SecurityToken.Model.GetCallerIdentityRequest());
            if (task.IsFaulted) {
               Error(task.Exception.Flatten().Message);
               Error("You must have assumed a role first.");
               Run = false;
               return string.Empty;
            } else {
               _answer = _serializer(task.Result);
               return _answer;
            }
         } else {
            Error("Unable to run stsgetcalleridentity.  Missing context and/or serializer.");
            return string.Empty;
         }
        
      }
   }
}