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
using System.Collections.Generic;

namespace CatLib.Generater.Editor.Tests.Policy
{
    [TestClass]
    public class MethodsPolicyTests
    {
        public interface ITestInterfaceParent
        {
            int Hello();
        }

        public const string ddd = "123";

        public interface ITestInterface<T> : ITestInterfaceParent, IDisposable
        {
            void Say();

            event Action<int> TestEvent;

            T Hello();

            List<string> dddp();

            int add_TestEvent();

            void TestVoid(int a, out Dictionary<string, int> b, ref int c, params int[] d);

            void TestOptional(int a = 100, float b = 12.2f, string c = ddd, string dd = null);

            int TestAttribute { get; set; }
        }

        private event Action bb;


        [TestMethod]
        public void TestMethodsInterfacePolicy()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(ITestInterface<int>));
            context.Class.Name = "UnitTest";
            policy.Factory(context);

            Console.WriteLine(Util.Generate(context.CompileUnit));
            //Assert.AreEqual(3, context.Class.Members.Count);
        }
    }
}
