using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelaunayTriangulation2D {
	public static HMesh Triangulate (List<Vector3> positions) {
		HMesh mesh = new HMesh();
		Stack<Halfedge> flipStack = new Stack<Halfedge>();

		List<Vertex> boundingVertices = CreateBoundingTriangle(mesh, positions);
		foreach (var pos in positions) {
			Face triangle = FindTriangleWithinPoint(mesh, pos);
			AddEdgesToFlipStack(triangle, flipStack);
			InsertPointInTriangle(triangle, pos);
			while (flipStack.Count>0){
				var e = flipStack.Pop();
				if (!IsLocalDelaunay(e)){
					var opp = e.opp;
					var check1 = opp.next;
					var check2 = check1.next;
					flipStack.Push(check1);
					flipStack.Push(check2);
					e.Flip();
				}
			}
		}
		return mesh;
	}

	static void FlipEdge(Halfedge e, Stack<Halfedge> flipStack){
		e.Flip();
	}

	static bool PrecondFlipEdge(HMesh mesh, Halfedge he){
		return true;
	}

	static bool IsLocalDelaunay(Halfedge e){
		if (e.IsBoundary()){
			return true;
		}
		Vector3 p1 = e.vert.position;
		Vector3 p2 = e.next.vert.position;
		Vector3 p3 = e.next.next.vert.position;
		Vector3 p4 = e.opp.next.vert.position;

		return HMeshMath.InCircle(p1,p2,p3,p4) || HMeshMath.InCircle(p2,p1,p4,p2);
	}



	static void InsertPointInTriangle(Face triangle, Vector3 point){
		triangle.Split().position = point;
	}

	static List<Vertex> CreateBoundingTriangle(HMesh mesh, List<Vector3> position){
		
		Bounds b = new Bounds(position[0], Vector3.zero);
		for (int i=0;i<position.Count;i++){
			b.Encapsulate(position[i]);
		}
		// encapsulate triangle
		b.center = b.center + b.extents*1.25f;
		b.extents = b.extents*40.0f;
		Vector3 v1 = b.min;
		Vector3 v2 = b.min + Vector3.right * b.size.x;
		Vector3 v3 = b.min + Vector3.up * b.size.y;
		Face face = mesh.CreateTriangle(v1, v2, v3);
		List<Vertex> boundingVertices = new List<Vertex>();
		foreach (var he in face.Circulate()){
			Gizmos.DrawLine(he.vert.position, he.next.vert.position);
			boundingVertices.Add(he.vert);
		}
		return boundingVertices;
	}

	static void AddEdgesToFlipStack(Face f, Stack<Halfedge> flipStack){
		foreach (var he in f.Circulate()){
			flipStack.Push(he);
		}
	}

	static Face FindTriangleWithinPoint(HMesh mesh, Vector3 pos){
		foreach (var face in mesh.GetFaces()){
			var edges = face.Circulate();
			var p1 = edges[0].vert.position;
			var p2 = edges[1].vert.position;
			var p3 = edges[2].vert.position;
			Debug.LogWarning("Points "+p1+", "+p2+", "+p3);
			if (HMeshMath.LeftOf(p1,p2,pos) && HMeshMath.LeftOf(p2,p3,pos) && HMeshMath.LeftOf(p3,p1,pos)){
				return face;
			}
		}
		Debug.LogWarning("Cannot find triangle");
		return null;
	}


}
