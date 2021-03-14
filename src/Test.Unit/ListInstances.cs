using System;
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Amazon.Connect.Autofac;
using Transformalize.Providers.Console;

namespace Test.Unit {

   /// <summary>
   /// In order for this test to pass, you have to assume a 
   /// role prior to starting Visual Studio that has 
   /// at least one Amazon Connect instance provisioned
   /// </summary>
   [TestClass]
   public class ListInstances {

      [TestMethod]
      public void InstancesExist() {
         var cfg = @"<cfg name='test'>
   <connections>
      <add name='input' provider='aws' service='connect' command='list-instances' />
   </connections>
   <entities>
      <add name='entity'>
         <fields>
            <add name='arn' primary-key='true' />
            <add name='CreatedTime' type='datetime' />
            <add name='Id' length='100' />
            <add name='IdentityManagementType' />
            <add name='InboundCallsEnabled' type='bool' />
            <add name='InstanceAlias' length='62' />
            <add name='InstanceStatus' />
            <add name='OutboundCallsEnabled' type='bool' />
            <add name='ServiceRole' length='512' />
         </fields>
      </add>
   </entities>
</cfg>";
         Environment.SetEnvironmentVariable("AWS_PROFILE", "vlad");

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new AmazonConnectProviderModule(process)).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               IRow[] rows = controller.Read().ToArray();

               Assert.AreNotEqual(0, rows.Count());
            }
         }
      }
   }
}