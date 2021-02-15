using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Aws.CloudWatch.Autofac;
using Transformalize.Providers.Console;

namespace Test.Unit {

   /// <summary>
   /// In order for this test to pass, you have to assume a role prior to starting
   /// Visual Studio.
   /// </summary>
   [TestClass]
   public class CloudWatchDescribeLogGroups {
      [TestMethod]
      public void TestMethod1() {
         var cfg = @"<cfg name='test'>
   <connections>
      <add name='input' provider='aws' service='logs' command='DescribeLogGroups' />
   </connections>
   <entities>
      <add name='entity'>
         <fields>
            <add name='arn' primary-key='true' />
            <add name='creationTime' type='datetime' />
            <add name='kmsKeyId' length='256' />
            <add name='logGroupName' length='512' />
            <add name='metricFilterCount' type='int' />
            <add name='retentionInDays' type='int' />
            <add name='storedBytes' type='long' />
         </fields>
      </add>
   </entities>
</cfg>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new AwsCloudWatchProviderModule(process)).CreateScope(process, logger)) {
               
               var controller = inner.Resolve<IProcessController>();
               IRow[] rows = controller.Read().ToArray();

               Assert.AreNotEqual(0, rows.Count());
            }
         }
      }
   }
}