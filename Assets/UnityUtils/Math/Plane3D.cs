using UnityEngine;

public struct Plane3D
{
	double m_Distance;
	Vector3D m_Normal;
	//
	// Properties
	//
	public double distance
	{
		get
		{
			return this.m_Distance;
		}
		set
		{
			this.m_Distance = value;
		}
	}
	
	public Vector3D normal
	{
		get
		{
			return this.m_Normal;
		}
		set
		{
			this.m_Normal = value;
		}
	}
	
	//
	// Constructors
	//

	public Plane3D (Vector3D inNormal, double d)
	{
		this.m_Normal = Vector3D.Normalize (inNormal);
		this.m_Distance = d;
	}
	
	public Plane3D (Vector3D inNormal, Vector3D inPoint)
	{
		this.m_Normal = Vector3D.Normalize (inNormal);
		this.m_Distance = -Vector3D.Dot (m_Normal, inPoint);
	}
	
	//
	// Methods
	//
	public double GetDistanceToPoint (Vector3D inPt)
	{
	    return Vector3D.Dot (this.normal, inPt) + distance;
	}
	
	public bool GetSide (Vector3D inPt)
	{
		return GetDistanceToPoint( inPt) > 0f;
	}
	
	public bool Raycast (RayD ray, out double enter)
	{
		double num = Vector3D.Dot (ray.direction, normal);
		double num2 = -Vector3D.Dot (ray.origin, normal) - distance;
		if (Mathf.Approximately ((float)num, 0f))
		{
			enter = 0f;
			return false;
		}
		enter = num2 / num;
		return enter > 0f;
	}
	
	public bool SameSide (Vector3D inPt0, Vector3D inPt1)
	{
		double distanceToPoint = this.GetDistanceToPoint (inPt0);
		double distanceToPoint2 = this.GetDistanceToPoint (inPt1);
		return (distanceToPoint > 0f && distanceToPoint2 > 0f) || (distanceToPoint <= 0f && distanceToPoint2 <= 0f);
	}
	
	public void Set3Points (Vector3D a, Vector3D b, Vector3D c)
	{
		this.normal = Vector3D.Normalize (Vector3D.Cross (b - a, c - a));
		this.distance = -Vector3D.Dot (this.normal, a);
	}
	
	public void SetNormalAndPosition (Vector3D inNormal, Vector3D inPoint)
	{
		this.normal = Vector3D.Normalize (inNormal);
		this.distance = -Vector3D.Dot (inNormal, inPoint);
	}

    public Vector3D ProjectAlongYAxis(double x, double z)
    {
        double y = -(+m_Distance + x * m_Normal.x + z * m_Normal.z) / m_Normal.y;
        return new Vector3D(x,y,z);
    }

    public void DebugDraw()
    {
        DebugExt.DrawCross((normal*distance).ToVector3(),Color.green,1,10);
    }

    public override string ToString()
    {
        return "Plane normal: " + normal.ToString("R") + " dist " + distance.ToString("R");
    }

    // return NaN if line is parallel with plane
    public Vector3D LineIntersection(Vector3D lineFrom, Vector3D lineTo)
    {
        var ray = new RayD(lineFrom, (lineTo - lineFrom).normalized);
        double enter;
        double num = Vector3D.Dot (ray.direction, normal);
        double num2 = -Vector3D.Dot (ray.origin, normal) - distance;
        if (num == 0)
        {
            return Vector3D.one * double.NaN;
        }
        enter = num2 / num;
        return ray.GetPointAt(enter);
    }


}