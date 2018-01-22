using UnityEngine;

// http://martin-thoma.com/how-to-check-if-two-line-segments-intersect/
public class LineSegment2D  {
	public Vector2D first{
		get{ 
			return array [0];
		}
	}
	public Vector2D second{
		get {
			return array[1];
		}
	}
	Vector2D[] array = new Vector2D[2];

	public LineSegment2D(Vector2D first_, Vector2D second_){
		array[0] = first_;
		array[1] = second_;
	}

	public double Length(){
		return (first - second).magnitude;
	}

	public Vector2D Direction(){
		var dir = (second-first);
		var magnitude = dir.magnitude;
		if (magnitude>0){
			dir = dir * (1.0f/magnitude);
		}
		return dir;
	}

	public static bool doesIntersect(double l1x1, double l1y1, double l1x2, double l1y2, double l2x1, double l2y1, double l2x2,
		double l2y2) {
		double denom = ((l2y2 - l2y1) * (l1x2 - l1x1)) - ((l2x2 - l2x1) * (l1y2 - l1y1));
		
		if (denom == 0.0f) {
			return false;
		}
		
		double ua = (((l2x2 - l2x1) * (l1y1 - l2y1)) - ((l2y2 - l2y1) * (l1x1 - l2x1))) / denom;
		double ub = (((l1x2 - l1x1) * (l1y1 - l2y1)) - ((l1y2 - l1y1) * (l1x1 - l2x1))) / denom;
		
		return ((ua >= 0.0f) && (ua <= 1.0f) && (ub >= 0.0f) && (ub <= 1.0f));
	}
	
	public static bool doesIntersect(LineSegment2D ln1, LineSegment2D ln2) {
		double denom = ((ln2.second.y - ln2.first.y) * (ln1.second.x - ln1.first.x))
			- ((ln2.second.x - ln2.first.x) * (ln1.second.y - ln1.first.y));
		
		if (denom == 0.0) {
			return false;
		}
		
		double ua = ((ln2.second.x - ln2.first.x) * (ln1.first.y - ln2.first.y))
			- ((ln2.second.y - ln2.first.y) * (ln1.first.x - ln2.first.x)) / denom;
		double ub = ((ln1.second.x - ln1.first.x) * (ln1.first.y - ln2.first.y))
			- ((ln1.second.y - ln1.first.y) * (ln1.first.x - ln2.first.x)) / denom;
		
		return ((ua >= 0.0f) && (ua <= 1.0f) && (ub >= 0.0f) && (ub <= 1.0f));
	}
	
	public static bool getIntersection(LineSegment2D line, double x1, double y1, double x2, double y2, out Vector2D res) {
		res = Vector2D.zero;
		double denom = ((line.second.y - line.first.y) * (x2 - x1))
			- ((line.second.x - line.first.x) * (y2 - y1));
		double nume_a = ((line.second.x - line.first.x) * (y1 - line.first.y))
			- ((line.second.y - line.first.y) * (x1 - line.first.x));
		double nume_b = ((x2 - x1) * (y1 - line.first.y)) - ((y2 - y1) * (x1 - line.first.x));
		
		if (denom == 0.0f) {
			return false;
		}
		
		double ua = nume_a / denom;
		double ub = nume_b / denom;
		
		if ((ua >= 0.0f) && (ua <= 1.0f) && (ub >= 0.0f) && (ub <= 1.0f)) {
			
			// Get the intersection point.
			double intersectX =  (line.first.x + ua * (line.second.x - line.first.x));
			double intersectY =  (line.first.y + ua * (line.second.y - line.first.y));
			
			res = new Vector2D(intersectX, intersectY);
			return true;
		}
		 
		return false;
	}

	public bool RayIntersect(Ray2DD l2, out Vector2D res) {
		LineSegment2D l1 = this;
		bool b = RayIntersect(l1.first.x, l1.first.y, l1.second.x, l1.second.y, 
			l2.origin.x, l2.origin.y, l2.origin.x + l2.direction.x*1000000, l2.origin.y + l2.direction.y*1000000, out res);
		if (b){
			return (first - res).magnitude + (second - res).magnitude <= (first - second).magnitude + 0.0001f;;
		} 
		return false;
	}

	public bool LineIntersect(LineSegment2D l2, out Vector2D res) {
		LineSegment2D l1 = this;
		return LineIntersect(l1.first.x, l1.first.y, l1.second.x, l1.second.y, 
		                     l2.first.x, l2.first.y, l2.second.x, l2.second.y, out res);
	}

	static bool RayIntersect(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, out Vector2D res) {
		res = Vector2D.zero;
		double denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
		if (denom == 0.0) { // Lines are parallel.
			return false;
		}
		double ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denom;
		double ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denom;

			// Get the intersection point.
			res = new Vector2D ((x1 + ua * (x2 - x1)), (y1 + ua * (y2 - y1)));
			return true;
	}

	public static bool LineIntersect(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, out Vector2D res) {
		res = Vector2D.zero;
	    double denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
		if (System.Math.Abs(denom) < Mathf.Epsilon) { // Lines are approx parallel.
			return false;
		}
		double ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denom;
		double ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denom;
		if (ua >= 0.0 && ua <= 1.0 && ub >= 0.0 && ub <= 1.0) {
			// Get the intersection point.
			res =  new Vector2D((x1 + ua * (x2 - x1)), (y1 + ua * (y2 - y1)));
			return true;
		}
		
		return false;
	}
	
	public double MinimumDistanceSqr(Vector2D point)
	{
		double t;
		return MinimumDistanceSqr(point, out t);
	}

	public double MinimumDistanceSqr(Vector2D point, out double t)
	{
		t = 0;
		// Return minimum distance between line segment vw and point p
		double l2 = (first - second).sqrMagnitude;  // i.e. |w-v|^2 -  avoid a sqrt
		if (l2 == 0.0) return (point - first).magnitude;   // v == w case
		// Consider the line extending the segment, parameterized as v + t (w - v).
		// We find projection of point p onto the line. 
		// It falls where t = [(p-v) . (w-v)] / |w-v|^2
		t = Vector2D.Dot(point- first, second - first) / l2;
		if (t < 0.0) return (point- first).magnitude;       // Beyond the 'v' end of the segment
		else if (t > 1.0) return (point- second).magnitude;  // Beyond the 'w' end of the segment
		var projection = first + t * (second - first);  // Projection falls on the segment
		return (point- projection).sqrMagnitude;
	}

	public double MinimumDistance(Vector2D point)
	{
		double t;
		return System.Math.Sqrt(MinimumDistanceSqr(point, out t));
	}

	public double MinimumDistance(Vector2D point, out double t)
	{
		return System.Math.Sqrt(MinimumDistanceSqr(point, out t));
	}

    public override string ToString()
    {
        return "{"+first+"} - {"+second+"}";
    }
}
