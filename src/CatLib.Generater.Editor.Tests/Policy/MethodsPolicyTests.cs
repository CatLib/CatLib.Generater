/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using CatLib.Generater.Editor.Context;
using CatLib.Generater.Editor.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CatLib.Generater.Editor.Tests.Policy
{
    [TestClass]
    public class MethodsPolicyTests
    {
        public interface ITestInterfaceParent
        {
            int Hello();
        }

        public interface ITestInterface : ITestInterfaceParent, IDisposable
        {
            void Say();

            event Action TestEvent;

            int add_TestEvent();

            int TestAttribute { get; set; }
        }

        private event Action bb;


        [TestMethod]
        public void TestMethodsInterfacePolicy()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(ITestInterface));
            context.Class.Name = "UnitTest";
            policy.Factory(context);

            Console.WriteLine(Util.Generate(context.CompileUnit));
            //Assert.AreEqual(3, context.Class.Members.Count);
        }
    }
}
