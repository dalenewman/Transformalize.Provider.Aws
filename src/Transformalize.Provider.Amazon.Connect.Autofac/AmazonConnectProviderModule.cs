using Autofac;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;

namespace Transformalize.Providers.Amazon.Connect.Autofac {
   public class AmazonConnectProviderModule : Module {

      private Process _process = null;
      public const string ProviderName = "aws";
      public const string ProviderServiceName = "connect";

      public AmazonConnectProviderModule() {
      }

      public AmazonConnectProviderModule(Process process) {
         _process = process;
      }

      protected override void Load(ContainerBuilder builder) {

         if (_process == null) {
            if (!builder.Properties.ContainsKey("Process")) {
               return;
            }
            _process = (Process)builder.Properties["Process"];
         }

         // Schema Reader
         foreach (var connection in _process.Connections.Where(c => c.Provider == ProviderName && c.Service == ProviderServiceName)) {
            builder.Register<ISchemaReader>(ctx => new ConnectSchemaReader(ctx.ResolveNamed<IConnectionContext>(connection.Key))).Named<ISchemaReader>(connection.Key);
         }

         // Entity input
         foreach (var entity in _process.Entities) {

            var connection = _process.Connections.FirstOrDefault(c => c.Name == entity.Input);

            if (connection != null && connection.Provider == ProviderName && connection.Service == ProviderServiceName) {

               // input version detector
               builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

               // input reader
               switch (connection.Command.ToLower().Replace("-",string.Empty)) {
                  case "listinstances":
                     builder.Register<IRead>(ctx => {
                        var input = ctx.ResolveNamed<InputContext>(entity.Key);
                        var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
                        return new InstancesReader(input, rowFactory);
                     }).Named<IRead>(entity.Key);
                     break;
                  case "listusers":
                     builder.Register<IRead>(ctx => {
                        var input = ctx.ResolveNamed<InputContext>(entity.Key);
                        var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
                        return new UsersReader(input, rowFactory);
                     }).Named<IRead>(entity.Key);
                     break;
                  default:
                     builder.Register<IRead>(ctx => {
                        var input = ctx.ResolveNamed<InputContext>(entity.Key);
                        input.Error($"No handler for aws {connection.Service} {connection.Command}");
                        return new NullReader(input);
                     }).Named<IRead>(entity.Key);
                     break;
               }
            }
         }
      }
   }
}
