/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HMeshRendererType {
	Full,
	BoundaryEdges,
	Vertices
}

public class HMeshRenderer : MonoBehaviour {

	public HMesh hmesh;

	public Mesh mesh;

	public Color color = Color.red;

	public HMeshRendererType renderType = HMeshRendererType.Full;

	Mesh GetMesh(){
		if (mesh == null){
			mesh = new Mesh();
		}
		return mesh;
	}

	void OnDrawGizmos() {
		Gizmos.color = color;

		Gizmos.DrawWireMesh(GetMesh(),transform.position, transform.rotation, transform.localScale);
	}

	// Update is called once per frame
	public Mesh UpdateMesh () {
		Mesh mesh = GetMesh();

		Debug.Log("UpdateMesh");

		List<Vector3> lines = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<int> indices = new List<int>();
		HMesh hmesh = this.hmesh;
		foreach (var face in hmesh.GetFaces()){
			foreach (var edge in face.Circulate()){
				if (renderType == HMeshRendererType.BoundaryEdges){
					if (!edge.IsBoundary()){
						continue;
					}
				} 
				lines.Add(edge.prev.vert.position);
				lines.Add(edge.vert.position);
				normals.Add(Vector3.up);
				normals.Add(Vector3.up);
				indices.Add(indices.Count);
				indices.Add(indices.Count);
			}
		}
		mesh.Clear();
		mesh.vertices = lines.ToArray();
		mesh.normals = normals.ToArray();
		var meshTopology = renderType == HMeshRendererType.Vertices ? MeshTopology.Points : MeshTopology.Lines;
		mesh.SetIndices(indices.ToArray(), meshTopology, 0);
		mesh.RecalculateBounds();
		mesh.UploadMeshData(false);

		return mesh;
	}

	public void CreateHMesh(){
		hmesh = HMesh.CreateTestMesh();
	}

	public void CreateHMeshTriangle(){
		hmesh = HMesh.CreateTestMeshTriangle();
	}

	public void CreateHMeshQuad(){
		hmesh = HMesh.CreateTestMeshQuad();
	}

	public void ApplyTransform(){
		var localToWorld = transform.localToWorldMatrix;
		foreach (var vert in hmesh.GetVertices()){
			vert.position = localToWorld.MultiplyPoint(vert.position);
		}
		transform.position = Vector3.zero;
		transform.localScale = Vector3.one;
		transform.rotation = Quaternion.identity;
	}
}
