using System.Collections;
using System.Collections.Generic;

using System.IO;

public class CatLib_CodeGenerater_APIs
{
	/// <summary>
	/// 生成FacadeFile到指定目录
	/// 指定目录为绝对路径
	/// </summary>
	/// <param name="folderPath">输出绝对路径.</param>
	/// <param name="interfaceAttributeName">接口标签的名称：打上这个标签，就会被提取.（此字段为空字符串，则所有Interface都会提取）</param>
	/// <param name="methodAttributeName">接口方法标签的名称：打上这个标签，则会被提取。（此字段为空字符串，则会提取每个Interface中的所有方法）</param>
	/// <param name="methodCommentAttributeName">接口方法注释标签的名称：打上这个标签，会在导出时加上注释</param>
	/// <param name="topCommentFilePath">顶部注释模板，是一个绝对路径</param>
	public static void GenerateFacadeFiles ( string folderPath, string interfaceAttributeName, string methodAttributeName, string methodCommentAttributeName ,string topCommentFilePath = "")
	{
		if ( !Directory.Exists ( folderPath ) )
		{
			Directory.CreateDirectory ( folderPath );
		}

		CatLib_CodeGenerater_Facade generater = new CatLib_CodeGenerater_Facade ();
		generater.topCommentFilePath = topCommentFilePath;
		generater.outPutHandler = (className, codeContent ) => {
			string path = string.Format ( "{0}\\{1}.cs", folderPath, className );
			if ( File.Exists ( path ) )
			{
				File.Delete ( path );
			}
			File.WriteAllText ( path, codeContent, new System.Text.UTF8Encoding (false) );
		};

		generater.Start ( interfaceAttributeName, methodAttributeName, methodCommentAttributeName );
	}
}
