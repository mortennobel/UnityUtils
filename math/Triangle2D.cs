/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

// A triangle but stores the x range in Range
public class Triangle2D {
	Vector2D[] points;

	public Triangle2D(params Vector2D[] ps){
		points = new Vector2D[3];
		double min = double.MaxValue;
		double max = double.MinValue;
		for (int i=0;i<ps.Length && i < 3;i++){
			points[i] = ps[i];
			min = System.Math.Min(min, ps[i].x);
			max = System.Math.Max(max, ps[i].x);
		}
		if (ps.Length==0){
			min = 0;
			max = 0;
		}

	}

	// skip Y!!!
	public Triangle2D(params Vector3D[] ps){
		points = new Vector2D[3];
		double min = double.MaxValue;
		double max = double.MinValue;
		for (int i=0;i<ps.Length && i < 3;i++){
			points[i] = new Vector2D(ps[i].x, ps[i].z);
			min = System.Math.Min(min, ps[i].x);
			max = System.Math.Max(max, ps[i].x);
		}
		if (ps.Length==0){
			min = 0;
			max = 0;
		}

	}


		
	// if the line segment is inside or is intersecting with the triangle then cut
	// otherwise return this as an array
	public Triangle2D[] Cut(LineSegment2D line, double minDist = 0.1f){
		
		if (IsPointInside (line.first) || IsPointInside (line.second) || IsIntersecting (line)) {
			Ray2DD ray = new Ray2DD (line.first, line.second - line.first);
			Vector2D[] collisions = new Vector2D[3];
			LineSegment2D[] segments = GetLineSegments ();
			bool[] inside = { 
				IsPointInside(line.first),
				IsPointInside(line.second)
			};

			bool onlyOnePointInside  = inside [0] ^ inside [1];
			if (onlyOnePointInside) {
				Vector2D newPoint = new Vector2D ();
				for (int i = 0; i < segments.Length; i++) {
					if (segments[i].LineIntersect(line, out newPoint)) {
						// if minimum distance is too small return this
						if (Vector2D.Distance (newPoint, points [i]) < minDist ||
						    Vector2D.Distance (newPoint, points [(i + 1 + 3) % 3]) < minDist) {
							return new Triangle2D[]{ this };
						}
						return new Triangle2D[]{ 
						 	new Triangle2D(points[i], newPoint,points[(i-1+3)%3]),
							new Triangle2D(newPoint,points[(i+1+3)%3],points[(i+2+3)%3])
						};

					}
				}
			}

			bool[] intersection = new bool[3];
			double[] minDistPerCollision = new double[3];
			int minDisCount = 0;
			for (int i = 0; i < 3; i++) {
				intersection [i] = segments [i].RayIntersect (ray, out collisions [i]);
				if (!intersection[i]) {
					minDistPerCollision[i] = double.MaxValue;
				} else {
					minDistPerCollision [i] = System.Math.Min (Vector2D.Distance (segments [i].first, collisions [i]),
						Vector2D.Distance (segments [i].second, collisions [i]));
					if (minDistPerCollision [i] < minDist) {
						minDisCount++;
					}
				}
			}

			int collisionCount = 0;
			foreach (var i in intersection) {
				if (i) {
					collisionCount++;
				}
			}
			if (collisionCount != 2 || minDisCount>=2) {
				return new Triangle2D[]{  this};	
			}
			if (minDisCount == 1) {
				for (int i = 0; i < 3; i++) {
					if (intersection [i] && minDistPerCollision [i] >= minDist) {
						return new Triangle2D[]{ 
							new Triangle2D(points[i], collisions[i],points[(i-1+3)%3]),
							new Triangle2D(points[(i+1)%3],points[(i+2)%3],collisions[i])
						};
					}
				}
			}

			int cutOffCorner = intersection[2]==false?1:(intersection[1]==false?0:2);



			return new Triangle2D[]{ 
				new Triangle2D(points[cutOffCorner], collisions[cutOffCorner],collisions[(cutOffCorner-1+3)%3]),
				new Triangle2D(collisions[cutOffCorner],points[(cutOffCorner+1)%3],collisions[(cutOffCorner-1+3)%3]),
				new Triangle2D(points[(cutOffCorner+1)%3],points[(cutOffCorner+2)%3],collisions[(cutOffCorner-1+3)%3]),
			};	
		} else {
			return new Triangle2D[]{this};	
		}
	}

	public Triangle To3D(){
		return new Triangle (new Vector3D(points [0].x, 0, points [0].y),
			new Vector3D(points [1].x, 0, points [1].y),
			new Vector3D(points [2].x, 0, points [2].y));
	}


	public Vector2D Center{
		get {
			return (points[0] + points[1] + points[2])*(1/3.0);
		}
	}

	public Vector2D Vertex(int i){
		return points[i];
	}

	public Vector3D Vertex3D(int i){
		return new Vector3D(points[i].x, 0, points[i].y);
	}

	double sign (Vector2D p1, Vector2D p2, Vector2D p3)
	{
		return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
	}


	public bool IsPointInside(Vector2D pt){
		Vector2D v1 = points [0];
		Vector2D v2 = points [1];
		Vector2D v3 = points [2];
		bool b1, b2, b3;

		b1 = sign(pt, v1, v2) < 0.0f;
		b2 = sign(pt, v2, v3) < 0.0f;
		b3 = sign(pt, v3, v1) < 0.0f;

		return ((b1 == b2) && (b2 == b3));
	}


	public LineSegment2D[] GetLineSegments(){
		LineSegment2D[] res = new LineSegment2D[3];
		for (int i=0;i<3;i++){
			res[i] = new LineSegment2D(new Vector2D(points[i].x,points[i].y), 
				new Vector2D(points[(i+1)%3].x,points[(i+1)%3].y));
		}
		return res;
	}

	public bool IsIntersecting(LineSegment2D ls, out Vector2D p){
		foreach (var l in GetLineSegments()){
			if (ls.LineIntersect(l, out p)) {
				return true;		
			}
		}
		p = Vector2D.zero;
		return false;
	}

	public bool IsIntersecting(LineSegment2D ls){
		foreach (var l in GetLineSegments()){
			Vector2D p = new Vector2D ();
			if (ls.LineIntersect(l, out p)) {
				return true;		
			}
		}
		return false;
	}
}
