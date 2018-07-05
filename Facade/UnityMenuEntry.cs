using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UnityMenuEntry
{
	[MenuItem("CatLib/CodeGenerater/FacadeFile")]
	public static void CatLib_CodeGenerater_FacadeFile()
	{
		CatLib_CodeGenerater_APIs.GenerateFacadeFiles ( Application.dataPath + "/../FacadeFiles/", "GenerateFacadeAttribute", "", "GenerateFacadeAPIAttribute" ,Application.dataPath + "/../FacadeFiles/template.txt");
	}
}
