/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitInfo{
	public int polygonLine;
	public int triangleLine;
	public Vector3 intersectionPoint3D;

	public HitInfo (int polygonLine, int triangleLine, Vector3 intersectionPoint3D)
	{
		this.polygonLine = polygonLine;
		this.triangleLine = triangleLine;
		this.intersectionPoint3D = intersectionPoint3D;
	}
};

public class Polygon {

	List<Vector3> vertices = new List<Vector3>();

	public Polygon(){

	}

	public Vector3 ComputeNormal(){
		var v1 = (vertices [1] - vertices [0]).normalized;
		var v2 = (vertices [2] - vertices [0]).normalized;
		return Vector3.Cross (v1, v2);
	}

	public void FlipNormal(){
		var tmp = vertices[1];
		vertices[1] = vertices[2];
		vertices[2] = tmp;
	}

	public Polygon(Vector3[] ps){
		foreach (var p in ps){
			vertices.Add(p);
		}
	}

	public Polygon(Vector3D[] ps){
		foreach (var p in ps){
			vertices.Add(p.ToVector3());
		}
	}

	public static Vector2D V2(Vector3 v){
		return new Vector2D(v.x, v.z);
	}

	public static Vector2D V2(Vector3D v){
		return new Vector2D(v.x, v.z);
	}

	// Join two polygons (assuming that they are adjacent and share an edge)
	public Polygon Join(Polygon other){
		int thisEdgeId = -1;
		int otherEdgeId = -1;
		for (int i=0;i<Length() && thisEdgeId == -1;i++){
			Vector3 thisFrom = Vertex(i);
			Vector3 thisTo = Vertex((i+1)%Length());
			for (int j=0;j<other.Length();j++){
				Vector3 otherFrom = other.Vertex(j);
				Vector3 otherTo = other.Vertex((j+1)%other.Length());
				if ((thisFrom-otherTo).sqrMagnitude < 0.001f && (thisTo-otherFrom).sqrMagnitude < 0.001f){
					thisEdgeId = i;
					otherEdgeId = j;
					break;
				} 
			}
		}
		if (thisEdgeId == -1){
			return null;
		}
		Polygon res = new Polygon();
		// add current points
		for (int i=0;i<Length();i++){
			res.vertices.Add(vertices[(i+thisEdgeId+1)%Length()]);
		}
		otherEdgeId=otherEdgeId+2; // skip current since this equals the last vertex of res 
		// add vertices from other
		for (int i=0;i<other.Length()-2;i++){
			res.vertices.Add(other.vertices[(i+otherEdgeId)%other.Length()]);
		} 

		// remove any edge (where two edges share endpoints but has different directions
		for (int i=0;i<res.Length();i++){
			var thisVert = res.Vertex(i);
			var nextNextVert = res.Vertex((i+2)%res.Length());
			bool almostEqual = (thisVert - nextNextVert).sqrMagnitude < 0.001f;
			if (almostEqual){
				var vects = new int[]{(i+1)%res.Length(),i};
				System.Array.Sort(vects);
				res.vertices.RemoveRange(vects[1],1);
				res.vertices.RemoveRange(vects[0],1);
				i = i+2;
			}
		}

		return res;
	}

	public void AddVertex(Vector3 v){
		vertices.Add(v);
	}

	public Vector3 Vertex(int i){
		return vertices[i];
	}

	public void SetVertex(int i, Vector3 pos){
		vertices [i] = pos;
	}

	public int Length(){
		return vertices.Count;
	}

	public List<HitInfo> IntersectTriangle(Triangle triangle){
		List<HitInfo> res = new List<HitInfo>();
		for (int i=0;i<vertices.Count;i++){
			LineSegment2D l1 = new LineSegment2D(V2(vertices[i]), V2(vertices[(i+1)%vertices.Count]));

			for (int j=0;j<3;j++){
				LineSegment2D l2 = new LineSegment2D(V2(triangle.Vertex(j)), V2(triangle.Vertex((j+1)%3)));
				Vector2D point;
				if (l1.LineIntersect(l2, out point)){
					double distance = (point - V2(vertices[i])).magnitude;
					double segmentLength = l1.Length();
				//if (l1.DoLinesIntersect(l2)){
					double normalizedDistance = distance/segmentLength;
					Vector3D point3D = Vector3D.Lerp (new Vector3D (vertices[i]), new Vector3D (vertices[(i+1)%vertices.Count]), normalizedDistance);
					res.Add(new HitInfo(i,j,point3D.ToVector3()));
				}
				/*Vector2 intersectionPoint;
				if (LineLineIntersection(from,to, tFrom, tTo,out intersectionPoint)){
					float normalizedDistance = Vector3.Dot(to-from, tTo-tFrom);
					Vector3 point3D = Vector3.Lerp (vertices[i], vertices[(i+1)%vertices.Count], normalizedDistance);
					res.Add(new HitInfo(i,j,point3D));
				}*/
			}
		}
		return res;
	}

