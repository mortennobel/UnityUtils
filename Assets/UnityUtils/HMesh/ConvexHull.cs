using UnityEngine;
using System.Collections.Generic;

/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

// Find Convex Hull in 2d (using only x and y)
public class ConvexHull {
	// Aka Jarvis march
	public static List<Vector3D> GiftWrapping(List<Vector3D> positions) {
		// Find leftmost point
		int leftMostPoint = 0;
		for (int i=1;i<positions.Count;i++){
			if (positions[i].x < positions[leftMostPoint].x){
				leftMostPoint = i;
			}
		}
		int pointOnHull = leftMostPoint;
 		int endPoint = 0;
		List<Vector3D> res = new List<Vector3D>();
		do {
			res.Add(positions[pointOnHull]);
			endPoint = pointOnHull==0?1:0;
			for (int j = 0;j<positions.Count;j++){
				if (j != pointOnHull && HMeshMath.LeftOfXY(positions[pointOnHull], positions[endPoint], positions[j])){
					endPoint = j;
				} 
			}
			pointOnHull = endPoint;
		} while (endPoint != leftMostPoint);
		return res;
	}

// Aka Jarvis march
	public static List<Vector3D> GiftWrappingXZ(List<Vector3D> positions) {
		// Find leftmost point
		int leftMostPoint = 0;
		for (int i=1;i<positions.Count;i++){
			if (positions[i].x < positions[leftMostPoint].x){
				leftMostPoint = i;
			}
		}
		int pointOnHull = leftMostPoint;
		int endPoint = 0;
		List<Vector3D> res = new List<Vector3D>();
		do {
			res.Add(positions[pointOnHull]);
			endPoint = pointOnHull==0?1:0;
			for (int j = 0;j<positions.Count;j++){
				if (j != pointOnHull && HMeshMath.LeftOfXZ(positions[pointOnHull], positions[endPoint], positions[j])){
					endPoint = j;
				} 
			}
			pointOnHull = endPoint;
		} while (endPoint != leftMostPoint);
		return res;
	}

	public static List<Vector3D> AklToussaintHeuristic(List<Vector3D> positions){
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

		/*Gizmos.color = Color.green;
		for (int i=0;i<points.Count;i++){
			Gizmos.DrawLine((positions[points[i]]+Vector3D.one*0.01f).ToVector3(), (positions[points[(i+1)%points.Count]]+Vector3D.one*0.01f).ToVector3());
		}*/

		List<Vector3D> res = new List<Vector3D>();

		for (int i=0;i<positions.Count;i++){
			if (points.Contains(i)){
				res.Add(positions[i]);
			} else {
				bool inside = true;
				for (int j=0;j<points.Count && inside;j++){
					inside = HMeshMath.LeftOfXY(positions[points[j]], positions[points[(j+1)%points.Count]], positions[i]);
				}
				if (!inside){
					res.Add(positions[i]);
				} 
			}
		}

		return res;
	}
	
	public static List<Vector3D> AklToussaintHeuristicXZ(List<Vector3D> positions){
		int maxX = 0, maxZ = 0, minX = 0, minZ = 0;


		for (int i=0;i<positions.Count;i++){
			if (positions[i].x < positions[minX].x){
				minX = i;
			}
			if (positions[i].x > positions[maxX].x){
				maxX = i;
			}	
			if (positions[i].y < positions[minZ].z){
				minZ = i;
			}
			if (positions[i].y > positions[maxZ].z){
				maxZ = i;
			}
		}
		List<int> points = new List<int>();
		points.Add(minX);
		if (!points.Contains(minZ)){
			points.Add(minZ);
		}
		if (!points.Contains(maxX)){	
			points.Add(maxX);
		}
		if (!points.Contains(maxZ)){	
			points.Add(maxZ);
		}

		if (points.Count<=2){
			return positions;
		}

		/*Gizmos.color = Color.green;
		for (int i=0;i<points.Count;i++){
			Gizmos.DrawLine((positions[points[i]]+Vector3D.one*0.01f).ToVector3(), (positions[points[(i+1)%points.Count]]+Vector3D.one*0.01f).ToVector3());
		}*/

		List<Vector3D> res = new List<Vector3D>();

		for (int i=0;i<positions.Count;i++){
			if (points.Contains(i)){
				res.Add(positions[i]);
			} else {
				bool inside = true;
				for (int j=0;j<points.Count && inside;j++){
					inside = HMeshMath.LeftOfXZ(positions[points[j]], positions[points[(j+1)%points.Count]], positions[i]);
				}
				if (!inside){
					res.Add(positions[i]);
				} 
			}
		}

		return res;
	}
}
