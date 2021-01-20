using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Transforms.Aws.Autofac;

namespace Test.Unit {

   /// <summary>
   /// In order for this test to pass, you have to assume a role prior to starting
   /// Visual Studio.
   /// </summary>
   [TestClass]
   public class Test {
      [TestMethod]
      public void TestMethod1() {
         var cfg = @"<cfg name='test'>
   <entities>
      <add name='entity'>
         <rows>
            <add key='1' />
         </rows>
         <fields>
            <add name='key' type='int' primary-key='true' />
         </fields>
         <calculated-fields>
            <add name='calleridentity' t='stsgetcalleridentity()' length='255' />
         </calculated-fields>
      </add>
   </entities>
</cfg>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer(new AwsTransformModule()).CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new AwsTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               IRow[] rows = controller.Read().ToArray();

               Assert.AreEqual(1, rows.Length);
               var value = rows[0][process.Entities[0].CalculatedFields[0]].ToString();
               Assert.AreNotEqual(string.Empty, value);
               Assert.AreEqual("{\"Account\"", value.Substring(0, 10));

            }
         }
      }
   }
}