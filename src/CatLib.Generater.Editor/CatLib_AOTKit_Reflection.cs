using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 生成器端Reflection相关的快捷方法均在此定义
/// </summary>
public class CatLib_AOTKit_Reflection
{
    #region 反射取值
    /// <summary>
    /// 通过反射获得某个对象的 某个字段的 值
    /// </summary>
    public static T GetValue<T>(object target, string nameOfPropertyOrField)
    {
        T result = default(T);
        object resultObject = GetValue(target, nameOfPropertyOrField);
        if (resultObject != null)
        {
            result = (T)resultObject;
        }
        return result;
    }

    /// <summary>
    /// 通过反射获得某个对象的 某个字段的 值
    /// </summary>
    public static object GetValue(object target, string nameOfPropertyOrField)
    {
        object result = null;
        Type targetType = target.GetType();
        PropertyInfo property = targetType.GetProperty(nameOfPropertyOrField);
        if (property != null)
        {
            MethodInfo getMethod = property.GetGetMethod();
            object resultObject = getMethod.Invoke(target, null);
            result = resultObject;
        }

        FieldInfo field = targetType.GetField(nameOfPropertyOrField);
        if (field != null)
        {
            object resultObject = field.GetValue(target);
            result = resultObject;
        }
        return result;
    }
    #endregion

    #region 标签剥离
    /// <summary>
    /// 从givenType中剥离标签
    /// </summary>
    public static bool ExtractAttributeWithName(string targetAttributeFullName, Type givenType, ref List<Attribute> result)
    {
        if (givenType == null)
        {
            return false;
        }
        var attributes = givenType.GetCustomAttributes(false);
        foreach (var attribute in attributes)
        {
            Attribute target = attribute as Attribute;
            if (target != null)
            {
                if (string.Equals(target.GetType().FullName, targetAttributeFullName))
                {
                    result.Add(target);
                }
            }
        }
        return result.Count > 0;
    }

    /// <summary>
    /// 从givenMethod中剥离标签
    /// </summary>
    public static bool ExtractAttributeWithName(string targetAttributeFullName, MethodInfo givenMethod, ref List<Attribute> result)
    {
        if (givenMethod == null)
        {
            return false;
        }
        var attributes = givenMethod.GetCustomAttributes(false);
        foreach (var attribute in attributes)
        {
            Attribute target = attribute as Attribute;
            if (target != null)
            {
                if (string.Equals(target.GetType().FullName, targetAttributeFullName))
                {
                    result.Add(target);
                }
            }
        }
        return result.Count > 0;
    }

    /// <summary>
    /// 从givenProperty中剥离标签
    /// </summary>
    public static bool ExtractAttributeWithName(string targetAttributeFullName, PropertyInfo givenProperty, ref List<Attribute> result)
    {
        if (givenProperty == null)
        {
            return false;
        }
        var attributes = givenProperty.GetCustomAttributes(false);
        foreach (var attribute in attributes)
        {
            Attribute target = attribute as Attribute;
            if (target != null)
            {
                if (string.Equals(target.GetType().FullName, targetAttributeFullName))
                {
                    result.Add(target);
                }
            }
        }
        return result.Count > 0;
    }
    #endregion

    #region 过滤提取
    /// <summary>
    /// 从domain中提取所有类，建议外面用的时候做缓存，这个很慢滴
    /// </summary>
    public static List<Type> ScanAllAssmblyTypes(Func<Assembly, bool> filter = null)
    {
        List<Type> allAssmblyTypes = new List<Type>();
        AppDomain domain = AppDomain.CurrentDomain;
        Assembly[] allAssmemblies = domain.GetAssemblies();
        foreach (var assmbly in allAssmemblies)
        {
            if (filter != null && !filter.Invoke(assmbly))
            {
                allAssmblyTypes.AddRange(assmbly.GetTypes());
            }
        }
        return allAssmblyTypes;
    }

    /// <summary>
    /// 在给定Type集合中筛选（且）：
    /// 1.贴有指定标签（如果targetAttributeFullName为null或者empty，则不会约束）
    /// 2.是Interface
    /// 方法保证每个传入processHandler的Type参数均满足要求
    /// </summary>
    public static void FilterAllInterfaceWithAttribute(List<Type> allTypesCollection, string targetAttributeFullName, Action<Type> processHandler)
    {
        if (processHandler == null)
        {
            return;
        }
        foreach (var type in allTypesCollection)
        {
            if (type.IsInterface)
            {
                List<Attribute> temp = new List<Attribute>();
                if (string.IsNullOrEmpty(targetAttributeFullName) || ExtractAttributeWithName(targetAttributeFullName, type, ref temp))
                {
                    processHandler.Invoke(type);
                }
            }
        }
    }

    /// <summary>
    /// 在给定Type集合中筛选（且）：
    /// 1.贴有指定标签（如果targetAttributeFullName为null或者empty，则不会约束）
    /// 2.是Class(不是struct，interface,enum）
    /// 方法保证每个传入processHandler的Type参数均满足要求
    /// </summary>
    public static void FilterAllClassWithAttribute(List<Type> allTypesCollection, string targetAttributeFullName, Action<Type> processHandler)
    {
        if (processHandler == null)
        {
            return;
        }
        foreach (var type in allTypesCollection)
        {
            if (!type.IsInterface && !type.IsValueType && !type.IsEnum)
            {
                List<Attribute> temp = new List<Attribute>();
                if (string.IsNullOrEmpty(targetAttributeFullName) || ExtractAttributeWithName(targetAttributeFullName, type, ref temp))
                {
                    processHandler.Invoke(type);
                }
            }
        }
    }

