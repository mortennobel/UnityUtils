using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HMeshLineRenderer))]
public class HMeshARendererEditor : Editor  {
	public override void OnInspectorGUI()
	{
	    HMeshARenderer myTarget = (HMeshARenderer)target;
		DrawDefaultInspector();
		if (GUILayout.Button("UpdateMesh")){
			myTarget.UpdateMesh();
		}
	}
}
