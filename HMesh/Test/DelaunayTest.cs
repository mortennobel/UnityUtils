using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(HMeshRenderer))]
public class DelaunayTest : MonoBehaviour {
	public Transform[] points;

	public void CreateRandomPoints(){
		if (points != null && points.Length>0){
			foreach (var p in points){
				DestroyImmediate(p);
			}
		}
		int c = 10;
		points = new Transform[c];
		for (int i=0;i<c;i++){
			var go = new GameObject("Point");
			go.transform.parent = transform;
			go.transform.position = new Vector3(Random.value,Random.value,0);
			points[i] = go.transform;
		}
	}

	// Update is called once per frame
	void OnDrawGizmosSelected() {
		if (points == null || points.Length ==0 || points[0] == null){
			// create random points
			CreateRandomPoints();
		}
		List<Vector3> positions = new List<Vector3>();
		foreach (var p in points){
			positions.Add(p.position);
		}
		var mesh = GetComponent<HMeshRenderer>();
		mesh.hmesh = DelaunayTriangulation2D.Triangulate(positions);
	}
}
