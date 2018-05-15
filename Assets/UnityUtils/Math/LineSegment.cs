/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

using UnityEngine;
//using MB.Algodat;

public class LineSegment  {
	public Vector3D first{
		get{ 
			return array [0];
		}
	}
	public Vector3D second{
		get {
			return array[1];
		}
	}
	Vector3D[] array = new Vector3D[2];
	public bool flag = false;

	public LineSegment(Vector3D first_, Vector3D second_){
		array[0] = first_;
		array[1] = second_;
		//Range = new Range<double>(System.Math.Min(first_.x, second_.x), System.Math.Max(first_.x, second_.x));
	}

	public double Length(){
		return (first - second).magnitude;
	}

	public bool IsSameDirection(LineSegment other, double thresholdSqr = 0.002*0.002){
		Vector3D dirThis = (second - first);
		Vector3D dirOther = (other.second - other.first);
		
		return Vector3D.IsParallel(dirThis, dirOther)
		       && Vector3D.Dot (dirThis, dirOther) >= 0.99f; // this number just have to be close to one since we are already testing for parallelism
	}

    /**
    * Return distance from first point
    */
    public bool Intersects(Vector3D point, out double distance)
    {
        Vector3D dirThis = (second - first).normalized;
        Vector3D dirOther = (point - first);

        if (dirOther.sqrMagnitude < 1E-10)
        {
            distance = 0;
            return true;
        }

        distance = Vector3D.Dot(dirThis, dirOther);
        return Vector3D.IsParallel((second - first), dirOther);
    }

    // http://www.realtimerendering.com/intersections.html (ay/ray: (after Goldman, Graphics Gems)
    public bool FindIntersections(LineSegment other, out Vector3D pos1, out Vector3D pos2, out bool parallel)
    {
        // L1(t1) = o1 + d1*t1
        // L2(t2) = o2 + d2*t2
        // The solution is:
        // t1 = Determinant{(o2-o1),d2,d1 X d2} / ||d1 X d2||^2
        // and
        // t2 = Determinant{(o2-o1),d1,d1 X d2} / ||d1 X d2||^2
        //
        // If the lines are parallel, the denominator ||d1 X d2||^2 is 0.
        Vector3D o1 = first;
        Vector3D o2 = other.first;
        Vector3D d1 = (second - first).normalized;
        Vector3D d2 = (other.second - other.first).normalized;
        double denominator = Vector3D.Cross(d1, d2).sqrMagnitude;
        if (Mathf.Approximately((float) denominator, 0))
        {
            parallel = true;
            pos1 = Vector3D.zero;
            pos2 = Vector3D.zero;
            return false;
        }
        var mat1 = new Matrix3x3D(o2 - o1, d2, Vector3D.Cross(d1, d2));
        var mat2 = new Matrix3x3D(o2 - o1, d1, Vector3D.Cross(d1, d2));
        double det1 = mat1.determinant;
        double det2 = mat2.determinant;
        double t1 = det1 / denominator;
        double t2 = det2 / denominator;
        pos1 = o1 + d1 * t1;
        pos2 = o2 + d2 * t2;
        parallel = false;
        return Mathf.Approximately((float)pos1.x, (float)pos2.x) && 
               Mathf.Approximately((float)pos1.y, (float)pos2.y) &&
               Mathf.Approximately((float)pos1.z, (float)pos2.z) && 
               t1 >=0 && 
               t2 >=0 &&
               t1*t1 <= (second - first).sqrMagnitude &&  
               t2*t2 <= (other.second - other.first).sqrMagnitude;

    }

    /*public Range<double> Range
	{
		get;
		set;
	}*/

	Vector3D ProjectPointLine(Vector3D point, Vector3D lineStart, Vector3D lineEnd)
	{
		Vector3D rhs = point - lineStart;
		Vector3D vector3 = lineEnd - lineStart;
		double magnitude2 = vector3.sqrMagnitude;
		Vector3D lhs = vector3;
		if ( magnitude2 < 9.99999997475243E-07)
		{
			return lineStart; // line has zero magnitude - just return start position
		}

		double dotProduct = Vector3D.Dot(lhs, rhs);
		double num = System.Math.Min(System.Math.Max(dotProduct, 0.0), magnitude2);
		return lineStart + lhs * num/magnitude2;
	}

	double DistancePointLine(Vector3D point, Vector3D lineStart, Vector3D lineEnd)
	{
		return Vector3D.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
	}

	public Vector3D ProjectPoint(Vector3D point)
	{
		return ProjectPointLine(point, first, second);
	}

	public double DistancePoint(Vector3D point)
	{
		return DistancePointLine(point, first, second);
	}

	public LineSegment2D To2D(int skip = 1){
		if (skip == 1) {
			return new LineSegment2D (new Vector2D (first.x, first.z), new Vector2D (second.x, second.z));
		}
		Debug.LogError("Not supported");;
		return null;
	}
}
