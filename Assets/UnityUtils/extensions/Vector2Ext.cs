using UnityEngine;

public static class Vector2Ext {
	public static Vector2 Rotate(this Vector2 v, float radians) {
		float sin = Mathf.Sin(radians);
		float cos = Mathf.Cos(radians);
		Vector2 res = v;
		float tx = res.x;
		float ty = res.y;
		res.x = (cos * tx) - (sin * ty);
		res.y = (sin * tx) + (cos * ty);
		return res;
	}
	

}
