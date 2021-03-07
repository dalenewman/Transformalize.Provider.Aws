using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Aws.CloudWatch {
   public class LogGroupsSchemaReader : ISchemaReader {

      private readonly IConnectionContext _context;

      public LogGroupsSchemaReader(IConnectionContext context) {
         _context = context;
      }

      public Schema Read() {
         return Common();
      }

      public Schema Read(Entity entity) {
         return Common();
      }

      private Schema Common() {

         var schema = new Schema {
            Connection = _context.Connection
         };

         var entity = new Entity { Name = "Schema", Input = _context.Connection.Name };
         entity.Fields = new List<Field> {
            new Field { Name = "arn", PrimaryKey = true },
            new Field { Name = "creationTime", Type = "datetime" },
            new Field { Name = "kmsKeyId", Length = "256" },
            new Field { Name = "logGroupName", Length = "512" },
            new Field { Name = "metricFilterCount", Type = "int" },
            new Field { Name = "retentionInDays", Type = "int" },
            new Field { Name = "storedBytes", Type = "long" }
         };

         return schema;
      }
   }
}
