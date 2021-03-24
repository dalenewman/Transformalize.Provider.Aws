using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Amazon.Connect {
   public class InstancesSchemaReader : ISchemaReader {

      private readonly IConnectionContext _context;

      public InstancesSchemaReader(IConnectionContext context) {
         _context = context;
      }

      public Schema Read() {
         var schema = new Schema {
            Connection = _context.Connection,
            Entities = new List<Entity>()
         };
         foreach (var entity in _context.Process.Entities.Where(e=> e.Input == _context.Connection.Name)) {
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

      public Entity AddFields(Entity entity) {

         entity.Fields = new List<Field> {
            new Field { Name = "Arn", PrimaryKey = true },
            new Field { Name = "CreatedTime", Type = "datetime" },
            new Field { Name = "Id", Length = "100" },
            new Field { Name = "IdentityManagementType" },
            new Field { Name = "InboundCallsEnabled", Type="bool" },
            new Field { Name = "InstanceAlias", Length="62" },
            new Field { Name = "InstanceStatus" },
            new Field { Name = "OutboundCallsEnabled", Type = "bool" },
            new Field { Name = "ServiceRole", Length ="512" } //an ARN
         };

         entity.Load();

         return entity;
      }

   }
}
