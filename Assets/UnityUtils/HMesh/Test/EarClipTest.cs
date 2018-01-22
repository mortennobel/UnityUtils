using UnityEngine;

public class EarClipTest : MonoBehaviour {
	void OnDrawGizmosSelected () {
		Gizmos.color = Color.green;
		for (int i=0;i<transform.childCount;i++){
			Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild((i+1)%transform.childCount).position); 
		}
	}
}