	// Based on public domain function by Darel Rex Finley, 2006
	// http://alienryderflex.com/intersect/
	public static bool LineLineIntersection(Vector2 l1From, Vector2 l1To, Vector2 l2From,Vector2 l2To, out Vector2 intersection){
		float Ax = l1From.x, Ay = l1From.y;
		float Bx = l1To.x, By = l1To.y;
		float Cx = l2From.x, Cy = l2From.y;
		float Dx = l2To.x, Dy = l2To.y;
		intersection = new Vector2();

		float  distAB, theCos, theSin, newX, ABpos ;
		
		//  Fail if either line is undefined.
		if (Ax==Bx && Ay==By || Cx==Dx && Cy==Dy) return false;
		
		//  (1) Translate the system so that point A is on the origin.
		Bx-=Ax; By-=Ay;
		Cx-=Ax; Cy-=Ay;
		Dx-=Ax; Dy-=Ay;
		
		//  Discover the length of segment A-B.
		distAB=Mathf.Sqrt(Bx*Bx+By*By);
		
		//  (2) Rotate the system so that point B is on the positive X axis.
		theCos=Bx/distAB;
		theSin=By/distAB;
		newX=Cx*theCos+Cy*theSin;
		Cy  =Cy*theCos-Cx*theSin; Cx=newX;
		newX=Dx*theCos+Dy*theSin;
		Dy  =Dy*theCos-Dx*theSin; Dx=newX;
		
		//  Fail if the lines are parallel.
		if (Cy==Dy) return false;
		
		//  (3) Discover the position of the intersection point along line A-B.
		ABpos=Dx+(Cx-Dx)*Dy/(Dy-Cy);

		if (ABpos<0 || ABpos > 1) return false;

		//  (4) Apply the discovered position to line A-B in the original coordinate system.
		intersection = new Vector2 (Ax+ABpos*theCos,
									Ay+ABpos*theSin);
		
		//  Success.
		return true; 
	}

	List<HitInfo> GetHitInfoByPolyIndex(int index, List<HitInfo> infos){

		List<HitInfo> res = new List<HitInfo>();
		foreach (HitInfo info in infos){
			if (info.polygonLine == index){
				res.Add(info);
			}
		}
		Vector3 pos = vertices[index];
		res.Sort(delegate(HitInfo hit1, HitInfo hit2){
			float dist1 = (hit1.intersectionPoint3D - pos).sqrMagnitude;
			float dist2 = (hit2.intersectionPoint3D - pos).sqrMagnitude;
			return dist1 < dist2 ? -1 : 1;
		});
		return res;
	}

	private bool ContainsVertex(Vector3 p){
		for (int i=0;i<vertices.Count;i++){
			if ((p-vertices[i]).sqrMagnitude < 0.0001f){
				return true;
			}
		}
		return false;
	}

	public bool IsPointInside(Vector2 point){
		Vector2D[] farFarAwayPoints = new Vector2D[]{
			new Vector2D(point) + new Vector2D(99999,99998),
			new Vector2D(point) + new Vector2D(-99999,89990),
		};
		LineSegment2D[] lineSegments = new LineSegment2D[] {
			new LineSegment2D(new Vector2D(point), farFarAwayPoints[0]),
			new LineSegment2D(new Vector2D(point), farFarAwayPoints[1]),
		};
		foreach (var ls in lineSegments) {
			int hits = 0;
			for (int i = 0;i < vertices.Count; i++) {
				LineSegment2D l1 = new LineSegment2D(Polygon.V2(vertices[i]), Polygon.V2(vertices[(i+1)%vertices.Count]));
				Vector2D res = Vector2D.zero;
				if (ls.LineIntersect(l1,out res)){
					hits++;
				}
			}
			if (hits % 2 == 1){
				return true;
			}
		}
		return false;
	}
}
