using UnityEngine;

public static class Vector3Ext {
    // rotate a point arount a line (defined by a pivot point and a direction)
	public static Vector3 RotateAround(this Vector3 inputPoint, Vector3 pivotPoint, Vector3 direction, float degrees) {

	    if (degrees == 0)
	    {
	        return inputPoint;
	    }
	    inputPoint = inputPoint - pivotPoint;

	    inputPoint = Quaternion.AngleAxis(degrees, direction) * inputPoint;

	    inputPoint = inputPoint + pivotPoint;
	    return inputPoint;
	}


    // Create an orthonormal basis given a normal
    // http://jcgt.org/published/0006/01/01/
    public static void BranchlessONB(this Vector3 n, out Vector3 b1, out Vector3 b2)
    {
        float sign = Mathf.Sign( n.z);
        float a = -1.0f / (sign + n.z);
        float b = n.x * n.y * a;
        b1 = new Vector3(1.0f + sign * n.x * n.x * a, sign * b, -sign * n.x);
        b2 = new Vector3(b, sign + n.y * n.y * a, -n.y);
    }
}
