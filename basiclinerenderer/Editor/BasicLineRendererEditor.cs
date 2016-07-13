/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)


using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(BasicLineRenderer))]
public class BasicLineRendererEditor : Editor {
	
	//Assets/basiclinerenderer/Editor/BasicLineRendererEditor.cs(8,14): warning CS0114: `BasicLineRendererEditor.OnInspectorGUI()' hides inherited member `UnityEditor.Editor.OnInspectorGUI()'. 
	//	To make the current member override that implementation, add the override keyword. Otherwise add the new keyword
	
	
	private bool parameters = false;
	private bool positionsExpanded = false;
	
	public override void OnInspectorGUI () {
		
		BasicLineRenderer basicLineRenderer = target as BasicLineRenderer;
		
		DrawDefaultInspector();
		EditorGUILayoutExt.Vector3ArrayField(ref basicLineRenderer.positions, "Positions", ref positionsExpanded);
		basicLineRenderer.UpdateMesh();
	}
}
