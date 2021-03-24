using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Aws.CloudWatch {
   public class LogGroupsSchemaReader : ISchemaReader {

      private readonly IConnectionContext _context;

      public LogGroupsSchemaReader(IConnectionContext context) {
         _context = context;
      }

      public Schema Read() {
         var schema = new Schema {
            Connection = _context.Connection,
            Entities = new List<Entity>()
         };

         foreach (var entity in _context.Process.Entities.Where(e => e.Input == _context.Connection.Name)) {
            schema.Entities.Add(AddFields(entity));
         }

         return schema;
      }

      public Schema Read(Entity entity) {
         var schema = new Schema {
            Connection = _context.Connection,
            Entities = new List<Entity>()
         };

         schema.Entities.Add(AddFields(entity));

         return schema;
      }

      private Entity AddFields(Entity entity) {

         entity.Fields = new List<Field> {
            new Field { Name = "arn", PrimaryKey = true },
            new Field { Name = "creationTime", Type = "datetime" },
            new Field { Name = "kmsKeyId", Length = "256" },
            new Field { Name = "logGroupName", Length = "512" },
            new Field { Name = "metricFilterCount", Type = "int" },
            new Field { Name = "retentionInDays", Type = "int" },
            new Field { Name = "storedBytes", Type = "long" }
         };

         entity.Load();

         return entity;
      }
   }
}
