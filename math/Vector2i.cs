/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)


using UnityEngine;

/// <summary>
/// A integer 2D vector class
/// </summary>
[System.Serializable]
public class Vector2i {
	public int x;
	public int y;

	public Vector2i(){}

	public Vector2i(int x,int y){
		this.x = x;
		this.y = y;
	}

	public Vector2i(Vector2 v){
		x = (int)v.x;
		y = (int)v.y;
	}

	public static Vector2i operator +(Vector2i c1,Vector2i c2){
		return new Vector2i(c1.x+c2.x, c1.y+c2.y);
	}

	public static Vector2 operator +(Vector2i c1,Vector2 c2){
		return new Vector3(c1.x+c2.x, c1.y+c2.y);
	}

	public static Vector2 operator +(Vector2 c1,Vector2i c2){
		return new Vector3(c1.x+c2.x, c1.y+c2.y);
	}

	public static Vector2i operator -(Vector2i c1,Vector2i c2){
		return new Vector2i(c1.x-c2.x, c1.y-c2.y);
	}

	public static Vector2i operator *(Vector2i c1,int c2){
		return new Vector2i(c1.x*c2, c1.y*c2);
	}

	public static Vector2 operator *(Vector2i c1,float c2){
		return new Vector2(c1.x*c2, c1.y*c2);
	}

	public static Vector2i operator *(int c1,Vector2i c2){
		return new Vector2i(c1*c2.x, c1*c2.y);
	}

	public static Vector2 operator *(float c1,Vector2i c2){
		return new Vector2(c1*c2.x, c1*c2.y);
	}

	// allow callers to initialize
	public int this[int idx]
	{
		get { return idx==0?x:y; }
		set { 
			switch (idx){
			case 0:
				x = value;
				break;
			default:
				y = value;
				break;
			}
		}
	}

	public Vector2 toVector2(){
		return new Vector2(x,y);
	}
	
	public float magnitude{
		get { return Mathf.Sqrt(x*x+y*y);}
	}
	
	public override bool Equals(System.Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Vector2i p = obj as Vector2i;
        if ((System.Object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y);
    }

    public bool Equals(Vector2i p)
    {
        // If parameter is null return false:
        if ((object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y);
    }
	
	public static bool operator ==(Vector2i a, Vector2i b)
	{
	    // If both are null, or both are same instance, return true.
	    if (System.Object.ReferenceEquals(a, b))
	    {
	        return true;
	    }
	
	    // If one is null, but not both, return false.
	    if (((object)a == null) || ((object)b == null))
	    {
	        return false;
	    }
	
	    // Return true if the fields match:
	    return a.x == b.x && a.y == b.y;
	}

	public static bool operator !=(Vector2i a, Vector2i b)
	{
	    return !(a == b);
	}

    public override int GetHashCode()
    {
        return x ^ y;
    }
	
	public override string ToString() {
		return "Vector2i {"+x+", "+y+"}";
	}
	
	
}
