using UnityEngine;
using System.Collections.Generic;

/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

// based on https://github.com/prideout/polygon.js/blob/master/js/polygon.js
public class EarClipping : MonoBehaviour {
	// currently discards y value
	public static List<Face> Tesselate(Face face){
		if (face.NoEdges() == 3){
			var r = new List<Face>();
			r.Add(face);
			return r;
		}

		List<Face> newFaces = new List<Face>();
		newFaces.Add(face);
		while (face.NoEdges() > 3){
			var he = FindEar(face);
			if (he==null){
				Debug.LogWarning("Warning - cannot cut ear");
				return newFaces;
			}
			var otherFace = face.Cut(he.prev.vert,he.next.vert);
			newFaces.Add(otherFace);
			if (otherFace.NoEdges()>3){
				face = otherFace;
			}
		}
		return newFaces;
	}

	static Halfedge FindEar(Face face){
		float minAngle = 99999;
		Halfedge minHe = null;
		foreach (var he in face.Circulate()){
			Vector3 from3 = he.prev.vert.position;
			Vector3 this3 = he.vert.position;
			Vector3 next3 = he.next.vert.position;
			Vector2 from2 = new Vector2(from3.x, from3.z);
			Vector2 this2 = new Vector2(this3.x, this3.z);
			Vector2 next2 = new Vector2(next3.x, next3.z);
			Vector2 from = from2 - this2;
			Vector2 to = next2 - this2;
			if (Vector3.Cross(from, to).z < 0){
				bool valid = he.prev.vert != he.next.vert; // special case where two halfedges are extruded into a face from a single vertex
				Halfedge h = he.next.next;
				while (h != he.prev && valid){
					Vector2 p = new Vector2(h.vert.position.x,h.vert.position.z);
					if (PointInTriangle(p, from2, this2, next2)){
						valid = false;
					}
					h = h.next;
				}

				if (valid){
					var angle = Vector2.Angle(from, to);
					if (angle < minAngle){
						minAngle = angle;
						minHe = he;
					}
				}
			}
		}
		return minHe;
	}

	public static bool PointInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
	{
		var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
		var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

		if ((s < 0) != (t < 0))
			return false;

		var A = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;
		if (A < 0.0)
		{
			s = -s;
			t = -t;
			A = -A;
		}
		return s > 0 && t > 0 && (s + t) <= A;
	}
}
