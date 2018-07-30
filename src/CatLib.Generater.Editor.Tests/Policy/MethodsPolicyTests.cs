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

using System;
using CatLib.Generater.Editor.Context;
using CatLib.Generater.Editor.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        }

        [TestMethod]
        public void TestMethodsInterfacePolicy()
        {
            var policy = new MethodsPolicy();
            var context = new FacadeContext(null, typeof(ITestInterface));

            policy.Factory(context);
            Assert.AreEqual(3, context.Class.Members.Count);
        }
    }
}
