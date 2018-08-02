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
    public class MemberStaticWrapPolicyTests
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
            var classPolicy = new ClassCreatePolicy();
            classPolicy.Factory(context);
            policy.Factory(context);

            Console.WriteLine(Util.Generate(context.CompileUnit));
            //Assert.AreEqual(3, context.Class.Members.Count);
        }

        #region 测试正常的方法
        public interface ITestNormalFunctionWrap
        {
            void VoidReturn();

            int IntReturn();

            int IntReturnAndHasParams(string name);
        }

        [TestMethod]
        public void TestNormalFunctionWrap()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(ITestNormalFunctionWrap))
            {
                Class = {Name = "TestNormalFunctionWrap"}
            };

            policy.Factory(context);

            Assert.AreEqual(3, context.Class.Members.Count);
            Assert.AreEqual("VoidReturn", context.Class.Members[0].Name);
            Assert.AreEqual("IntReturn", context.Class.Members[1].Name);
            Assert.AreEqual("IntReturnAndHasParams", context.Class.Members[2].Name);

            Assert.AreEqual(
@"public static void VoidReturn() {
    Instance.VoidReturn();
}", Util.GenerateFromMember(context.Class.Members[0]));

            Assert.AreEqual(
@"public static int IntReturn() {
    return Instance.IntReturn();
}", Util.GenerateFromMember(context.Class.Members[1]));

            Assert.AreEqual(
@"public static int IntReturnAndHasParams(string name) {
    return Instance.IntReturnAndHasParams(name);
}", Util.GenerateFromMember(context.Class.Members[2]));
        }
        #endregion

        #region 测试含有默认值的方法
        public interface ITestHasDefaultValueFunction
        {
            void HasIntDefaultValue(int num = 100);
            void HasFloatDefaultValue(float num = 100.123f, float num2 = 200);
            void HasBooleanDefaultValue(bool v1 = true, bool v2 = false);
            void HasDoubleDefaultValue(double num = 123.123456789d, double num2 = 123, double num3 = 123.1234567890f);
            void HasUintDefaultValue(uint num = 122);
            void HasStringDefaultValue(string str = "helloworld");
            void HasStringNullDefaultValue(string str = null);
            void HasClassDefaultValue(ITestHasDefaultValueFunction cls = null);
            void TestGenericTypeDefaultValue(Dictionary<string, ITestHasDefaultValueFunction> generic = null);
        }

        [TestMethod]
        public void TestHasDefaultValueFunction()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(ITestHasDefaultValueFunction))
            {
                Class = { Name = "TestHasDefaultValueFunction" }
            };

            policy.Factory(context);
            Console.WriteLine(Util.GenerateFromType(context.Class));
            Assert.AreEqual(
@"public class TestHasDefaultValueFunction {
    
    public static void HasIntDefaultValue(int num = 100) {
        Instance.HasIntDefaultValue(num);
    }
    
    public static void HasFloatDefaultValue(float num = 100.123f, float num2 = 200f) {
        Instance.HasFloatDefaultValue(num, num2);
    }
    
    public static void HasBooleanDefaultValue(bool v1 = true, bool v2 = false) {
        Instance.HasBooleanDefaultValue(v1, v2);
    }
    
    public static void HasDoubleDefaultValue(double num = 123.123456789d, double num2 = 123d, double num3 = 123.123458862305d) {
        Instance.HasDoubleDefaultValue(num, num2, num3);
    }
    
    public static void HasUintDefaultValue(uint num = 122) {
        Instance.HasUintDefaultValue(num);
    }
    
    public static void HasStringDefaultValue(string str = ""helloworld"") {
        Instance.HasStringDefaultValue(str);
    }
    
    public static void HasStringNullDefaultValue(string str = null) {
        Instance.HasStringNullDefaultValue(str);
    }
    
    public static void HasClassDefaultValue(CatLib.Generater.Editor.Tests.Policy.MemberStaticWrapPolicyTests.ITestHasDefaultValueFunction cls = null) {
        Instance.HasClassDefaultValue(cls);
    }
    
    public static void TestGenericTypeDefaultValue(System.Collections.Generic.Dictionary<string, CatLib.Generater.Editor.Tests.Policy.MemberStaticWrapPolicyTests.ITestHasDefaultValueFunction> generic = null) {
        Instance.TestGenericTypeDefaultValue(generic);
    }
}", Util.GenerateFromType(context.Class));
        }
        #endregion
    }
}
