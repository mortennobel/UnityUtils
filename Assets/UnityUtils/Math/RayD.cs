using UnityEngine;

public class RayD {
	public Vector3D origin;
	public Vector3D direction;

	public RayD(Vector3D origin, Vector3D direction){
		this.origin = origin;
		this.direction = direction;
	}


    public Vector3D GetPointAt(double d)
    {
	    return origin + direction * d;
    }

	// return the distance to the line
	public double LinePointDistance(Vector3D p)
	{
		if (Vector3D.Distance(p, origin) < Mathf.Epsilon)
		{
			return 0.0;
		}
		// Notation from https://math.stackexchange.com/a/1300565
		var p1 = origin;
		var p2 = origin + direction;
		var resVector = (p - p1) - (Vector3D.Dot(p - p1, p2 - p1) / (p2 - p1).sqrMagnitude) * (p2 - p1);
		return resVector.magnitude;
	}
}
