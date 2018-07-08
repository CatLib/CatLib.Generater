using System;
using System.Collections.Generic;
using System.Reflection;
using CatReflection = CatLib_AOTKit_Reflection;
using CatWritter = CatLib_AOTKit_CodeWritter;

public class CatLib_CodeGenerater_Facade
{
    public string givenNameSpace = "CatLib.Facade";
    public string topCommentFilePath;

    public System.Action<string, string> outPutHandler;

    public void Start(string interfaceAttributeName, string methodAttributeName, string methodCommentAttributeName)
    {
        if (outPutHandler == null)
        {
            return;
        }

        List<Type> allTypes = CatReflection.ScanAllAssmblyTypes(CatReflection.AssmblyFilter_UNITY);

        List<Type> allMarkedInterfaceTypes = new List<Type>();
        CatReflection.FilterAllInterfaceWithAttribute(allTypes, interfaceAttributeName, allMarkedInterfaceTypes.Add);

        foreach (var interfaceType in allMarkedInterfaceTypes)
        {
            //收集信息
            List<MethodInfo> allMethods = new List<MethodInfo>();
            CatReflection.FilterAllMethodsWithAttribute(interfaceType, methodAttributeName, allMethods.Add);

            List<PropertyInfo> allProperties = new List<PropertyInfo>();
            CatReflection.FilterAllPropertiesWithAttribute(interfaceType, methodAttributeName, allProperties.Add);

            var parentInterfaces = interfaceType.GetInterfaces();
            foreach (var parentInterface in parentInterfaces)
            {
                CatReflection.FilterAllMethodsWithAttribute(parentInterface, methodAttributeName, allMethods.Add);
                CatReflection.FilterAllPropertiesWithAttribute(parentInterface, methodAttributeName, allProperties.Add);
            }

            //facade 名称
            string interfaceTypeDefaultName = interfaceType.Name;
            interfaceTypeDefaultName = interfaceTypeDefaultName.StartsWith("I") ? interfaceTypeDefaultName.Substring(1, interfaceTypeDefaultName.Length - 1) : string.Format("Facade_{0}", interfaceTypeDefaultName);
            string facadeName = CatReflection.GetValueFromAttribute(interfaceType, interfaceAttributeName, "facadeName") as string;
            facadeName = string.IsNullOrEmpty(facadeName) ? interfaceTypeDefaultName : facadeName;

            //写代码
            System.Text.StringBuilder codeBuilder = new System.Text.StringBuilder();

            //注释
            codeBuilder.Append(CatWritter.TopComment(topCommentFilePath));
            codeBuilder.Append("\n");

            //using 头
            codeBuilder.Append("using ");
            codeBuilder.Append(interfaceType.Namespace);
            codeBuilder.Append(";\n");
            codeBuilder.Append("\n");

            //命名空间
            codeBuilder.Append("namespace ");
            codeBuilder.Append(givenNameSpace);
            codeBuilder.Append("{\n"); //命名空间

            //类声明
            codeBuilder.Append("\n");
            codeBuilder.Append("\tpublic sealed class ");
            codeBuilder.Append(facadeName);
            codeBuilder.Append(" : ");
            codeBuilder.AppendFormat("Facade<{0}>\n", CatWritter.TypeToString(interfaceType));
            codeBuilder.Append("\t{\n"); //类声明

            //打印一个方法
            foreach (var method in allMethods)
            {
                //注释
                string comment = CatReflection.GetValueFromAttribute(method, methodCommentAttributeName, "comment") as string;
                if (!string.IsNullOrEmpty(comment))
                {
                    codeBuilder.Append(CatWritter.BuildCommentXMLFormat(2, comment));
                }

                //函数声明
                codeBuilder.AppendFormat("\t\tpublic static {0} {1} ", CatWritter.TypeToString(method.ReturnType), CatWritter.MethodNameToString(method));
                codeBuilder.Append(CatWritter.MethodParamsToString(method));
                codeBuilder.Append(CatWritter.MethodConstraintsToString(method));
                codeBuilder.Append(" \n");
                codeBuilder.Append("\t\t{\n ");//函数声明

                //函数体
                codeBuilder.Append("\t\t\t");
                if (method.ReturnType.FullName != "System.Void")
                {
                    codeBuilder.Append("return ");
                }
                codeBuilder.Append(facadeName);
                codeBuilder.Append(".Instance.");
                codeBuilder.Append(CatWritter.MethodNameToString(method));
                codeBuilder.Append(CatWritter.MethodParamsToStringWithOutType(method));
                codeBuilder.Append(";\n");
                codeBuilder.Append("\t\t}\n ");//函数声明
                codeBuilder.Append("\n");
            }

            //打印一个属性
            foreach (var property in allProperties)
            {
                MethodInfo getMethod = property.GetGetMethod();
                MethodInfo setMethod = property.GetSetMethod();

                if (getMethod != null || setMethod != null)
                {
                    //注释
                    string comment = CatReflection.GetValueFromAttribute(property, methodCommentAttributeName, "comment") as string;
                    if (!string.IsNullOrEmpty(comment))
                    {
                        codeBuilder.Append(CatWritter.BuildCommentXMLFormat(2, comment));
                    }

                    //属性声明
                    codeBuilder.AppendFormat("\t\tpublic static {0} {1} ", CatWritter.TypeToString(property.PropertyType), property.Name);
                    codeBuilder.Append(" \n");
                    codeBuilder.Append("\t\t{\n ");//属性声明

                    //getter体
                    if (getMethod != null)
                    {
                        codeBuilder.Append("\t\t\tget");
                        codeBuilder.Append("{ return ");
                        codeBuilder.Append(facadeName);
                        codeBuilder.Append(".Instance.");
                        codeBuilder.Append(property.Name);
                        codeBuilder.Append(";}\n");
                    }

                    //setter体
                    if (setMethod != null)
                    {
                        codeBuilder.Append("\t\t\tset");
                        codeBuilder.Append("{ ");
                        codeBuilder.Append(facadeName);
                        codeBuilder.Append(".Instance.");
                        codeBuilder.Append(property.Name);
                        codeBuilder.Append(" = value;}\n");
                    }

                    codeBuilder.Append("\t\t}\n ");//属性声明
                }
                codeBuilder.Append("\n");
            }

            codeBuilder.Append("\t}\n"); //类声明
            codeBuilder.Append("}\n"); //命名空间

            outPutHandler.Invoke(facadeName, codeBuilder.ToString());
        }
    }
}
