using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HMeshTest : MonoBehaviour {

	public enum TestType{ 
		FaceSplit,
		EdgeFlip,
		EdgeSplit
	};

	public TestType testType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		if (GUI.Button(new Rect(0,0,200,50), "FaceSplit")){
			HMesh hMesh = new HMesh();
			var mf = GetComponent<MeshFilter>();
			hMesh.Build(mf.mesh);

			foreach (var f in hMesh.GetFaces()){
				f.Split();
			}
		 
			var mesh = hMesh.Export();
			mesh.RecalculateNormals();
			mf.mesh = mesh;
		}
		if (GUI.Button(new Rect(200,0,200,50), "EdgeFlip")){
			HMesh hMesh = new HMesh();
			var mf = GetComponent<MeshFilter>();
			hMesh.Build(mf.mesh);
			HashSet<Halfedge> isFlipped = new HashSet<Halfedge>();
			foreach (var h in hMesh.GetHalfedges()){
				if (!h.IsBoundary() && !isFlipped.Contains(h)){
					Debug.DrawLine(h.vert.position, h.prev.vert.position,Color.cyan,5);
					h.Flip();
					isFlipped.Add(h.opp);
				}
			}
			var mesh = hMesh.Export();
			mesh.RecalculateNormals();
			mf.mesh = mesh;
		}
		if (GUI.Button(new Rect(400,0,200,50), "EdgeSplit")){
			HMesh hMesh = new HMesh();
			var mf = GetComponent<MeshFilter>();
			hMesh.Build(mf.mesh);
			HashSet<Halfedge> isSplit = new HashSet<Halfedge>();
			foreach (var h in hMesh.GetHalfedges()){
				if (!h.IsBoundary() && !isSplit.Contains(h)){
					Debug.DrawLine(h.vert.position, h.prev.vert.position,Color.cyan,5);
					isSplit.Add(h.opp);
					h.Split();
				}
			}
			var mesh = hMesh.Export();
			mesh.RecalculateNormals();
			mf.mesh = mesh;
		}
		if (GUI.Button(new Rect(600,0,200,50), "CollapseEdge")){
			HMesh hMesh = new HMesh();
			var mf = GetComponent<MeshFilter>();
			hMesh.Build(mf.mesh);
			HashSet<Halfedge> isCollapsed = new HashSet<Halfedge>();
			foreach (var h in hMesh.GetHalfedges()){
				if (!h.IsBoundary()){
					if (!h.vert.IsBoundary() && !h.prev.vert.IsBoundary() && !isCollapsed.Contains(h)){
						Debug.DrawLine(h.vert.position, h.prev.vert.position,Color.cyan,5);
						isCollapsed.Add(h.opp);
						h.Collapse();
						break;
					}
				}
			}
			var mesh = hMesh.Export();
			mesh.RecalculateNormals();
			mf.mesh = mesh;
		}

		if (GUI.Button(new Rect(0,50,200,50), "CirculateRndVertex")){
			HMesh hMesh = new HMesh();
			var mf = GetComponent<MeshFilter>();
			hMesh.Build(mf.mesh);
			HashSet<Halfedge> isCollapsed = new HashSet<Halfedge>();

			var vertices = new List<Vertex>(hMesh.GetVertices());
			var vertex = vertices[1];//Random.Range(0,vertices.Count-1)];
			foreach (var h in vertex.Circulate()){
				Debug.DrawLine(h.vert.position, h.prev.vert.position,Color.cyan,5);
			}
			Debug.Log("Circulate vertex: "+vertex.Circulate().Count);
		}
		if (GUI.Button(new Rect(200,50,200,50), "CirculateOppRndVertex")){
			HMesh hMesh = new HMesh();
			var mf = GetComponent<MeshFilter>();
			hMesh.Build(mf.mesh);
			//HashSet<Halfedge> isCollapsed = new HashSet<Halfedge>();
			
			var vertices = new List<Vertex>(hMesh.GetVertices());
			var vertex = vertices[1];//Random.Range(0,vertices.Count-1)];
			foreach (var h in vertex.CirculateOpp()){
				Debug.DrawLine(h.vert.position, h.prev.vert.position,Color.cyan,5);
			}
			Debug.Log("Circulate vertex: "+vertex.Circulate().Count);
		}
		if (GUI.Button(new Rect(400,50,200,50), "Circulate1RingRndVertex")){
			HMesh hMesh = new HMesh();
			var mf = GetComponent<MeshFilter>();
			hMesh.Build(mf.mesh);
			// HashSet<Halfedge> isCollapsed = new HashSet<Halfedge>();
			
			var vertices = new List<Vertex>(hMesh.GetVertices());
			var vertex = vertices[Random.Range(0,vertices.Count-1)];
			foreach (var h in vertex.CirculateOneRing()){
				Debug.DrawLine(h.vert.position, h.prev.vert.position,Color.cyan,5);
			}
			Debug.Log("Circulate 1-ring: "+vertex.CirculateOneRing().Count);
		}
		if (GUI.Button(new Rect(600,50,200,50), "CirculateRndFace")){
			HMesh hMesh = new HMesh();
			var mf = GetComponent<MeshFilter>();
			hMesh.Build(mf.mesh);
			HashSet<Halfedge> isCollapsed = new HashSet<Halfedge>();
			
			var faces = new List<Face>(hMesh.GetFaces());
			var face = faces[Random.Range(0,faces.Count-1)];
			foreach (var h in face.Circulate()){
				Debug.DrawLine(h.vert.position, h.prev.vert.position, Color.cyan, 5);
			}
			Debug.Log("Circulate face: "+face.Circulate().Count);
		}
	}
}
