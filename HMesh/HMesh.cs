/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HMesh {
	HashSet<Vertex> vertices = new HashSet<Vertex>();
	HashSet<Face> faces = new HashSet<Face>();
	HashSet<Halfedge> halfedges = new HashSet<Halfedge>();

	public HashSet<Vertex> GetVertices(){
		return new HashSet<Vertex>(vertices);
	}

	public HashSet<Face> GetFaces(){
		return new HashSet<Face>(faces);
	}

	public HashSet<Halfedge> GetHalfedges(){
		return new HashSet<Halfedge>(halfedges);
	}

	public HMesh(){
	}

	public HMesh Copy(){
		List<Vertex> fromVert = new List<Vertex>();
		List<Face> fromFaces = new List<Face>();
		List<Halfedge> fromHalfedge = new List<Halfedge>();

		List<Vertex> toVert = new List<Vertex>();
		List<Face> toFace = new List<Face>();
		List<Halfedge> toHalfedge = new List<Halfedge>();

		HMesh newMesh = new HMesh();

		int index = 0;
		foreach (var v in vertices){
			v.label = index;
			fromVert.Add(v);
			var nv = newMesh.CreateVertex(v.position);
			nv.uv1 = v.uv1;
			nv.uv2 = v.uv2;
			toVert.Add(nv);
			index++;
		}
		index = 0;
		foreach (var f in faces){
			f.label = index;
			fromFaces.Add(f);
			var nf = newMesh.CreateFace();
			nf.label = f.label;
			toFace.Add(nf);
			index++;
		}
		index = 0;
		foreach (var e in halfedges){
			e.label = index;
			fromHalfedge.Add(e);
			toHalfedge.Add(newMesh.CreateHalfedge());
			index++;
		}

		// copy references
		foreach (var from in vertices){
			Vertex to = toVert[from.label];
			if (from.halfedge != null){to.halfedge = toHalfedge[from.halfedge.label];}
		
		}

		foreach (var from in faces){
			Face to = toFace[from.label];
			if (from.halfedge != null){to.halfedge = toHalfedge[from.halfedge.label];}
		}

		foreach (var from in halfedges){
			Halfedge to = toHalfedge[from.label];
			if (from.face != null){to.face = toFace[from.face.label];}
			if (from.opp  != null){to.opp  = toHalfedge[from.opp.label];}
			if (from.next != null){to.next = toHalfedge[from.next.label];}
			if (from.prev != null){to.prev = toHalfedge[from.prev.label];}
			if (from.vert != null){to.vert = toVert[from.vert.label];}
		
		}
		return newMesh;
	}

	public static HMesh CreateTestMesh() {
		HMesh mesh = new HMesh();
		GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		var meshFilter = plane.GetComponent<MeshFilter>();
		mesh.Build(meshFilter.sharedMesh,Matrix4x4.identity);
		GameObject.DestroyImmediate(plane);
		return mesh;
	}

	public static HMesh CreateTestMeshTriangle() {
		HMesh mesh = new HMesh();
		var face = mesh.CreateFace();
		var edges = new Halfedge[]{
			mesh.CreateHalfedge(),
			mesh.CreateHalfedge(),
			mesh.CreateHalfedge()
		};
		var verts = new Vertex[]{
			mesh.CreateVertex(new Vector3(0,0,0)),
			mesh.CreateVertex(new Vector3(1,0,0)),
			mesh.CreateVertex(new Vector3(0,0,1)),
		};
		for (int i=0;i<3;i++){
			edges[i].Link(face);
			edges[i].next = edges[(i+1)%3];
			edges[(i+1)%3].prev = edges[i];

			edges[i].vert = verts[i];
			verts[i].halfedge = edges[i].next;
		}
		return mesh;
	}

	public static HMesh CreateTestMeshQuad() {
		HMesh mesh = new HMesh();
		var face = mesh.CreateFace();
		var edges = new Halfedge[]{
			mesh.CreateHalfedge(),
			mesh.CreateHalfedge(),
			mesh.CreateHalfedge(),
			mesh.CreateHalfedge(),
		};
		var verts = new Vertex[]{
			mesh.CreateVertex(new Vector3(0,0,0)),
			mesh.CreateVertex(new Vector3(1,0,0)),
			mesh.CreateVertex(new Vector3(1,0,1)),
			mesh.CreateVertex(new Vector3(0,0,1)),
		};
		for (int i=0;i<4;i++){
			edges[i].Link(face);
			edges[i].next = edges[(i+1)%4];
			edges[(i+1)%4].prev = edges[i];

			edges[i].vert = verts[i];
			verts[i].halfedge = edges[i].next;
		}
		return mesh;
	}

	uint EdgeKey(int vertex1, int vertex2){
		ushort vertex1s = (ushort)vertex1;
		ushort vertex2s = (ushort)vertex2;
		return(uint)(vertex1s + (vertex2s<<16));
	}

	public void Build(Mesh mesh){
		Build(mesh, Matrix4x4.identity);
	}

	public void Build(Mesh mesh, Matrix4x4 transform){
		if (mesh.subMeshCount != 1){
			Debug.LogError("Invalid mesh.subMeshCount. Must be 1.");
		}
		if (mesh.GetTopology(0) != MeshTopology.Triangles){
			Debug.LogError("Only triangles supported.");
		}
		List<Vertex> vertexList = new List<Vertex>();
		Dictionary<uint, Halfedge> halfedgeByVertexID = new Dictionary<uint, Halfedge>();

		// Create a list of (HMesh) Vertices
		bool hasUv1 = mesh.uv != null && mesh.uv.Length == mesh.vertices.Length;
		bool hasUv2 = mesh.uv2 != null && mesh.uv2.Length == mesh.vertices.Length;
		for (int i=0;i<mesh.vertices.Length;i++){
			var newV = CreateVertex();
			newV.position = transform.MultiplyPoint(mesh.vertices[i]);
			if (hasUv1){
				newV.uv1 = mesh.uv[i];
			}
			if (hasUv2){
				newV.uv2 = mesh.uv2[i];
			}
			vertexList.Add(newV);
		}

		// create faces and half edges
		for (int i=0;i<mesh.triangles.Length;i+=3){
			int[] idx = new int[]{
				mesh.triangles[i],
				mesh.triangles[i+1],
				mesh.triangles[i+2]};
			Halfedge[] edges = new Halfedge[3];
			Face face = CreateFace();
			for (int j=0;j<3;j++){
				Halfedge edge = CreateHalfedge();
				edge.Link(face);
				edges[j] = edge;
			}
			for (int j=0;j<3;j++){
				int from = idx[j];
				int to = idx[(j+1)%3];
				edges[j].Link(edges[(j+1)%3]);
				edges[j].vert = vertexList[to];
				vertexList[to].halfedge = edges[j].next;
				uint edgeId = EdgeKey(from,to);
				halfedgeByVertexID.Add(edgeId, edges[j]);
			}
		}
		int glued = 0;
		// glue all opposite half edges
		for (int i=0;i<mesh.triangles.Length;i+=3){
			int[] idx = new int[]{
				mesh.triangles[i],
				mesh.triangles[i+1],
				mesh.triangles[i+2]};
			for (int j=0;j<3;j++){
				int from = idx[j];
				int to = idx[(j+1)%3];
				uint key = EdgeKey(from,to);
				uint oppKey = EdgeKey(to,from);
				Halfedge edge = halfedgeByVertexID[key];
				bool isOppUnassigned = edge.opp == null;
				if (isOppUnassigned && halfedgeByVertexID.ContainsKey(oppKey)){
					Halfedge oppEdge = halfedgeByVertexID[oppKey];
					edge.Glue(oppEdge);
					glued++;
				}
			}
		}
	}

	public Mesh Export(){
		List<Vertex> vertexList = new List<Vertex>(vertices);
		Mesh res = new Mesh();
		Vector3[] vertexArray = new Vector3[vertexList.Count];
		Vector2[] uv1 = new Vector2[vertexList.Count];
		Vector2[] uv2 = new Vector2[vertexList.Count];
		for (int i=0;i<vertexArray.Length;i++){
			vertexArray[i] = vertexList[i].position;
			uv1[i] = vertexList[i].uv1;
			uv2[i] = vertexList[i].uv2;
		}
		res.vertices = vertexArray;
		res.uv = uv1;
		res.uv2 = uv2;
		List<int> triangles = new List<int>();
		foreach (var face in faces){
			if (face.NoEdges() != 3){
				Debug.LogError("Only triangles supported");
				continue;
			}
			var he = face.halfedge;
			bool first = true;
			while (he != face.halfedge || first){
				int indexOfVertex = vertexList.IndexOf(he.vert);
				triangles.Add(indexOfVertex);
				he = he.next;
				first = false;
			}
		}

		string s = "";
		foreach (var i in triangles.ToArray()){
			s+= i+", ";
		}
		Debug.Log("Exporting triangles "+s+" count "+triangles.Count);

		Debug.Log("Vertices "+vertexList.Count);
		res.SetTriangles(triangles.ToArray(),0);
		return res;
	}

	public void Clear(){
		foreach (var v in new List<Vertex>(vertices)){
			Destroy(v);
		}
		foreach (var f in new List<Face>(faces)){
			Destroy(f);
		}
		foreach (var h in new List<Halfedge>(halfedges)){
			Destroy(h);
		}
	}

	public Vertex CreateVertex(){
		var res = new Vertex(this);
		vertices.Add(res);
		return res;
	}

	public Vertex CreateVertex(Vector3 p){
		var res = new Vertex(this);
		res.position = p;
		vertices.Add(res);
		return res;
	}

	public Face CreateFace(){
		var res = new Face(this);
		faces.Add(res);
		return res;
	}

	public Halfedge CreateHalfedge(){
		var res = new Halfedge(this);
		halfedges.Add(res);
		return res;
	}

	public bool Destroy(Face face){
		bool res = faces.Remove(face);
		if (res){
			face.halfedge = null;
		}
		return res;
	}

	public bool Destroy(Halfedge halfedge){
		bool res = halfedges.Remove(halfedge);
		if (res){
			halfedge.face = null;
			halfedge.next = null;
			halfedge.opp = null;
			halfedge.prev = null;
			halfedge.vert = null;
		}
		return res;
	}

	public bool Destroy(Vertex vertex){
		bool res = vertices.Remove(vertex);
		if (res){
			vertex.halfedge = null;
			vertex.position = new Vector3(Mathf.Infinity,Mathf.Infinity,Mathf.Infinity);
		}
		return res;
	}

	public bool IsValid(){
		bool valid = true;
		foreach (var v in vertices){
			valid &= v.IsValid();
		}
		foreach (var he in halfedges){
			valid &= he.IsValid();
		}
		foreach (var f in faces){
			valid &= f.IsValid();
		}
		return valid;
	}

}
