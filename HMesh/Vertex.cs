/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vertex : System.IEquatable<Vertex> {
	public Halfedge halfedge;
	public Vector3 position;
	public Vector3 uv1;
	public Vector3 uv2;

	public int label;

	public readonly HMesh hmesh;

	// should only be called by HMesh.CreateVertex
	public Vertex(HMesh hmesh){
		this.hmesh = hmesh;
		position = Vector3.zero;
		uv1 = Vector2.zero;
		uv2 = Vector2.zero;
	}

	public override bool Equals (object obj)
	{
		return this == obj;
	}

	// Replaces this vertex with another
	// does not destroy any vertex
	public void ReplaceVertex(Vertex vertex){
		Debug.Assert(vertex.hmesh == hmesh);
		foreach (var he in CirculateOpp()){
			he.vert = vertex;
			vertex.halfedge = he.next;
		}
	}

	public bool IsBoundary(){
		Halfedge iter = halfedge;
		bool first = true;
		while (iter != null && (iter != halfedge || first == true)){
			first = false;
			// goto next out-going halfedge
			iter = iter.opp;
			if (iter != null){
				iter = iter.next;
			}
		} 
		return iter == null;
	}

	// Circulate in-going halfedges
	public List<Halfedge> CirculateOpp(){
		List<Halfedge> res = new List<Halfedge>();
		Halfedge iter = halfedge.prev;
		bool first = true;
		while (iter != null && (iter != halfedge || first == true)){
			res.Add(iter);
			first = false;
			// goto next out-going halfedge
			iter = iter.opp;
			if (iter != null){
				iter = iter.prev;
			}
		}

		bool meetBoundary = iter == null;
		if (meetBoundary){
			// circulate other way
			iter = halfedge.opp;
			while (iter != null){
				res.Insert(0,iter);
				iter = iter.next;
				if (iter != null){
					iter = iter.opp;
				}
			}
		}
		return res;
	}

	// Circulate out-going halfedges
	public List<Halfedge> Circulate(){
		List<Halfedge> res = new List<Halfedge>();

		Halfedge iter = halfedge;
		bool first = true;
		while (iter != null && (iter != halfedge || first == true)){
			res.Add(iter);
			first = false;
			// goto next out-going halfedge
			iter = iter.opp;
			if (iter != null){
				iter = iter.next;
			}
		} 


		bool meetBoundary = iter == null;
		if (meetBoundary){
			// circulate other way
			iter = halfedge.prev.opp;
			while (iter != null){
				res.Insert(0,iter);
				iter = iter.prev;
				if (iter != null){
					iter = iter.opp;
				}
			}
		}
		return res;
	}

	public List<Halfedge> CirculateOneRing(){
		List<Halfedge> res = new List<Halfedge>();
		if (IsBoundary()){
			Debug.LogWarning("Cannot iterate one ring of vertex when boundary vertex");
			return res;
		}
		Halfedge iter = halfedge.next;
		Halfedge firstHE = iter;
		bool first = true;
		while (iter != firstHE || first){
			res.Add(iter);
			iter = iter.next.opp.next;
			first = false;
		}
		return res;
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	bool System.IEquatable<Vertex>.Equals(Vertex obj){
		return obj == this;
	}

	public bool IsValid(){
		bool valid = true;
		if (halfedge == null){
			Debug.LogWarning("Halfedge is null");
			valid = false;
			return valid;
		}

		if (halfedge.prev.vert != this){
			Debug.LogWarning("Vertex is not correctly associated with halfedge.");
			valid = false;
		}
		return valid;
	}
}