    /// <summary>
    /// 在给定的Type中筛选方法（且）：
    /// 1.贴有指定标签（如果targetAttributeFullName为null或者empty，则不会约束）
    /// 2.不是getter setter方法
    /// 3.不是索引器（this[])
    /// 方法保证每个传入processHandler的MethodInfo参数均满足要求
    /// </summary>
    public static void FilterAllMethodsWithAttribute(Type givenType, string targetAttributeFullName, Action<MethodInfo> processHandler)
    {
        if (processHandler == null)
        {
            return;
        }

        foreach (var method in givenType.GetMethods())
        {
            if (!method.IsSpecialName)
            {
                List<Attribute> temp = new List<Attribute>();
                if (string.IsNullOrEmpty(targetAttributeFullName) || ExtractAttributeWithName(targetAttributeFullName, method, ref temp))
                {
                    processHandler.Invoke(method);
                }
            }
        }
    }

    /// <summary>
    /// 在给定的givenType中筛选属性（且）：
    /// 1.贴有指定标签（如果targetAttributeFullName为null或者empty，则不会约束）
    /// 2.不是索引器（this[])
    /// 方法保证每个传入processHandler的PropertyInfo参数均满足要求
    /// </summary>
    public static void FilterAllPropertiesWithAttribute(Type givenType, string targetAttributeFullName, Action<PropertyInfo> processHandler)
    {
        if (processHandler == null)
        {
            return;
        }

        foreach (var property in givenType.GetProperties())
        {
            var indexParams = property.GetIndexParameters();

            if (indexParams != null && indexParams.Length > 0)
            {
                return;
            }

            List<Attribute> temp = new List<Attribute>();
            if (string.IsNullOrEmpty(targetAttributeFullName) || ExtractAttributeWithName(targetAttributeFullName, property, ref temp))
            {
                processHandler.Invoke(property);
            }
        }
    }

    /// <summary>
    /// 在给定的givenProperty中筛选方法（且）：
    /// 1.贴有指定标签（如果targetAttributeFullName为null或者empty，则不会约束）
    /// 2.不是索引器（this[])
    /// 方法保证每个传入processHandler的MethodInfo参数均满足要求
    /// </summary>
    public static void FilterAllPropertiesWithAttribute(PropertyInfo givenProperty, string targetAttributeFullName, Action<MethodInfo> processGetterHandler, Action<MethodInfo> processSetterHandler)
    {
        if (processGetterHandler == null)
        {
            return;
        }

        var indexParams = givenProperty.GetIndexParameters();

        if (indexParams != null && indexParams.Length > 0)
        {
            return;
        }

        List<Attribute> temp = new List<Attribute>();
        if (!string.IsNullOrEmpty(targetAttributeFullName) && ExtractAttributeWithName(targetAttributeFullName, givenProperty, ref temp))
        {
            return;
        }

        MethodInfo getMethod = givenProperty.GetGetMethod();
        MethodInfo setMethod = givenProperty.GetSetMethod();

        if (getMethod != null && processGetterHandler != null)
        {
            processGetterHandler.Invoke(getMethod);
        }

        if (setMethod != null && processSetterHandler != null)
        {
            processSetterHandler.Invoke(setMethod);
        }
    }
    #endregion

    #region 一些用到过的高级api
    /// <summary>
    /// 直接从某个标签中获取值
    /// 如果：
    /// 1.标签不存在
    /// 2.标签里没有这个字段/属性
    /// 则返回null
    /// </summary>
    public static object GetValueFromAttribute(Type givenType, string targetAttributeName, string valueName)
    {
        object result = null;
        List<Attribute> attributes = new List<Attribute>();
        if (ExtractAttributeWithName(targetAttributeName, givenType, ref attributes))
        {
            result = GetValue(attributes[0], valueName);
        }
        return result;
    }

    /// <summary>
    /// 直接从某个标签中获取值
    /// 如果：
    /// 1.标签不存在
    /// 2.标签里没有这个字段/属性
    /// 则返回null
    /// </summary>
    public static object GetValueFromAttribute(MethodInfo givenMethod, string targetAttributeName, string valueName)
    {
        object result = null;
        List<Attribute> attributes = new List<Attribute>();
        if (ExtractAttributeWithName(targetAttributeName, givenMethod, ref attributes))
        {
            result = GetValue(attributes[0], valueName);
        }
        return result;
    }

    /// <summary>
    /// 直接从某个标签中获取值
    /// 如果：
    /// 1.标签不存在
    /// 2.标签里没有这个字段/属性
    /// 则返回null
    /// </summary>
    public static object GetValueFromAttribute(PropertyInfo givenProperty, string targetAttributeName, string valueName)
    {
        object result = null;
        List<Attribute> attributes = new List<Attribute>();
        if (ExtractAttributeWithName(targetAttributeName, givenProperty, ref attributes))
        {
            result = GetValue(attributes[0], valueName);
        }
        return result;
    }
    #endregion

    #region 一些常用的filter handler
    /// <summary>
    /// 此filter配合 ScanAllAssmblyTypes 使用，在Unity环境下过滤掉
    /// 1.引擎dll
    /// 2.systemdll
    /// </summary>
    public static bool AssmblyFilter_UNITY(Assembly assembly)
    {
        string name = assembly.GetName().Name;
        bool result = false;
        result |= name.StartsWith("UnityEditor.");
        result |= name.Equals("UnityEditor");
        result |= name.StartsWith("UnityEngine.");
        result |= name.Equals("UnityEngine");
        result |= name.StartsWith("Unity.");
        result |= name.StartsWith("System.");
        result |= name.Equals("System");
        result |= name.StartsWith("Boo.");
        result |= name.StartsWith("UnityScript.");
        result |= name.Equals("UnityScript");
        result |= name.StartsWith("Mono.");
        result |= name.Equals("mscorlib");
        return result;
    }
    #endregion
}
