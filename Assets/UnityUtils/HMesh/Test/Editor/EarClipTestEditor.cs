using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(EarClipTest))]
public class EarClipTestEditor : Editor {
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		var earClipTest = target as EarClipTest;
		if (GUILayout.Button("Create mesh")){
			var mesh = CreateTestMeshPolygon(earClipTest);
			var renderer = earClipTest.GetComponent<HMeshRenderer>();
			renderer.hmesh = mesh;
			renderer.UpdateMesh();
		}
		if (GUILayout.Button("Create and triangulate")){
			var mesh = CreateTestMeshPolygon(earClipTest);
			foreach (var f in mesh.GetFaces()){
				EarClipping.Tesselate(f);
			}
			var renderer = earClipTest.GetComponent<HMeshRenderer>();
			renderer.hmesh = mesh;
			renderer.UpdateMesh();
		}
	}

	public static HMesh CreateTestMeshPolygon(EarClipTest clip) {
		HMesh mesh = new HMesh();

		Transform t = clip.transform;

		var face = mesh.CreateFace();

		var edges = new Halfedge[t.childCount];
		var verts = new List<Vertex>();
		for (int i=0;i<t.childCount;i++){
			edges[i] = mesh.CreateHalfedge();
			verts.Add(mesh.CreateVertex(t.GetChild(i).position));
		}

		for (int i=0;i<t.childCount;i++){
			edges[i].Link(face);
			edges[i].next = edges[(i+1)%t.childCount];
			edges[(i+1)%t.childCount].prev = edges[i];

			edges[i].vert = verts[i];
		}
		return mesh;
	}
}
