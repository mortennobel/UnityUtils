using UnityEngine;
using System.Collections.Generic;

/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

// http://stackoverflow.com/questions/15281616/take-slice-from-2d-array-int-using-linq-in-c-sharp
public static class Ext
{
	public static string NameFull(this GameObject go){
		string name = go.name;
		while (go.transform.parent) {
			go = go.transform.parent.gameObject;
			name = go.name + "/" + name;
		}
		return name;
	}

	// warning - will also find prefab objects
	public static GameObject[] FindAllObjectsWithTag(string tag){
		List<GameObject> res = new List<GameObject> ();
		GameObject[] gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
		foreach (var go in gameObjects) {
			if (go.tag == tag) {
				if (!res.Contains (go)) {
					res.Add (go);
				}
			}
		}
		return res.ToArray ();
	} 

	// warning - will also find prefab objects
	public static GameObject[] FindInactiveObjectsWithTag(string tag){
		List<GameObject> res = new List<GameObject> ();
		GameObject[] gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
		foreach (var go in gameObjects) {
			if (go.tag == tag && !go.activeSelf) {
				if (!res.Contains (go)) {
					res.Add (go);
				}
			}
		}
		return res.ToArray ();
	} 

	public static T[] Slice<T>(this T[] source, int fromIdx, int toIdx)
	{
		T[] ret = new T[toIdx - fromIdx + 1];
		for(int srcIdx=fromIdx, dstIdx = 0; srcIdx <= toIdx; srcIdx++)
		{
			ret[dstIdx++] = source[srcIdx];
		}
		return ret;
	}
	public static T[,] Slice<T>(this T[,] source, int fromIdxRank0, int toIdxRank0, int fromIdxRank1, int toIdxRank1)
	{
		T[,] ret = new T[toIdxRank0 - fromIdxRank0 + 1, toIdxRank1 - fromIdxRank1 + 1];
		
		for(int srcIdxRank0=fromIdxRank0, dstIdxRank0 = 0; srcIdxRank0 <= toIdxRank0; srcIdxRank0++, dstIdxRank0++)
		{        
			for(int srcIdxRank1=fromIdxRank1, dstIdxRank1 = 0; srcIdxRank1 <= toIdxRank1; srcIdxRank1++, dstIdxRank1++)
			{
				ret[dstIdxRank0, dstIdxRank1] = source[srcIdxRank0, srcIdxRank1];
			}
		}
		return ret;
	}

	public static void DestroyAllChildred(this GameObject go){
		var children = new List<GameObject>();
		foreach (Transform child in go.transform) children.Add(child.gameObject);
		children.ForEach(child => GameObject.DestroyImmediate(child));
	}

	public static double DotD (Vector3 lhs, Vector3 rhs)
	{
		return lhs.x * (double)rhs.x + lhs.y * (double)rhs.y + lhs.z * (double)rhs.z;
	}


	public static bool RaycastD (this Plane plane, Ray ray, out double enter)
	{
		double num = DotD (ray.direction, plane.normal);
		double num2 = -DotD (ray.origin, plane.normal) - plane.distance;
		if (Mathf.Approximately ((float)num, 0f))
		{
			enter = 0f;
			return false;
		}
		enter = num2 / num;
		return enter > 0f;
	}

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}