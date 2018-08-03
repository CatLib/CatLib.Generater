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

using System.CodeDom;

namespace CatLib.Generater.Editor.Policy.StaticWrap
{
    /// <summary>
    /// 成员聚合器
    /// </summary>
    public class MemberPolymerization : IMemberPolymerization
    {
        // 防歧义处理方案
        // 生成应该自上而下生成
        // 首层生成的事件(属性，函数)会禁止后续的同名属性(事件，函数)的生成
        // (种类间互斥，属性，事件，函数)
        // 
        // 继承的不同接口间出现同名的事件和属性会导致二义性，所以忽略生成
        // 继承的不同接口间出现同名的事件和函数，函数优先
        // 继承的不同接口间出现同名的属性和函数，函数优先
        // 继承关系中函数重载不会导致歧义，但相同的重载只生成一次


        // 先按照 函数，事件，属性的顺序生成首层的成员。成员保存到临时变量中
        // 将首层的成员名字加入禁止使用的成员名字列表
        // 再生成继承的 函数，事件，属性。保存到临时变量中
        // 将函数导入到上下文，同时建立排他名字hash。后续事件和属性的导入会检查排他hash，如果存在则列外。

        /// <summary>
        /// 聚合类型成员转为有效的成员模型列表
        /// </summary>
        /// <param name="typeMembers">需要筛选的类型成员列表</param>
        /// <returns>已经完成筛选的成员模型</returns>
        public CodeTypeMember[] Polymerization(TypeMembers[] typeMembers)
        {
            return null;
        }
    }
}
