using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

/// <summary>
/// 生成器端，代码书写函数，均在此定义
/// </summary>
public class CatLib_AOTKit_CodeWritter 
{
	/// <summary>
	/// 顶部注释模板
	/// TODO，正则替换时间，作者等功能还没加
	/// </summary>
	public static string TopComment ( string templateFilePath)
	{
		if ( !File.Exists ( templateFilePath ) )
		{
			return string.Empty;
		}
		return TopCommentWithContent ( File.ReadAllText ( templateFilePath ) );
	}

	/// <summary>
	/// 顶部注释模板
	/// TODO，正则替换时间，作者等功能还没加
	/// </summary>
	public static string TopCommentWithContent ( string content )
	{
		return content;
	}

	/// <summary>
	/// 书写一个comment代码
	/// </summary>
	public static string BuildCommentXMLFormat (int tapCount, string commentContent )
	{
		string tapHead = string.Empty;
		for ( int i = 0 ; i < tapCount ; i++ )
		{
			tapHead+="\t";
		}

		StringBuilder builder = new StringBuilder ();
		builder.Append ( tapHead + "/// <summary>\n" );
		builder.AppendFormat ( tapHead + "/// {0}\n", commentContent );
		builder.Append ( tapHead + "/// </summary>\n" );
		return builder.ToString ();
	}

	/// <summary>
	/// 建议使用这个方法来处理Type打印成string
	/// 此方法处理了泛型问题。
	/// </summary>
	public static string TypeToString ( Type givenType )
	{
		if ( givenType.FullName == "System.Void" )
		{
			return "void";
		}

		if ( !givenType.IsGenericTypeDefinition && givenType.IsGenericType )
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			var genericType = givenType.GetGenericTypeDefinition ();
			var genericParameters = givenType.GetGenericArguments ();
			builder.Append ( genericType.FullName.Replace ( string.Format ( "`{0}", genericParameters.Length ), "" ) );
			builder.Append ( "<" );
			for ( int i = 0 ; i < genericParameters.Length ; i++ )
			{
				var genericParameter = genericParameters [i];
				builder.Append ( TypeToString ( genericParameter ) );
				if ( i != genericParameters.Length - 1 )
				{
					builder.Append ( ", " );
				}
			}
			builder.Append ( ">" );
			return builder.ToString ();
		}
		return string.IsNullOrEmpty ( givenType.FullName ) ? givenType.Name : givenType.FullName;
	}

	/// <summary>
	/// 将函数名打印成代码
	/// </summary>
	public static string MethodNameToString ( MethodInfo givenMethod )
	{
		if ( givenMethod.IsGenericMethodDefinition && givenMethod.IsGenericMethod )
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			var genericType = givenMethod.GetGenericMethodDefinition ();
			var genericParameters = givenMethod.GetGenericArguments ();
			builder.Append ( genericType.Name );
			builder.Append ( "<" );
			for ( int i = 0 ; i < genericParameters.Length ; i++ )
			{
				var genericParameter = genericParameters [i];
				builder.Append ( TypeToString ( genericParameter ) );
				if ( i != genericParameters.Length - 1 )
				{
					builder.Append ( ", " );
				}
			}
			builder.Append ( ">" );
			return builder.ToString ();
		}
		return givenMethod.Name;
	}

	/// <summary>
	/// 将所给函数的参数列表打印成代码
	/// </summary>
	public static string MethodParamsToString ( MethodInfo givenMethod )
	{
		System.Text.StringBuilder builder = new System.Text.StringBuilder ();
		builder.Append ( "(" );
		var methodParams = givenMethod.GetParameters ();
		for ( int i = 0 ; i < methodParams.Length ; i++ )
		{
			builder.AppendFormat ( "{0} {1}", TypeToString ( methodParams [i].ParameterType ), methodParams [i].Name );
			if ( i != methodParams.Length - 1 )
			{
				builder.Append ( ", " );
			}
		}
		builder.Append ( ")" );
		return builder.ToString ();
	}

	/// <summary>
	/// 将所给函数的限制列表（where）打印成代码
	/// </summary>
	public static string MethodConstraintsToString ( MethodInfo givenMethod )
	{
		if ( givenMethod.IsGenericMethodDefinition && givenMethod.IsGenericMethod )
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			var genericParameters = givenMethod.GetGenericArguments ();
			for ( int i = 0 ; i < genericParameters.Length ; i++ )
			{
				var genericParameter = genericParameters [i];

				string content = string.Empty;
				GenericParameterAttributes constraints = genericParameter.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
				//class
				if ( (constraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0 )
				{
					content += "class,";
				}
				//new
				if ( (constraints & GenericParameterAttributes.DefaultConstructorConstraint) != 0 )
				{
					content += "new(),";
				}

				var paramTypeConstraints = genericParameter.GetGenericParameterConstraints ();
				foreach ( var paramTypeConstraint in paramTypeConstraints )
				{
					content += TypeToString ( paramTypeConstraint );
					content += ",";
				}
				if ( !string.IsNullOrEmpty ( content ) )
				{
					content = content.Substring ( 0, content.Length - 1 );
					builder.Append ( " where " );
					builder.Append ( genericParameter.Name );
					builder.Append ( " : " );
					builder.Append ( content );
				}
			}

			return builder.ToString ();
		}
		return string.Empty;
	}

	/// <summary>
	/// 将所给函数的参数列表打印成代码(无类型，这个用于api穿惨代码生成）
	/// </summary>
	public static string MethodParamsToStringWithOutType ( MethodInfo givenMethod )
	{
		System.Text.StringBuilder builder = new System.Text.StringBuilder ();
		builder.Append ( "(" );
		var methodParams = givenMethod.GetParameters ();
		for ( int i = 0 ; i < methodParams.Length ; i++ )
		{
			builder.AppendFormat ( "{0}", methodParams [i].Name );
			if ( i != methodParams.Length - 1 )
			{
				builder.Append ( ", " );
			}
		}
		builder.Append ( ")" );
		return builder.ToString ();
	}
}
