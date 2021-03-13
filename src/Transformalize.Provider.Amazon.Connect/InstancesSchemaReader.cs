using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Amazon.Connect {
   public class InstancesSchemaReader : ISchemaReader {

      private readonly IConnectionContext _context;

      public InstancesSchemaReader(IConnectionContext context) {
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
            Connection = _context.Connection, 
            Entities = new List<Entity>()
         };

         var entity = new Entity { Name = "Schema", Input = _context.Connection.Name };
         entity.Fields = new List<Field> {
            new Field { Name = "Arn", PrimaryKey = true },
            new Field { Name = "CreatedTime", Type = "datetime" },
            new Field { Name = "id", Length = "100" },
            new Field { Name = "IdentityManagementType" },
            new Field { Name = "InboundCallsEnabled", Type="bool" },
            new Field { Name = "InstanceAlias", Length="62" },
            new Field { Name = "InstanceStatus" },
            new Field { Name = "OutboundCallsEnabled", Type = "bool" },
            new Field { Name = "ServiceRole" }
         };

         schema.Entities.Add(entity);

         return schema;
      }
   }
}
