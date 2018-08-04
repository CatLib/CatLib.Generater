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
using CatLib.Generater.Editor.Policy.StaticWrap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CatLib.Generater.Editor.Tests.Policy
{
    [TestClass]
    public class MemberStaticWrapPolicyTests
    {
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
                Class = {Name = "TestHasDefaultValueFunction"}
            };

            policy.Factory(context);
            // Console.WriteLine(Util.GenerateFromType(context.Class));
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

        #region 测试带有事件的方法

        public interface ITestEvent
        {
            event Action ActionEventTest;
            event Action<ITestEvent> ActionGenericEventTest;
            event Func<int, Dictionary<string, ITestEvent>> FuncEventTest;
        }

        [TestMethod]
        public void TestEvent()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(ITestEvent))
            {
                Class = {Name = "TestEvent"}
            };

            policy.Factory(context);
             Console.WriteLine(Util.GenerateFromType(context.Class));

            Assert.AreEqual(
                @"public class TestEvent {
    
    public static event System.Action ActionEventTest {
        add {  
            Instance.ActionEventTest += value;
        }
        remove {
            Instance.ActionEventTest -= value;
        }
    }
    public static event System.Action<CatLib.Generater.Editor.Tests.Policy.MemberStaticWrapPolicyTests.ITestEvent> ActionGenericEventTest {
        add {  
            Instance.ActionGenericEventTest += value;
        }
        remove {
            Instance.ActionGenericEventTest -= value;
        }
    }
    public static event System.Func<int, System.Collections.Generic.Dictionary<string, CatLib.Generater.Editor.Tests.Policy.MemberStaticWrapPolicyTests.ITestEvent>> FuncEventTest {
        add {  
            Instance.FuncEventTest += value;
        }
        remove {
            Instance.FuncEventTest -= value;
        }
    }
}", Util.GenerateFromType(context.Class));
        }

        #endregion

        #region 测试属性选择器

        public interface ITestAttributeSelector
        {
            string StringData { get; set; }
            string StringDataOnlyGet { get; }
            string StringDataOnlySet { set; }

            List<string> ListData { get; set; }
            ITestAttributeSelector ClassData { set; }
            Dictionary<string, List<ITestAttributeSelector>> ComplexGenericTypeOnlyGet { get; }
            Dictionary<string, List<ITestAttributeSelector>> ComplexGenericTypeOnlySet { set; }
        }

        [TestMethod]
        public void TestAttributeSelector()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(ITestAttributeSelector))
            {
                Class = {Name = "TestAttributeSelector"}
            };

            policy.Factory(context);
            // Console.WriteLine(Util.GenerateFromType(context.Class));

            Assert.AreEqual(
                @"public class TestAttributeSelector {
    
    public static string StringData {
        get {
            return Instance.StringData;
        }
        set {
            Instance.StringData = value;
        }
    }
    
    public static string StringDataOnlyGet {
        get {
            return Instance.StringDataOnlyGet;
        }
    }
    
    public static string StringDataOnlySet {
        set {
            Instance.StringDataOnlySet = value;
        }
    }
    
    public static System.Collections.Generic.List<string> ListData {
        get {
            return Instance.ListData;
        }
        set {
            Instance.ListData = value;
        }
    }
    
    public static CatLib.Generater.Editor.Tests.Policy.MemberStaticWrapPolicyTests.ITestAttributeSelector ClassData {
        set {
            Instance.ClassData = value;
        }
    }
    
    public static System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<CatLib.Generater.Editor.Tests.Policy.MemberStaticWrapPolicyTests.ITestAttributeSelector>> ComplexGenericTypeOnlyGet {
        get {
            return Instance.ComplexGenericTypeOnlyGet;
        }
    }
    
    public static System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<CatLib.Generater.Editor.Tests.Policy.MemberStaticWrapPolicyTests.ITestAttributeSelector>> ComplexGenericTypeOnlySet {
        set {
            Instance.ComplexGenericTypeOnlySet = value;
        }
    }
}", Util.GenerateFromType(context.Class));
        }

        #endregion

        #region 测试接口继承含有模版方法的接口
        public interface ITestHasGenericInterfaceParent<T>
        {
            T Say(T value, T def = default(T));
        }

        public interface ITestHasGenericInterface : ITestHasGenericInterfaceParent<ITestHasGenericInterface>
        {
            
        }

        [TestMethod]
        public void TestHasGenericInterface()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(ITestHasGenericInterface))
            {
                Class = { Name = "TestHasGenericInterface" }
            };

            policy.Factory(context);
            //Console.WriteLine(Util.GenerateFromType(context.Class));

            Assert.AreEqual(
@"public class TestHasGenericInterface {
    
    public static CatLib.Generater.Editor.Tests.Policy.MemberStaticWrapPolicyTests.ITestHasGenericInterface Say(CatLib.Generater.Editor.Tests.Policy.MemberStaticWrapPolicyTests.ITestHasGenericInterface value, CatLib.Generater.Editor.Tests.Policy.MemberStaticWrapPolicyTests.ITestHasGenericInterface def = null) {
        return Instance.Say(value, def);
    }
}", Util.GenerateFromType(context.Class));

        }
        #endregion

        #region 重载函数测试
        public interface ITestOverloadMethod_v0_0
        {
            void TestFunction(int a);
            void TestFunction2(int a);
        }

        public interface ITestOverloadMethod_v0_1 : ITestOverloadMethod_v0_0
        {
            new void TestFunction(int a);
            void TestFunction2(double a = 100);
            Action TestFunctionRef(int a, ref double b);
        }

        public interface ITestOverloadMethod_v1_0
        {
            void TestFunction(int a);
            void TestFunction2(double a, int b = 100);
            void TestFunctionRef(int a, ref double b);
        }

        public interface ITestOverloadMethod : ITestOverloadMethod_v0_1, ITestOverloadMethod_v1_0
        {
            void TestFunction();
            new void TestFunction2(int a = 100);
            new event Action TestFunctionRef;
        }

        [TestMethod]
        public void TestOverloadMethod()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(ITestOverloadMethod))
            {
                Class = { Name = "TestOverloadMethod" }
            };

            policy.Factory(context);
            // Console.WriteLine(Util.GenerateFromType(context.Class));

            Assert.AreEqual(
@"public class TestOverloadMethod {
    
    public static event System.Action TestFunctionRef {
        add {  
            Instance.TestFunctionRef += value;
        }
        remove {
            Instance.TestFunctionRef -= value;
        }
    }
    
    public static void TestFunction() {
        Instance.TestFunction();
    }
    
    public static void TestFunction2(int a = 100) {
        Instance.TestFunction2(a);
    }
    
    public static void TestFunction(int a) {
        Instance.TestFunction(a);
    }
    
    public static void TestFunction2(double a = 100d) {
        Instance.TestFunction2(a);
    }
    
    public static void TestFunction2(double a, int b = 100) {
        Instance.TestFunction2(a, b);
    }
}", Util.GenerateFromType(context.Class));
        }
        #endregion

        #region 重载函数二义性检查（返回值）

        public interface IMethodReturnCallTwoSense_0_0
        {
            int TestFunction();
        }

        public interface IMethodReturnCallTwoSense_0_1 : IMethodReturnCallTwoSense_0_0
        {
            int TestFunction(float num);
        }

        public interface IMethodReturnCallTwoSense_1_0
        {
            void TestFunction(float num);
        }

        public interface IMethodReturnCallTwoSense : IMethodReturnCallTwoSense_0_1 , IMethodReturnCallTwoSense_1_0
        {
            
        }

        [TestMethod]
        public void MethodReturnCallTwoSense()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(IMethodReturnCallTwoSense))
            {
                Class = { Name = "MethodReturnCallTwoSense" }
            };

            try
            {
                policy.Factory(context);
            }
            catch (GenerateException)
            {
                return;
            }

            Assert.Fail();
        }
        #endregion

        #region 重载函数二义性检查（参数方向）
        public interface IMethodDirectionCallTwoSense_0_0
        {
            int TestFunction(out int a);
        }

        public interface IMethodDirectionCallTwoSense_0_1 : IMethodDirectionCallTwoSense_0_0
        {
            int TestFunction(out int a);
        }

        public interface IMethodDirectionCallTwoSense_1_0
        {
            int TestFunction(ref int a);
        }

        public interface IMethodDirectionCallTwoSense : IMethodDirectionCallTwoSense_0_1, IMethodDirectionCallTwoSense_1_0
        {

        }

        [TestMethod]
        public void MethodDirectionCallTwoSense()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(IMethodDirectionCallTwoSense))
            {
                Class = { Name = "MethodDirectionCallTwoSense" }
            };

            try
            {
                policy.Factory(context);
            }
            catch (GenerateException)
            {
                return;
            }

            Assert.Fail();
        }
        #endregion

        #region 覆盖继承函数
        public interface IOverrideMethod_0_0
        {
            int TestFunction(out int a);
            int TestFunction2(out int a);
        }

        public interface IOverrideMethod_0_1 : IOverrideMethod_0_0
        {
            new int TestFunction(out int a);
        }

        public interface IOverrideMethod_1_0
        {
            int TestFunction(ref int a);
            int TestFunction2(out int a, out int b);
        }

        public interface IOverrideMethod : IOverrideMethod_0_1, IOverrideMethod_1_0
        {
            new event Action TestFunction ;
            new float TestFunction2 { get; }
        }

        [TestMethod]
        public void OverrideMethod()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(IOverrideMethod))
            {
                Class = { Name = "OverrideMethod" }
            };

            policy.Factory(context);

            Console.WriteLine(Util.GenerateFromType(context.Class));

            Assert.AreEqual(
@"public class OverrideMethod {
    
    public static event System.Action TestFunction {
        add {  
            Instance.TestFunction += value;
        }
        remove {
            Instance.TestFunction -= value;
        }
    }
    
    public static float TestFunction2 {
        get {
            return Instance.TestFunction2;
        }
    }
}", Util.GenerateFromType(context.Class));
        }
        #endregion

        #region 混合覆盖
        public interface IMixedOverride_0_0
        {
            event Action TestFunction;
            Action TestFunction2 { get; }
            float TestFunction3(int a = 100);
        }

        public interface IMixedOverride_0_1 : IMixedOverride_0_0
        {
        }

        public interface IMixedOverride_1_0
        {
            Action TestFunction { get; }
            float TestFunction2(int a = 100);
            event Action TestFunction3;
        }

        public interface IMixedOverride : IMixedOverride_0_1, IMixedOverride_1_0
        {
            new float TestFunction(int a = 100);
            new event Action<MemberStaticWrapPolicy> TestFunction2;
            new Action TestFunction3 { get; set; }
        }

        [TestMethod]
        public void MixedOverride()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(IMixedOverride))
            {
                Class = { Name = "MixedOverride" }
            };

            policy.Factory(context);

            Console.WriteLine(Util.GenerateFromType(context.Class));

            Assert.AreEqual(@"public class MixedOverride {
    
    public static event System.Action<CatLib.Generater.Editor.Policy.StaticWrap.MemberStaticWrapPolicy> TestFunction2 {
        add {  
            Instance.TestFunction2 += value;
        }
        remove {
            Instance.TestFunction2 -= value;
        }
    }
    
    public static System.Action TestFunction3 {
        get {
            return Instance.TestFunction3;
        }
        set {
            Instance.TestFunction3 = value;
        }
    }
    
    public static float TestFunction(int a = 100) {
        return Instance.TestFunction(a);
    }
}", Util.GenerateFromType(context.Class));
        }
        #endregion

        #region 对于 this[] 的特殊字段进行剥离测试
        public interface ITestThisProperty_0_0
        {
            int this[uint b] { get; set; }
        }

        public interface ITestThisProperty: ITestThisProperty_0_0
        {
            int this[int index = 1, float b = 10f] { get; set; }
            int this[float b] { get; set; }
            void TestFunction();
        }
        
        [TestMethod]
        public void TestThisProperty()
        {
            var policy = new MemberStaticWrapPolicy();
            var context = new FacadeContext(null, typeof(ITestThisProperty))
            {
                Class = { Name = "TestThisProperty" }
            };

            policy.Factory(context);

            // Console.WriteLine(Util.GenerateFromType(context.Class));

            Assert.AreEqual(
@"public class TestThisProperty {
    
    public static void TestFunction() {
        Instance.TestFunction();
    }
}", Util.GenerateFromType(context.Class));
        }
        #endregion
    }
}
