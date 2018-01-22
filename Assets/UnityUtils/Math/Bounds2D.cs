using UnityEngine;

public struct BoundsD {

    private Vector3D m_Center;
    private Vector3D m_Extents;

    /// <summary>
    ///   <para>The center of the bounding box.</para>
    /// </summary>
    public Vector3D center
    {
        get
        {
            return this.m_Center;
        }
        set
        {
            this.m_Center = value;
        }
    }

    /// <summary>
    ///   <para>The total size of the box. This is always twice as large as the extents.</para>
    /// </summary>
    public Vector3D size
    {
        get
        {
            return this.m_Extents * 2f;
        }
        set
        {
            this.m_Extents = value * 0.5f;
        }
    }

    /// <summary>
    ///   <para>The extents of the box. This is always half of the size.</para>
    /// </summary>
    public Vector3D extents
    {
        get
        {
            return this.m_Extents;
        }
        set
        {
            this.m_Extents = value;
        }
    }

    /// <summary>
    ///   <para>The minimal point of the box. This is always equal to center-extents.</para>
    /// </summary>
    public Vector3D min
    {
        get
        {
            return this.center - this.extents;
        }
        set
        {
            this.SetMinMax(value, this.max);
        }
    }

    /// <summary>
    ///   <para>The maximal point of the box. This is always equal to center+extents.</para>
    /// </summary>
    public Vector3D max
    {
        get
        {
            return this.center + this.extents;
        }
        set
        {
            this.SetMinMax(this.min, value);
        }
    }

    /// <summary>
    ///   <para>Creates new Bounds with a given center and total size. Bound extents will be half the given size.</para>
    /// </summary>
    /// <param name="center"></param>
    /// <param name="size"></param>
    public BoundsD(Vector3D center, Vector3D size)
    {
        this.m_Center = center;
        this.m_Extents = size * 0.5;
    }

    public static bool operator ==(BoundsD lhs, BoundsD rhs)
    {
        if (lhs.center == rhs.center)
            return lhs.extents == rhs.extents;
        return false;
    }

    public static bool operator !=(BoundsD lhs, BoundsD rhs)
    {
        return !(lhs == rhs);
    }

    public override int GetHashCode()
    {
        return this.center.GetHashCode() ^ this.extents.GetHashCode() << 2;
    }

    public override bool Equals(object other)
    {
        if (!(other is Bounds))
            return false;
        Bounds bounds = (Bounds) other;
        if (this.center.Equals((object) bounds.center))
            return this.extents.Equals((object) bounds.extents);
        return false;
    }

    /// <summary>
    ///   <para>Sets the bounds to the min and max value of the box.</para>
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void SetMinMax(Vector3D min, Vector3D max)
    {
        this.extents = (max - min) * 0.5;
        this.center = min + this.extents;
    }

    /// <summary>
    ///   <para>Grows the Bounds to include the point.</para>
    /// </summary>
    /// <param name="point"></param>
    public void Encapsulate(Vector3D point)
    {
        this.SetMinMax(Vector3D.Min(this.min, point), Vector3D.Max(this.max, point));
    }

    /// <summary>
    ///   <para>Grow the bounds to encapsulate the bounds.</para>
    /// </summary>
    /// <param name="bounds"></param>
    public void Encapsulate(BoundsD bounds)
    {
        this.Encapsulate(bounds.center - bounds.extents);
        this.Encapsulate(bounds.center + bounds.extents);
    }

    /// <summary>
    ///   <para>Expand the bounds by increasing its size by amount along each side.</para>
    /// </summary>
    /// <param name="amount"></param>
    public void Expand(double amount)
    {
        amount *= 0.5;
        this.extents += new Vector3D(amount, amount, amount);
    }

    /// <summary>
    ///   <para>Expand the bounds by increasing its size by amount along each side.</para>
    /// </summary>
    /// <param name="amount"></param>
    public void Expand(Vector3D amount)
    {
        this.extents += amount * 0.5f;
    }

    /// <summary>
    ///   <para>Does another bounding box intersect with this bounding box?</para>
    /// </summary>
    /// <param name="bounds"></param>
    public bool Intersects(BoundsD bounds)
    {
        if ((double) this.min.x <= (double) bounds.max.x && (double) this.max.x >= (double) bounds.min.x && ((double) this.min.y <= (double) bounds.max.y && (double) this.max.y >= (double) bounds.min.y) && (double) this.min.z <= (double) bounds.max.z)
            return (double) this.max.z >= (double) bounds.min.z;
        return false;
    }

    /// <summary>
    ///   <para>Is point contained in the bounding box?</para>
    /// </summary>
    /// <param name="point"></param>
    public bool Contains(Vector3D point)
    {
        for (int i = 0; i < 3; i++)
        {
            if (point[i] < this.min[i] ||
                point[i] > this.max[i])
            {
                return false;
            }
        }
        return true;
    }

    public override string ToString()
    {
        return "Bounds {"+m_Center+", "+m_Extents+"} min max {"+this.min+", "+this.max+"}";
    }

    public Bounds ToBounds()
    {
        return new Bounds(center.ToVector3(), size.ToVector3());
    }

    public void DebugDraw(Color color, float duration = 0)
    {
        DebugExt.DrawBox(min.ToVector3(),max.ToVector3(), color, duration);
    }
}
