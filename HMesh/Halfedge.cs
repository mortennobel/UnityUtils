/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;

public class Halfedge  : System.IEquatable<Halfedge>  {
	public Halfedge next;
	public Halfedge prev;
	public Halfedge opp;
	public Vertex vert;
	public Face face;

	public int label;

	public readonly HMesh hmesh;

	// should only be called by HMesh.CreateHalfEdge
	public Halfedge(HMesh hmesh){
		this.hmesh = hmesh;
	}

	public Vertex Collapse(bool center = false){
		if (center){
			vert.position = (vert.position + prev.vert.position)*0.5f;
			vert.uv1 = (vert.uv1 + prev.vert.uv1)*0.5f;
			vert.uv2 = (vert.uv2 + prev.vert.uv2)*0.5f;
		}
		CollapseInternal(false);
		if (opp != null){
			opp.CollapseInternal(true);
			hmesh.Destroy(opp);
		}
		hmesh.Destroy(this);

		return vert;
	}


	void CollapseInternal(bool opp){

		if (!opp) {
			prev.vert.ReplaceVertex(vert);
			hmesh.Destroy(prev.vert);
		} else {
			vert.ReplaceVertex(prev.vert);
			hmesh.Destroy(vert);
		}
		prev.opp.Glue(next.opp);
		foreach (var he in face.Circulate()){
			hmesh.Destroy(he);
		}
		hmesh.Destroy(face);
	}

	public void Flip(){
		#if UNITY_EDITOR		
		if (IsBoundary()){
			Debug.LogError("Cannot flip boundary edge");
			return;
		}
		if (face.NoEdges() != 3 || opp.face.NoEdges() != 3){
			Debug.LogError("Can only flip edge between two triangles");
		}
		Debug.Assert(hmesh.IsValid());
		#endif

		Halfedge oldNext = next;
		Halfedge oldPrev = prev;
		Halfedge oldOppNext = opp.next;
		Halfedge oldOppPrev = opp.prev;

		Vertex thisVert = vert;
		Vertex oppVert = opp.vert;
		Vertex thisOppositeVert = next.vert;
		Vertex oppOppositeVert = opp.next.vert;

		// flip halfedges
		oldOppNext.Link(this);
		this.Link(oldPrev);

		oldNext.Link(opp);
		opp.Link(oldOppPrev);

		oldOppPrev.Link(oldNext);
		oldPrev.Link (oldOppNext);

		// reassign vertices
		this.vert = thisOppositeVert;
		thisOppositeVert.halfedge = oldPrev;
		opp.vert = oppOppositeVert;
		oppOppositeVert.halfedge = oldOppPrev;
		thisOppositeVert.halfedge = oldPrev;
		thisVert.halfedge = oldNext;
		oppVert.halfedge = oldOppNext;

		face.halfedge = this;
		face.ReassignFaceToEdgeLoop();
		opp.face.halfedge = opp;
		opp.face.ReassignFaceToEdgeLoop();
		#if UNITY_EDITOR	
		Debug.Assert(hmesh.IsValid());
		#endif
	}

	public bool IsBoundary(){
		return opp==null;
	}

	// Set next reference to nextEdge and nextEdge.prev to this
	public void Link(Halfedge nextEdge){
		if (this == nextEdge){
			throw new UnityException("Link of self");
		}
		next = nextEdge;
		nextEdge.prev = this;
	}

	public void Link(Face newFace){
		Debug.Assert(newFace.hmesh == hmesh);
		face = newFace;
		face.halfedge = this;
	}

	public Vertex Split(float slitFraction = 0.5f){
		Vertex vertex = hmesh.CreateVertex();
		vertex.position = Vector3.Lerp(prev.vert.position,vert.position,slitFraction);
		vertex.uv1 = Vector2.Lerp(prev.vert.uv1,vert.uv1,slitFraction);
		vertex.uv2 = Vector2.Lerp(prev.vert.uv2,vert.uv2,slitFraction);
		var newHE = SplitInternal(vertex);
		Debug.Assert(newHE.IsValid()); 
		Debug.Assert(vertex.IsValid()); 
		return vertex;
	}

	// glue two halfedges together
	public void Glue(Halfedge oppEdge){
		Debug.Assert(oppEdge.hmesh == hmesh);
		if (oppEdge == this){
			Debug.LogWarning("Glue to self");
		}
		opp = oppEdge;
		oppEdge.opp = this;
	}

	public override bool Equals (object obj)
	{
		return this == obj;
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	bool System.IEquatable<Halfedge>.Equals(Halfedge obj){
		return obj == this;
	}

	Halfedge SplitInternal(Vertex vertex){
		Debug.Assert(vertex.hmesh == hmesh);
		Halfedge oldNext = next;
		Vertex oldVert = vert;

		Halfedge splitEdge1 = hmesh.CreateHalfedge();

		if (opp != null){
			Vertex oppositeVert = prev.vert;
			Halfedge oppOldPrev = opp.prev;

			Halfedge splitEdge2 = hmesh.CreateHalfedge();
			splitEdge1.Glue(splitEdge2);

			splitEdge2.vert = vertex;
			oppOldPrev.Link(splitEdge2);
			splitEdge2.Link(opp.face);
			splitEdge2.Link(opp);
		}

		// link face
		splitEdge1.Link(face);

		splitEdge1.Link(oldNext);

		// link vertex
		splitEdge1.vert = oldVert;
		oldVert.halfedge = splitEdge1.next;

		vert = vertex;
		vertex.halfedge = splitEdge1;
		Link(splitEdge1);

		return splitEdge1;
	}

	public bool IsValid(){
		bool valid = true;
		if (opp != null && opp.opp != this){
			Debug.LogWarning("opp is different from this or null");
			valid = false;
		}
		if (opp != null && vert == opp.vert){
			Debug.LogWarning("opp is has same vertex as this");
			valid = false;
		}
		if (opp != null && face == opp.face){
			Debug.LogWarning("opp is has same face as this");
			valid = false;
		}
		if (prev.next != this){
			Debug.LogWarning("prev.next is different from this");
			valid = false;
		}
		if (next.prev != this){
			Debug.LogWarning("next.prev is different from this");
			valid = false;
		}

		return valid;
	}
}
