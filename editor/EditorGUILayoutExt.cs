/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)


using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class EditorGUILayoutExt : MonoBehaviour {
	public static void Vector3ArrayField(ref Vector3[] value, string name, ref bool expanded){
		
		expanded = EditorGUILayout.Foldout( expanded,name);
		if (expanded){
			EditorGUILayout.BeginVertical();
			int count = Mathf.Max(0,EditorGUILayout.IntField("Count",value.Length));
			if (count != value.Length){
				Array.Resize<Vector3>(ref value, count);
			} else {
				for (int i=0;i<count;i++){
					value[i] = EditorGUILayout.Vector3Field("Element "+(i+1),value[i]);
				}
			}
			EditorGUILayout.EndVertical();
		}
	}
}
