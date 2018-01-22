using UnityEngine;
using System;

public struct Vector3D {

	//
	// Static Fields
	//
	public const double kEpsilon = 1E-10f;
	
	//
	// Fields
	//
	public double z;
	
	public double y;
	
	public double x;
	
	//
	// Static Properties
	//
	public static Vector3D back
	{
		get
		{
		    return new Vector3D (0f, 0f, -1f);
		}
	}
	
	public static Vector3D down
	{
		get
		{
			return new Vector3D (0f, -1f, 0f);
		}
	}
	
	public static Vector3D forward
	{
		get
		{
			return new Vector3D (0f, 0f, 1f);
		}
	}
	
	[Obsolete ("Use Vector3D.forward instead.")]
	public static Vector3D fwd
	{
		get
		{
			return new Vector3D (0f, 0f, 1f);
		}
	}
	
	public static Vector3D left
	{
		get
		{
			return new Vector3D (-1f, 0f, 0f);
		}
	}
	
	public static Vector3D one
	{
		get
		{
			return new Vector3D (1f, 1f, 1f);
		}
	}
	
	public static Vector3D right
	{
		get
		{
			return new Vector3D (1f, 0f, 0f);
		}
	}
	
	public static Vector3D up
	{
		get
		{
			return new Vector3D (0f, 1f, 0f);
		}
	}
	
	public static Vector3D zero
	{
		get
		{
			return new Vector3D (0.0, 0.0, 0.0);
		}
	}

	//
	// Properties
	//
	public double magnitude
	{
		get
		{
		    double sqMag = this.sqrMagnitude;
		    if (sqMag == 0.0)
		    {
		        return 0;
		    }
		    return Math.Sqrt (sqMag);
		}
	}
	
	public Vector3D normalized
	{
		get
		{
			return Vector3D.Normalize (this);
		}
	}

	public double sqrMagnitude
	{
		get
		{
			return this.x * this.x + this.y * this.y + this.z * this.z;
		}
	}

    /// <summary>
    /// Compare two vectors using Mathf.Approximatly
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Approximatly(Vector3D other)
    {
        for (int i = 0; i < 3; i++)
        {
            if (!Mathf.Approximately((float)this[i], (float)other[i]))
            {
                return false;
            }
        }
        return true;
    }

    public bool AreSame(Vector3D other, double epsilon = 0.00002f)
    {
        for (int i = 0; i < 3; i++)
        {
            double diff = System.Math.Abs( this[i] - other[i]);
            if (diff > epsilon)
            {
                return false;
            }
        }
        return true;
    }



    static double Clamp(double value, double min, double max)
	{

		return (value < min) ? min : (value > max) ? max : value;  
	}

	static double Clamp01(double value)  
	{
		return Clamp(value, 0, 1);
	}

	//
	// Indexer
	//
	public double this [int index]
	{
		get
		{
			switch (index)
			{
			case 0:
				return this.x;
			case 1:
				return this.y;
			case 2:
				return this.z;
			default:
				throw new IndexOutOfRangeException ("Invalid Vector3D index!");
			}
		}
		set
		{
			switch (index)
			{
			case 0:
				this.x = value;
				break;
			case 1:
				this.y = value;
				break;
			case 2:
				this.z = value;
				break;
			default:
				throw new IndexOutOfRangeException ("Invalid Vector3D index!");
			}
		}
	}
	
	//
	// Constructors
	//
	public Vector3D (double x, double y)
	{
		this.x = x;
		this.y = y;
		this.z = 0f;
	}
	
