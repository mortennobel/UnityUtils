/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Face : System.IEquatable<Face> {
	public Halfedge halfedge;
	public readonly HMesh hmesh;

	public int label = 0;

	// should only be called by HMesh.CreateFace
	public Face(HMesh hmesh){
		this.hmesh = hmesh;
	}

	public int NoEdges(){
		int count = 1;
		Halfedge current = halfedge.next;
		while (current != halfedge){
			count++;
			current = current.next;
		}
		return count;
	}

	public List<Halfedge> Circulate(){
		List<Halfedge> res = new List<Halfedge>();
		Halfedge iter = halfedge;
		bool first = true;
		while (iter != halfedge || first){
			res.Add(iter);
			first = false;
			iter = iter.next;
		}
		return res;
	}

	public void ReassignFaceToEdgeLoop(){
		foreach (var he in Circulate()){
			he.Link(this);
		}
	}

	// Cut the face in two parts. Assumes that the face is convex
	public Face Cut(Vertex v1, Vertex v2){
		Debug.Assert(v1.hmesh == hmesh);
		Debug.Assert(v2.hmesh == hmesh);
		Debug.Assert(v1.halfedge != null);
		Debug.Assert(v2.halfedge != null);
		Debug.Assert(v1 != v2);
		Halfedge v1He = null;
		Halfedge v2He = null;
		foreach (var he in Circulate()){
			if (he.vert == v1){
				v1He = he; 
			}
			else if (he.vert == v2){
				v2He = he;
			}
		}
		if (v1He == null || v2He == null){
			Debug.LogError("Cannot find vertices for split");
			return null;
		}
		Debug.Assert(v1He != v2He);
		Halfedge v1HeNext = v1He.next;
		Halfedge v2HeNext = v2He.next;
		Face f = hmesh.CreateFace();
		Halfedge he1 = hmesh.CreateHalfedge();
		Halfedge he2 = hmesh.CreateHalfedge();

		he1.Glue(he2);

		v1He.Link(he1);
		he1.Link(v2HeNext);
		v2He.Link(he2);
		he2.Link(v1HeNext);
		he1.vert = v2;
		he2.vert = v1;

		halfedge = he1;
		ReassignFaceToEdgeLoop();

		f.halfedge = he2;
		f.ReassignFaceToEdgeLoop();

		v1.halfedge = v1HeNext;
		v2.halfedge = v2HeNext;

		return f;
	}

	// Creates a new vertex inside the face and connect it to the cutFrom vertex using two halfedges.
	// Note that this function leaves the face in an invalid state, since the new halfedges will have face == opp.face.
	public Vertex CutInto(Vertex cutFrom, Vector3 pos){
		Debug.Assert(cutFrom.hmesh == hmesh);
		Halfedge cutFromEdge = null;
		foreach (var e in Circulate()){
			if (e.vert == cutFrom){
				cutFromEdge = e;
			}
		}
		if (cutFromEdge == null){
			Debug.LogError("Cannot find vertex in face");
			return null;
		}

		Vertex newVertex = hmesh.CreateVertex();
		newVertex.position = pos;
		Halfedge he1 = hmesh.CreateHalfedge();
		Halfedge he2 = hmesh.CreateHalfedge();
		he1.Glue(he2);

		Halfedge cutFromEdgeNext = cutFromEdge.next;
		cutFromEdge.Link(he1);
		he1.Link(he2);
		he2.Link(cutFromEdgeNext);

		he1.Link(this);
		he2.Link(this);
		he1.vert = newVertex;
		newVertex.halfedge = he1.next;
		Debug.Assert(newVertex.halfedge != null);
		he2.vert = cutFrom;
		cutFrom.halfedge = he2.next;

		return newVertex;
	}

	// split the face in the center
	// return the new Vertex
	public Vertex Split(){
		Vertex v = hmesh.CreateVertex();

		List<Halfedge> newHalfedges = new List<Halfedge>();
		List<Vertex> vertices = new List<Vertex>();
		// create double halfedges
		foreach (var he in Circulate()){
			v.position += he.vert.position;
			v.uv1 += he.vert.uv1;
			v.uv2 += he.vert.uv2;

			Halfedge toNewVertex = hmesh.CreateHalfedge();
			Halfedge fromNewVertex = hmesh.CreateHalfedge();
			toNewVertex.Glue(fromNewVertex);
			newHalfedges.Add(toNewVertex);
			vertices.Add(he.vert);
		}
		int count = 0;


		// second iteration - link everything together
		foreach (var he in Circulate()){
			Vertex prevVertex = vertices[(count-1+newHalfedges.Count)%newHalfedges.Count];
			Vertex nextVertex = vertices[count];
			Halfedge prevNewOppEdge = newHalfedges[(count-1+newHalfedges.Count)%newHalfedges.Count].opp;
			Halfedge newEdge = newHalfedges[count];
			// link halfedges
			newEdge.Link(prevNewOppEdge);
			prevNewOppEdge.Link(he);
			he.Link(newEdge);

			// link vertices
			newEdge.vert = v;
			v.halfedge = prevNewOppEdge;
			prevNewOppEdge.vert = prevVertex;

			bool first = count==0;
			Face f = he.face;
			if (!first){
				f = hmesh.CreateFace();
			} 
			f.halfedge = he;
			f.ReassignFaceToEdgeLoop();

			count++;
		}

		v.position = v.position * (1.0f/count); // set average position
		v.uv1 = v.uv1 * (1.0f/count);
		v.uv2 = v.uv2 * (1.0f/count);

		return v;
	}



	bool System.IEquatable<Face>.Equals(Face obj){
		return obj == this;
	}

	public override bool Equals (object obj)
	{
		return this == obj;
	}
	
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public Bounds bound{
		get {
			var faces = Circulate();
			Bounds b = new Bounds(faces[0].vert.position, Vector3.zero);
			for (int i=1;i<faces.Count;i++){
				b.Encapsulate(faces[i].vert.position);
			}
			return b;
		}
	} 

	public bool IsValid(){
		bool valid = true;
		if (halfedge == null){
			Debug.LogWarning("Halfedge is null");
			valid = false;
		}

		foreach (var he in Circulate()){
			if (he.face != this){
				Debug.LogWarning("Halfedge.face is not correct");
				valid = false;
			}
			if (!hmesh.GetHalfedges().Contains(he)){
				Debug.LogWarning("Halfedge.face is not correct");
				valid = false;	
			}
		}
		return valid;
	}
}
