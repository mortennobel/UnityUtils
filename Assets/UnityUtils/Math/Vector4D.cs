using System;
using UnityEngine;


public struct Vector4D {

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

    public double w;

    //
    // Static Properties
    //
    public static Vector4D zero
    {
        get
        {
            return new Vector4D (0, 0, 0, 0);
        }
    }

    public static Vector4D one
    {
        get
        {
            return new Vector4D (1, 1, 1, 1);
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
            return System.Math.Sqrt (sqMag);
        }
    }

    public Vector4D normalized
    {
        get
        {
            return Vector4D.Normalize (this);
        }
    }

    public double sqrMagnitude
    {
        get
        {
            return this.x * this.x + this.y * this.y + this.z * this.z+this.w * this.w;
        }
    }

    /// <summary>
    /// Compare two vectors using Mathf.Approximatly
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Approximatly(Vector4D other)
    {
        for (int i = 0; i < 4; i++)
        {
            if (!Mathf.Approximately((float)this[i], (float)other[i]))
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
                case 3:
                    return this.w;
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
                case 3:
                    this.w = value;
                    break;
                default:
                    throw new IndexOutOfRangeException ("Invalid Vector3D index!");
            }
        }
    }

    //
    // Constructors
    //
    public Vector4D (double x, double y)
    {
        this.x = x;
        this.y = y;
        this.z = 0;
        this.w = 0;
    }

    public Vector4D (double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = 0;
    }
    public Vector4D (double x, double y, double z, double w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Vector4D (Vector4 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
        this.w = v.w;
    }

    public Vector4 ToVector4(){
        return new Vector4((float)x,(float)y,(float)z, (float)w);
    }

    public static Vector4D ClampMagnitude (Vector4D vector, double maxLength)
    {
        if (vector.sqrMagnitude > maxLength * maxLength)
        {
            return vector.normalized * maxLength;
        }
        return vector;
    }

    public static double Dot (Vector4D lhs, Vector4D rhs)
    {
        return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z + lhs.w * rhs.w;
    }

    public static Vector4D Lerp (Vector4D from, Vector4D to, double t)
    {
        t = Clamp01 (t);
        return new Vector4D (from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
    }

    public static double Magnitude (Vector4D a)
    {
        return Math.Sqrt (Dot(a,a));
    }

    public static Vector4D Max (Vector4D lhs, Vector4D rhs)
    {
        return new Vector4D (Math.Max (lhs.x, rhs.x), Math.Max (lhs.y, rhs.y), Math.Max (lhs.z, rhs.z), Math.Max (lhs.w, rhs.w));
    }

    public static Vector4D Min (Vector4D lhs, Vector4D rhs)
    {
        return new Vector4D (Math.Min (lhs.x, rhs.x), Math.Min (lhs.y, rhs.y), Math.Min (lhs.z, rhs.z), Math.Min (lhs.w, rhs.w));
    }

    public static Vector4D MoveTowards (Vector4D current, Vector4D target, double maxDistanceDelta)
    {
        Vector4D a = target - current;
        double magnitude = a.magnitude;
        if (magnitude <= maxDistanceDelta || magnitude == 0f)
        {
            return target;
        }
        return current + a / magnitude * maxDistanceDelta;
    }

    public static Vector4D Normalize (Vector4D value)
    {
        double num = Vector4D.Magnitude (value);
        if (num > 1E-25f) {
            return value / num;
        } else {
            Debug.LogWarning ("Error normalizing "+value);
        }
        return Vector4D.zero;
    }

    /*public static void OrthoNormalize (ref Vector3D normal, ref Vector3D tangent)
    {
        Vector3D.Internal_OrthoNormalize2 (ref normal, ref tangent);
    }

    public static void OrthoNormalize (ref Vector3D normal, ref Vector3D tangent, ref Vector3D binormal)
    {
        Vector3D.Internal_OrthoNormalize3 (ref normal, ref tangent, ref binormal);
    }*/


    /*public static Vector3D RotateTowards (Vector3D current, Vector3D target, double maxRadiansDelta, double maxMagnitudeDelta)
    {
        return Vector3D.INTERNAL_CALL_RotateTowards (ref current, ref target, maxRadiansDelta, maxMagnitudeDelta);
    }*/

    public static Vector4D Scale (Vector4D a, Vector4D b)
    {
        return new Vector4D (a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
    }

    public static bool AllLessThan(Vector4D a, Vector4D b)
    {
        return a.x < b.x && a.y < b.y && a.z < b.z && a.w < b.w;
    }

    public static bool AllLessEqualThan(Vector4D a, Vector4D b)
    {
        return a.x <= b.x && a.y <= b.y && a.z <= b.z && a.w <= b.w;
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

    public static double SqrMagnitude (Vector4D a)
    {
        return a.x * a.x + a.y * a.y + a.z * a.z+ a.w * a.w;
    }

    //
    // Methods
    //
    public override bool Equals (object other)
    {
        if (!(other is Vector4D))
        {
            return false;
        }
        Vector4D vector = (Vector4D)other;
        return this.x.Equals (vector.x) && this.y.Equals (vector.y) && this.z.Equals (vector.z) && this.w.Equals (vector.w);
    }

    public override int GetHashCode ()
    {
        return this.x.GetHashCode () ^ this.y.GetHashCode () << 2 ^ this.z.GetHashCode () >> 2^ this.w.GetHashCode () >> 3;
    }

    public void Normalize ()
    {
        double num = Vector4D.Magnitude (this);
        if (num > 1E-05f)
        {
            this /= num;
        }
        else
        {
            this = Vector4D.zero;
        }
    }

    public void Scale (Vector4D scale)
    {
        this.x *= scale.x;
        this.y *= scale.y;
        this.z *= scale.z;
        this.w *= scale.w;
    }

    public void Set (double new_x, double new_y, double new_z, double new_w)
    {
        this.x = new_x;
        this.y = new_y;
        this.z = new_z;
        this.w = new_w;
    }

    public string ToString (string format)
    {
        return string.Format ("({0}, {1}, {2}, {3})", new object[]
        {
            this.x.ToString (format),
            this.y.ToString (format),
            this.z.ToString (format),
            this.w.ToString (format),
        });
    }

    public override string ToString ()
    {
        return string.Format ("({0:F1}, {1:F1}, {2:F1}, {3:F1})", new object[]
        {
            this.x,
            this.y,
            this.z,
            this.w
        });
    }

    //
    // Operators
    //
    public static Vector4D operator + (Vector4D a, Vector4D b)
    {
        return new Vector4D (a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    }

    public static Vector4D operator / (Vector4D a, double d)
    {
        return new Vector4D (a.x / d, a.y / d, a.z / d, a.w / d);
    }

    public static bool operator == (Vector4D lhs, Vector4D rhs)
    {
        return Vector4D.SqrMagnitude (lhs - rhs) < 9.99999944E-11f;
    }

    public static bool operator != (Vector4D lhs, Vector4D rhs)
    {
        return Vector4D.SqrMagnitude (lhs - rhs) >= 9.99999944E-11f;
    }

    public static Vector4D operator * (double d, Vector4D a)
    {
        return new Vector4D (a.x * d, a.y * d, a.z * d, a.w * d);
    }

    public static Vector4D operator * (Vector4D a, double d)
    {
        return new Vector4D (a.x * d, a.y * d, a.z * d, a.w * d);
    }

    public static Vector4D operator - (Vector4D a, Vector4D b)
    {
        return new Vector4D (a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
    }

    public static Vector4D operator - (Vector4D a)
    {
        return new Vector4D (-a.x, -a.y, -a.z, -a.w);
    }
}
