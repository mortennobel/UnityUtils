using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConvexHullTest : MonoBehaviour {
	public Transform[] points;
	public List<Vector3> res = new List<Vector3>();
	public bool forceNew;
	public int c = 10;
	public void CreateRandomPoints(){
		if (points != null && points.Length>0){
			foreach (var p in points){
				DestroyImmediate(p.gameObject);
			}
		}
		points = new Transform[c];
		for (int i=0;i<c;i++){
			var go = new GameObject("Point");
			go.transform.parent = transform;
			go.transform.position = new Vector3(Random.value*10,Random.value*10,0);
			points[i] = go.transform;
			go.AddComponent<ConvexHullTest>();
		}
	}

	// Update is called once per frame
	void OnDrawGizmosSelected() {
		if (transform.parent != null && transform.parent.GetComponent<ConvexHullTest>() != null){
			transform.parent.GetComponent<ConvexHullTest>().OnDrawGizmosSelected();
			return;
		}
		Gizmos.color = Color.white;
		if (points == null || points.Length ==0 || points[0] == null || forceNew){
			// create random points
			CreateRandomPoints();
		}
		List<Vector3> positions = new List<Vector3>();
		foreach (var p in points){
			positions.Add(p.position);
			Gizmos.DrawSphere(p.position,0.1f);
		}

		var culled = ConvexHull.AklToussaintHeuristic (positions);
		foreach (var p in culled){
			Gizmos.color = Color.yellow;	
			Gizmos.DrawSphere(p,0.2f);
		}
		res = ConvexHull.GiftWrapping(culled);
		Gizmos.color = Color.white;	
		for (int i=0;i<res.Count;i++){
			Gizmos.DrawLine(res[i],res[(i+1)%res.Count]);
		}
	}
}