	public Vector3D (double x, double y, double z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3D (Vector3 v)
	{
		this.x = v.x;
		this.y = v.y;
		this.z = v.z;
	}

	public Vector3 ToVector3(){
		return new Vector3((float)x,(float)y,(float)z);
	}

	
	// Return the angle in degrees between from and to
    public static double Angle(Vector3D from, Vector3D to)
    {
        return Math.Acos(Math.Max(Math.Min(Dot(from.normalized, to.normalized), 1f),-1)) * 57.295779513082321;
    }

    public static Vector3D ClampMagnitude (Vector3D vector, double maxLength)
	{
		if (vector.sqrMagnitude > maxLength * maxLength)
		{
			return vector.normalized * maxLength;
		}
		return vector;
	}
	
	public static Vector3D Cross (Vector3D lhs, Vector3D rhs)
	{
		return new Vector3D (lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
	}
	
	public static double Distance (Vector3D a, Vector3D b)
	{
		Vector3D vector = new Vector3D (a.x - b.x, a.y - b.y, a.z - b.z);
		return Math.Sqrt (vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
	}
	
	public static double Dot (Vector3D lhs, Vector3D rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
	}
		
	public static Vector3D Lerp (Vector3D from, Vector3D to, double t)
	{
		t = Clamp01 (t);
		return new Vector3D (from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
	}
	
	public static double Magnitude (Vector3D a)
	{
		return Math.Sqrt (Dot(a,a));
	}
	
	public static Vector3D Max (Vector3D lhs, Vector3D rhs)
	{
		return new Vector3D (Math.Max (lhs.x, rhs.x), Math.Max (lhs.y, rhs.y), Math.Max (lhs.z, rhs.z));
	}
	
	public static Vector3D Min (Vector3D lhs, Vector3D rhs)
	{
		return new Vector3D (Math.Min (lhs.x, rhs.x), Math.Min (lhs.y, rhs.y), Math.Min (lhs.z, rhs.z));
	}
	
	public static Vector3D MoveTowards (Vector3D current, Vector3D target, double maxDistanceDelta)
	{
		Vector3D a = target - current;
		double magnitude = a.magnitude;
		if (magnitude <= maxDistanceDelta || magnitude == 0f)
		{
			return target;
		}
		return current + a / magnitude * maxDistanceDelta;
	}
	
	public static Vector3D Normalize (Vector3D value)
	{
	    value.Normalize();
	    return value;
	}
	
	/*public static void OrthoNormalize (ref Vector3D normal, ref Vector3D tangent)
	{
		Vector3D.Internal_OrthoNormalize2 (ref normal, ref tangent);
	}
	
	public static void OrthoNormalize (ref Vector3D normal, ref Vector3D tangent, ref Vector3D binormal)
	{
		Vector3D.Internal_OrthoNormalize3 (ref normal, ref tangent, ref binormal);
	}*/
	
	public static Vector3D Project (Vector3D vector, Vector3D onNormal)
	{
		double num = Vector3D.Dot (onNormal, onNormal);
		if (num < Mathf.Epsilon)
		{
			return Vector3D.zero;
		}
		return onNormal * Vector3D.Dot (vector, onNormal) / num;
	}
	
	public static Vector3D ProjectOnPlane (Vector3D vector, Vector3D planeNormal)
	{
		return vector - Vector3D.Project (vector, planeNormal);
	}
	
	public static Vector3D Reflect (Vector3D inDirection, Vector3D inNormal)
	{
		return -2f * Vector3D.Dot (inNormal, inDirection) * inNormal + inDirection;
	}
	
	/*public static Vector3D RotateTowards (Vector3D current, Vector3D target, double maxRadiansDelta, double maxMagnitudeDelta)
	{
		return Vector3D.INTERNAL_CALL_RotateTowards (ref current, ref target, maxRadiansDelta, maxMagnitudeDelta);
	}*/
	
	public static Vector3D Scale (Vector3D a, Vector3D b)
	{
		return new Vector3D (a.x * b.x, a.y * b.y, a.z * b.z);
	}

    public static bool AllLessThan(Vector3D a, Vector3D b)
    {
        return a.x < b.x && a.y < b.y && a.z < b.z;
    }

    public static bool AllLessEqualThan(Vector3D a, Vector3D b)
    {
        return a.x <= b.x && a.y <= b.y && a.z <= b.z;
    }

    /*public static Vector3D Slerp (Vector3D from, Vector3D to, double t)
    {
        return Vector3D.INTERNAL_CALL_Slerp (ref from, ref to, t);
    }*/
	
	/*public static Vector3D SmoothDamp (Vector3D current, Vector3D target, ref Vector3D currentVelocity, double smoothTime, [DefaultValue ("Mathf.Infinity")] double maxSpeed, [DefaultValue ("Time.deltaTime")] double deltaTime)
	{
		smoothTime = Mathf.Max (0.0001f, smoothTime);
		double num = 2f / smoothTime;
		double num2 = num * deltaTime;
		double d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
		Vector3D vector = current - target;
		Vector3D vector2 = target;
		double maxLength = maxSpeed * smoothTime;
		vector = Vector3D.ClampMagnitude (vector, maxLength);
		target = current - vector;
		Vector3D vector3 = (currentVelocity + num * vector) * deltaTime;
		currentVelocity = (currentVelocity - num * vector3) * d;
		Vector3D vector4 = target + (vector + vector3) * d;
		if (Vector3D.Dot (vector2 - current, vector4 - vector2) > 0f)
		{
			vector4 = vector2;
			currentVelocity = (vector4 - vector2) / deltaTime;
		}
		return vector4;
	}*/
	
	/*[ExcludeFromDocs]
	public static Vector3D SmoothDamp (Vector3D current, Vector3D target, ref Vector3D currentVelocity, double smoothTime, double maxSpeed)
	{
		double deltaTime = Time.deltaTime;
		return Vector3D.SmoothDamp (current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	}
	
	[ExcludeFromDocs]
	public static Vector3D SmoothDamp (Vector3D current, Vector3D target, ref Vector3D currentVelocity, double smoothTime)
	{
		double deltaTime = Time.deltaTime;
		double maxSpeed = double.PositiveInfinity;
		return Vector3D.SmoothDamp (current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	}*/
	
	public static double SqrMagnitude (Vector3D a)
	{
		return a.x * a.x + a.y * a.y + a.z * a.z;
	}
	
	//
	// Methods
	//
	public override bool Equals (object other)
	{
		if (!(other is Vector3D))
		{
			return false;
		}
		Vector3D vector = (Vector3D)other;
		return this.x.Equals (vector.x) && this.y.Equals (vector.y) && this.z.Equals (vector.z);
	}
	
	public override int GetHashCode ()
	{
		return this.x.GetHashCode () ^ this.y.GetHashCode () << 2 ^ this.z.GetHashCode () >> 2;
	}

    public Vector3D Absolute()
    {
        return new Vector3D(System.Math.Abs(x),System.Math.Abs(y),System.Math.Abs(z));
    }

    // Return the axis with the largest value Note return values are 0,1,2 for x, y, or z.
    public int MaxAxis()
    {
        return x <y ? (y <z ? 2 : 1) : (x <z ? 2 : 0);

    }

    public void Normalize ()
	{
	    // Inspired by bullet's safenormalize
	    // http://bulletphysics.org/Bullet/BulletFull/btVector3_8h_source.html
	    var absVec = Absolute();
	    int maxIndex = absVec.MaxAxis();
	    if (absVec[maxIndex] > 0.0)
	    {

	        this /= absVec[maxIndex];
	        this /= Magnitude(this);

	    }
	    else
	    {
	        x = 0;
	        y = 0;
	        z = 0;
	    }
	}
	
	public void Scale (Vector3D scale)
	{
		this.x *= scale.x;
		this.y *= scale.y;
		this.z *= scale.z;
	}
	
	public void Set (double new_x, double new_y, double new_z)
	{
		this.x = new_x;
		this.y = new_y;
		this.z = new_z;
	}
	
	public string ToString (string format)
	{
		return string.Format ("({0}, {1}, {2})", new object[]
		                           {
			this.x.ToString (format),
			this.y.ToString (format),
			this.z.ToString (format)
		});
	}
	
	public override string ToString ()
	{
		return string.Format ("({0:F1}, {1:F1}, {2:F1})", new object[]
		                           {
			this.x,
			this.y,
			this.z
		});
	}
	
	//
	// Operators
	//
	public static Vector3D operator + (Vector3D a, Vector3D b)
	{
		return new Vector3D (a.x + b.x, a.y + b.y, a.z + b.z);
	}
	
	public static Vector3D operator / (Vector3D a, double d)
	{
		return new Vector3D (a.x / d, a.y / d, a.z / d);
	}
	
	public static bool operator == (Vector3D lhs, Vector3D rhs)
	{
		return Vector3D.SqrMagnitude (lhs - rhs) < 1E-21f;
	}
	
	public static bool operator != (Vector3D lhs, Vector3D rhs)
	{
		return Vector3D.SqrMagnitude (lhs - rhs) >= 1E-21f;
	}
	
	public static Vector3D operator * (double d, Vector3D a)
	{
		return new Vector3D (a.x * d, a.y * d, a.z * d);
	}
	
	public static Vector3D operator * (Vector3D a, double d)
	{
		return new Vector3D (a.x * d, a.y * d, a.z * d);
	}
	
	public static Vector3D operator - (Vector3D a, Vector3D b)
	{
		return new Vector3D (a.x - b.x, a.y - b.y, a.z - b.z);
	}
	
	public static Vector3D operator - (Vector3D a)
	{
		return new Vector3D (-a.x, -a.y, -a.z);
	}

	// determines if two directions (A, B) are parallel based on the distance from origin to AB is less than hmesh.minDist  
	//
	//  A----------------------------------B               
	//    - - - - - -       | dist  ------
	//               - - - -AB-------
	public static bool IsParallelDist(Vector3D a, Vector3D b, double minDistSqr)
	{
		// flip direction of a if needed
		if (Dot(a, b) > 0)
		{
			a = a * -1;
		}
		
		Vector3D ab = b - a;
		ab = ab.normalized;

		Vector3D aProjectedToAB = ab * Dot(-a, ab);
		
		// aProjectedToAB - (-a))
		return (aProjectedToAB + a).sqrMagnitude < minDistSqr;
	}
	
    public static bool IsParallel(Vector3D direction1, Vector3D direction2,double epsilon = 1E-10f)
    {
        var res = Cross(direction1, direction2);
        return res.sqrMagnitude < epsilon;
    }

    // Compute barycentric coordinates (u, v, w) for
    // point p with respect to triangle (a, b, c)
    // Based on
    // http://gamedev.stackexchange.com/a/23745
    public static Vector3D Barycentric(Vector3D p, Vector3D a, Vector3D b, Vector3D c)
    {
        Vector3D v0 = b - a, v1 = c - a, v2 = p - a;
        double d00 = Dot(v0, v0);
        double d01 = Dot(v0, v1);
        double d11 = Dot(v1, v1);
        double d20 = Dot(v2, v0);
        double d21 = Dot(v2, v1);
        double denom = d00 * d11 - d01 * d01;
        double v = (d11 * d20 - d01 * d21) / denom;
        double w = (d00 * d21 - d01 * d20) / denom;
        double u = 1.0f - v - w;
        return new Vector3D(v, w, u);
    }
}
