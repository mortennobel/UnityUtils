/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;


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
}