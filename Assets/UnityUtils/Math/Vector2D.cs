/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

using UnityEngine;


public struct Vector2D {
	public double x;
	public double y;
	
	public Vector2D(Vector2 vector){
		x = vector.x;
		y = vector.y;
	}
	
	public Vector2D(double x,	double y){
		this.x = x;
		this.y = y;
	}

	public Vector2 ToVector2(){
		return new Vector2((float)x,(float)y);
	}
	
	public Vector2D Copy(){
		return new Vector2D(x,y);
	}
	
	public Vector2D Rotate(double radians) {
		double sin = System.Math.Sin(radians);
		double cos = System.Math.Cos(radians);

		Vector2D v = this;
		
		double tx = v.x;
		double ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}
	
	public double this[int i]
	{
		get { return i==0?x:y; }
		set {
			switch (i){
			case 0:
				x = value;
				break;
			case 1:
				y = value;
				break;
				
			default:
				throw new System.Exception("Out of range "+i);
			}
		}
	}
	
	public bool Equals(Vector2D p) {
		// If parameter is null return false:
		if ((object)p == null)
		{
			return false;
		}
		
		// Return true if the fields match:
		return (x == p.x) && (y == p.y);
	}
	
	public static Vector2D operator +(Vector2D c1, Vector2D c2) {
		return new Vector2D(c1.x + c2.x, c1.y + c2.y);
	}
	
	public static Vector2D operator +(Vector2D c1, Vector2 c2) {
		return new Vector2D(c1.x + c2.x, c1.y + c2.y);
	}
	
	public static Vector2D operator -(Vector2D c1, Vector2D c2) {
		return new Vector2D(c1.x - c2.x, c1.y - c2.y);
	}
	
	public static Vector2D operator -(Vector2D c1, Vector2 c2) {
		return new Vector2D(c1.x - c2.x, c1.y - c2.y);
	}
	
	public static Vector2D operator /(Vector2D c1, double i) {
		return new Vector2D(c1.x / i, c1.y / i);
	}
		
	public static Vector2D operator *(Vector2D c1, double i) {
		return new Vector2D(c1.x * i, c1.y * i);
	}

	public static Vector2D operator *(double i, Vector2D c1) {
		return new Vector2D(c1.x * i, c1.y * i);
	}

	public static Vector2D operator *(Vector2D c1, Vector2 c2) {
		return new Vector2D(c1.x * c2.x, c1.y * c2.y);
	}
	
	
	public static Vector2D operator - (Vector2D c1){
		return new Vector2D(-c1.x, -c1.y);
	}
	
	public static bool operator ==(Vector2D c1, Vector2D c2) {
		return c1.x == c2.x && c1.y == c2.y;
	}
	
	public static bool operator !=(Vector2D c1, Vector2D c2) {
		return !(c1 == c2);
	}
	
	public override int GetHashCode()
	{
		return (x + y).GetHashCode();
	}
	/*
	public Vec2d Clamp(Vec2d min, Vec2d max){

		return new Vec2d(Mathf.Clamp(x, min.x, max.x),
		                 Mathf.Clamp(y, min.y, max.y));
	}*/
	
	public static Vector2D one{
		get {
			return new Vector2D(1,1);
		}
	}
	
	public static Vector2D zero{
		get {
			return new Vector2D(0,0);
		}
	}
	
	public static double Dot(Vector2D v1, Vector2D v2){
		return v1.x * v2.x + v1.y * v2.y;
	}

	public static double Distance (Vector2D v1, Vector2D v2){
		return (v1 - v2).magnitude;
	}


	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(Vector2D))
			return false;
		Vector2D other = (Vector2D)obj;
		return x == other.x && y == other.y;
	}


	//
	// Properties
	//
	public double magnitude
	{
		get
		{

			return System.Math.Sqrt (this.x * this.x + this.y * this.y);
		}
	}

	public double sqrMagnitude
	{
		get
		{
			return this.x * this.x + this.y * this.y;
		}
	}
	
	public override string ToString()
	{
		return "["+x+","+y+"]";
	}

    // http://stackoverflow.com/a/1501725/420250
    public static double LineSegmentPointDistance(Vector2D v, Vector2D w, Vector2D p) {
        // Return minimum distance between line segment vw and point p
        double l2 = (v-w).sqrMagnitude;  // i.e. |w-v|^2 -  avoid a sqrt
        if (l2 == 0.0) return Vector2D.Distance(p, v);   // v == w case
        // Consider the line extending the segment, parameterized as v + t (w - v).
        // We find projection of point p onto the line.
        // It falls where t = [(p-v) . (w-v)] / |w-v|^2
        // We clamp t from [0,1] to handle points outside the segment vw.
        double t = System.Math.Max(0.0, System.Math.Min(1.0, Vector2D.Dot(p - v, w - v) / l2));
        Vector2D projection = v + t * (w - v);  // Projection falls on the segment
        return Vector2D.Distance(p, projection);
    }
}