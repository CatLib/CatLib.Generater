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

/// <summary>
/// 用于生成Facade的标签，这个标签标记的Interface会被导出成为Facade.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public class GenerateFacadeAttribute : Attribute
{
	/// <summary>
	/// 导出名
	/// </summary>
	public readonly string facadeName;

	/// <summary>
	/// 生成的Facade代码的名称
	/// 1.如果不填：IWorkerSlave -> WorkerSlave; WorkerSlave(非法，因为接口名字必须以大写I开头）
	/// 2.如果填写：[GenerateFacade("AssetLoader")]IWorkerSlave -> AssetLoader; WorkerSlave->AssetLoader(即使非法，也会按照给的名字：AssetLoader进行导出)
	/// </summary>
	public GenerateFacadeAttribute ( string facadeName = "" )
	{
		this.facadeName = facadeName;
	}
}


/// <summary>
/// GenerateFacadeAttribute标记接口的控制条目，对getter/setter；event;method有效
/// 使用此条目可以对生成的Facade代码加注释
/// </summary>
[AttributeUsage(AttributeTargets.Property|AttributeTargets.Event|AttributeTargets.Method,Inherited = false)]
public class GenerateFacadeAPIAttribute : Attribute
{
	/// <summary>
	/// 未来写在Facade上边的注释，不写则没有注释
	/// </summary>
	public readonly string comment;

	/// <summary>
	/// 在GenerateFacadeAttribute标记的接口下，这个标签才会生效
	/// </summary>
	/// <param name="apiComment">未来写在Facade上边的注释，不写则没有注释</param>
	public GenerateFacadeAPIAttribute ( string apiComment = "" )
	{
		this.comment = apiComment;
	}
}
