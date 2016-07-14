using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Find Convex Hull in 2d (using only x and y)
public class ConvexHull {
	// Aka Jarvis march
	public static List<Vector3> GiftWrapping(List<Vector3> positions) {
		// Find leftmost point
		int leftMostPoint = 0;
		for (int i=1;i<positions.Count;i++){
			if (positions[i].x < positions[leftMostPoint].x){
				leftMostPoint = i;
			}
		}
		int pointOnHull = leftMostPoint;
		Debug.Log("leftMostPoint "+leftMostPoint);
		int endPoint = 0;
		List<Vector3> res = new List<Vector3>();
		do {
			Debug.Log("Add "+pointOnHull);
			res.Add(positions[pointOnHull]);
			endPoint = pointOnHull==0?1:0;
			for (int j = 0;j<positions.Count;j++){
				if (j != pointOnHull && HMeshMath.LeftOf(positions[pointOnHull], positions[endPoint], positions[j])){
					endPoint = j;
				} 
			}
			pointOnHull = endPoint;
		} while (endPoint != leftMostPoint);
		return res;
	}

	public static List<Vector3> AklToussaintHeuristic(List<Vector3> positions){
		int maxX = 0, maxY = 0, minX = 0, minY = 0;


		for (int i=0;i<positions.Count;i++){
			if (positions[i].x < positions[minX].x){
				minX = i;
			}
			if (positions[i].x > positions[maxX].x){
				maxX = i;
			}	
			if (positions[i].y < positions[minY].y){
				minY = i;
			}
			if (positions[i].y > positions[maxY].y){
				maxY = i;
			}
		}
		List<int> points = new List<int>();
		points.Add(minX);
		if (!points.Contains(minY)){
			points.Add(minY);
		}
		if (!points.Contains(maxX)){	
			points.Add(maxX);
		}
		if (!points.Contains(maxY)){	
			points.Add(maxY);
		}

		if (points.Count<=2){
			return positions;
		}

		Gizmos.color = Color.green;
		for (int i=0;i<points.Count;i++){
			Gizmos.DrawLine(positions[points[i]]+Vector3.one*0.01f, positions[points[(i+1)%points.Count]]+Vector3.one*0.01f); 
		}

		List<Vector3> res = new List<Vector3>();

		for (int i=0;i<positions.Count;i++){
			if (points.Contains(i)){
				res.Add(positions[i]);
			} else {
				bool inside = true;
				for (int j=0;j<points.Count && inside;j++){
					inside = HMeshMath.LeftOf(positions[points[j]], positions[points[(j+1)%points.Count]], positions[i]);
				}
				if (!inside){
					res.Add(positions[i]);
				} 
			}
		}

		return res;
	}
}
