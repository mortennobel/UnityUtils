/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)


using UnityEngine;

/// <summary>
/// A integer 2D vector class
/// </summary>
[System.Serializable]
public class Vector3i {
	public int x;
	public int y;
	public int z;

	public Vector3i(){}

	public Vector3i(int x,int y,int z){
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public static Vector3i Max(Vector3i a, Vector3i b){
		return new Vector3i (Mathf.Max (a.x,b.x), Mathf.Max (a.y,b.y), Mathf.Max (a.z,b.z));
	}

	public static Vector3i Min(Vector3i a, Vector3i b){
		return new Vector3i (Mathf.Min (a.x,b.x), Mathf.Min (a.y,b.y), Mathf.Min (a.z,b.z));
	}

	public static Vector3i operator +(Vector3i c1,Vector3i c2){
		return new Vector3i(c1.x+c2.x, c1.y+c2.y, c1.z+c2.z);
	}

    public static Vector3i operator -(Vector3i c1,Vector3i c2){
		return new Vector3i(c1.x-c2.x, c1.y-c2.y, c1.z-c2.z);
	}

	public static Vector3i operator *(Vector3i c1,int c2){
		return new Vector3i(c1.x*c2, c1.y*c2, c1.z*c2);
	}

    public static Vector3i operator /(Vector3i c1,int c2){
        return new Vector3i(c1.x/c2, c1.y/c2, c1.z/c2);
    }

	public static Vector3i operator *(int c1,Vector3i c2){
		return new Vector3i(c1*c2.x, c1*c2.y, c1*c2.z);
	}

	// allow callers to initialize
	public int this[int idx]
	{
		get { return idx==0?x:(idx==1?y:z); }
		set { 
			switch (idx){
			case 0:
				x = value;
				break;
			    case 1:
            y = value;
				break;
			default:
				z = value;
				break;
			}
		}
	}

		public float magnitude{
		get { return Mathf.Sqrt(x*x+y*y+z*z);}
	}
	
	public override bool Equals(System.Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Vector3i p = obj as Vector3i;
        if ((System.Object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y) && (z == p.z);
    }

    public bool Equals(Vector3i p)
    {
        // If parameter is null return false:
        if ((object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y) && (z == p.z);
    }
	
	public static bool operator ==(Vector3i a, Vector3i b)
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
	    return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static bool operator !=(Vector3i a, Vector3i b)
	{
	    return !(a == b);
	}

    public override int GetHashCode()
    {
        return x ^ y ^ z;
    }
	
	public override string ToString() {
		return "Vector3i {"+x+", "+y+", "+z+"}";
	}
}
