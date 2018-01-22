using System;
using System.Collections.Generic;

public class Triangle3D
{

    private Vector3D[] points;

    public Triangle3D(Vector3D p1,Vector3D p2, Vector3D p3)
    {
        points = new[] { p1, p2, p3};
    }

    public double ComputeArea()
    {
        return Vector3D.Cross(points[1] - points[0], points[2] - points[0]).magnitude*0.5;
    }

    public Vector3D ComputeCenter()
    {
        return (points[0] + points[1] + points[2])*(1/3.0);
    }

    public Vector3D GetNormal()
    {
        var res = Vector3D.Cross(points[1] - points[0], points[2] - points[0]);
        var sqrMagnitude = res.sqrMagnitude;
        if (sqrMagnitude > 0)
        {
            res = res.normalized;
        }
        return res;
    }

    public Vector3D this[int key]
    {
        get { return points[key]; }
        set { points[key] = value; }
    }

    public string ToString (string format)
    {
        return string.Format ("({0}, {1}, {2})", new object[]
        {
            this[0].ToString (format),
            this[1].ToString (format),
            this[2].ToString (format)
        });
    }

    public bool VectorsOnSameSide(Vector3D Vec, Vector3D A, Vector3D B)
    {
        var CrossA = Vector3D.Cross(Vec , A);
        var  CrossB = Vector3D.Cross(Vec, B);
        bool IsNegativeFloat = Vector3D.Dot(CrossA, CrossB)<0;
        return !IsNegativeFloat;
    }

    public bool PointInTriangle(Vector3D P)
    {
        var A = this[0];
        var B = this[1];
        var C = this[2];
        // Cross product indicates which 'side' of the vector the point is on
        // If its on the same side as the remaining vert for all edges, then its inside.
        if( VectorsOnSameSide(B-A, P-A, C-A) &&
            VectorsOnSameSide(C-B, P-B, A-B) &&
            VectorsOnSameSide(A-C, P-C, B-C) )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override string ToString ()
    {
        return string.Format ("({0}, {1}, {2})", new object[]
        {
            this[0],
            this[1],
            this[2]
        });
    }

    public static List<Triangle3D> SplitPlaneTriangleIntersection(Vector3D planeNormal, Vector3D planePoint, Vector3D p1,
        Vector3D p2, Vector3D p3)
    {

        Plane3D plane = new Plane3D(planeNormal,planePoint );
        var triangle = new Triangle3D(p1,
            p2, p3);

        var res = SplitPlaneTriangleIntersection(plane, triangle);

        return res;
    }


    public struct PlaneTriangleIntersectionResult
    {
        public bool intersect;

        // The number is 0 (no intersection), 1 (plane and triangle intersect
        // at a single point [vertex]), 2 (plane and triangle intersect in a
        // segment), or 3 (triangle is in the plane).  When the number is 2,
        // the segment is either interior to the triangle or is an edge of the
        // triangle, the distinction stored in 'isInterior'.
        public int numIntersections;
        public bool isInterior;
        public Vector3D[] point;

        public PlaneTriangleIntersectionResult(bool intersect)
        {
            this.intersect = intersect;
            this.numIntersections = 0;
            this.isInterior = false;
            this.point = new Vector3D[3];
        }
    };

    public static List<Triangle3D> SplitPlaneTriangleIntersection(Plane3D plane, Triangle3D inTriangle, double epsilon = 1e-7)
    {
        Polygon3D poly = new Polygon3D();
        poly.vertices.Add(inTriangle[0]);
        poly.vertices.Add(inTriangle[1]);
        poly.vertices.Add(inTriangle[2]);
        Polygon3D front;
        Polygon3D back;
        var res = poly.SplitWithPlane(plane, out front, out back, true);

        var triangleList = new List<Triangle3D>();
        if (res != Polygon3D.SplitType.SP_Split)
        {
            triangleList.AddRange(new[]{inTriangle});
            return triangleList;
        }


        var polygons = new[] { front, back};
        foreach (var p in polygons)
        {
            int offset = 0;
            if (p.vertices.Count > 3)
            {
                double o0a0 = new Triangle3D(p.vertices[0], p.vertices[1], p.vertices[2]).ComputeArea();
                double o0a1 = new Triangle3D(p.vertices[0], p.vertices[2], p.vertices[3]).ComputeArea();
                double o1a0 = new Triangle3D(p.vertices[1], p.vertices[2], p.vertices[3]).ComputeArea();
                double o1a1 = new Triangle3D(p.vertices[1], p.vertices[3], p.vertices[0]).ComputeArea();
                // find optimat split by taking the least difference between triangle areas
                if (Math.Abs(o0a0 - o0a1) > Math.Abs(o1a0 - o1a1))
                {
                    offset = 1;
                }
            }
            for (int i = 2; i < p.vertices.Count; i++)
            {
                triangleList.Add(new Triangle3D( p.vertices[offset],
                    p.vertices[(i-1+offset)%p.vertices.Count],
                    p.vertices[(i+offset)%p.vertices.Count]
                    ));
            }
        }

        return triangleList;
    }

    public static bool IsPointsBehindTriangle(Vector3D p1, Vector3D p2, Vector3D p3, bool include, params Vector3D[] points)
    {
        Vector3D normal = Vector3D.Cross(p2 - p1, p3 - p1).normalized;
        foreach (var p in points)
        {
            // Find distance from LP1 and LP2 to the plane defined by the triangle
            double Dist1 = Vector3D.Dot(p - p1, normal);
            if (include)
            {
                if (Dist1 > 0)
                {
                    return false;
                }
            } else if (Dist1 >= 0)
            {
                return false;
            }
        }
        return true;
    }

/*! @param PIP Point-in-Plane */
    // http://gamedev.stackexchange.com/a/5589
    public static bool testRayThruTriangle( Vector3D P1, Vector3D P2, Vector3D P3, Vector3D R1, Vector3D R2, out Vector3D PIP)
    {
        Vector3D Normal;
        Vector3D IntersectPos;
        PIP = Vector3D.zero;

        // Find Triangle Normal
        Normal = Vector3D.Cross( P2 - P1, P3 - P1 );
        Normal = Normal.normalized; // not really needed?  Vector3f does this with cross.

        // Find distance from LP1 and LP2 to the plane defined by the triangle
        double Dist1 = Vector3D.Dot(R1-P1, Normal );
        double Dist2 = Vector3D.Dot(R2-P1, Normal );

        if ( (Dist1 * Dist2) >= 0.0) {
            //SFLog(@"no cross");
            return false;
        } // line doesn't cross the triangle.

        if ( Dist1 == Dist2) {
            //SFLog(@"parallel");
            return false;
        } // line and plane are parallel

        // Find point on the line that intersects with the plane
        IntersectPos = R1 + (R2-R1) * ( -Dist1/(Dist2-Dist1) );

        // Find if the interesection point lies inside the triangle by testing it against all edges
        Vector3D vTest;

        vTest = Vector3D.Cross( Normal, P2-P1 );
        if ( Vector3D.Dot( vTest, IntersectPos-P1) < 0.0 ) {
            return false;
        }

        vTest = Vector3D.Cross( Normal, P3-P2 );
        if ( Vector3D.Dot( vTest, IntersectPos-P2) < 0.0 ) {
            return false;
        }

        vTest = Vector3D.Cross( Normal, P1-P3 );
        if ( Vector3D.Dot( vTest, IntersectPos-P1) < 0.0 ) {
            return false;
        }

        PIP = IntersectPos;

        return true;
    }
}
