using UnityEngine;

/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

public class EarClipTest : MonoBehaviour {
	void OnDrawGizmosSelected () {
		Gizmos.color = Color.green;
		for (int i=0;i<transform.childCount;i++){
			Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild((i+1)%transform.childCount).position); 
		}
	}
}
