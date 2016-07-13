/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(HMeshRenderer))]
public class HMeshRendererEditor : Editor  {
	public override void OnInspectorGUI()
	{
		HMeshRenderer myTarget = (HMeshRenderer)target;
		DrawDefaultInspector();
		if (GUILayout.Button("UpdateMesh")){
			myTarget.UpdateMesh();
		}
		if (GUILayout.Button("Create HMesh plane")){
			myTarget.CreateHMesh();
			myTarget.UpdateMesh();
		}
		if (GUILayout.Button("Create HMesh triangle")){
			myTarget.CreateHMeshTriangle();
			myTarget.UpdateMesh();
		}
		if (GUILayout.Button("Create HMesh Quad")){
			myTarget.CreateHMeshQuad();
			myTarget.UpdateMesh();
		}
		if (GUILayout.Button("Apply transform")){
			myTarget.ApplyTransform();
			myTarget.UpdateMesh();
		}
	}
}
