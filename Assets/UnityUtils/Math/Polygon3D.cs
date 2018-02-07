/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

using System.Collections.Generic;

public class Polygon3D
{
    public Vector3D basePoint;
    public Vector3D normal;
    public List<Vector3D> vertices;

    public Polygon3D()
    {
        basePoint = Vector3D.zero;
        normal = Vector3D.zero;
        vertices = new List<Vector3D>();
    }

    public void Reverse()
    {
        normal *= -1;

        for (int i = vertices.Count / 2; i >= 0; i--)
        {
            var other = vertices.Count - 1 - i;
            var tmp = vertices[other];
            vertices[other] = vertices[i];
            vertices[i] = tmp;
        }
    }

    public int Fix()
    {
        int prev = vertices.Count - 1;
        int j = 0;
        for (int i = 0; i < vertices.Count; i++)
        {
            if ( !vertices[i].AreSame(vertices[prev]) )
            {
                if (j != i)
                {
                    vertices[j] = vertices[i];
                }
                prev = j;
                j++;
            }
        }
        if (j < 3)
        {
            vertices.Clear();
        }
        else if (j < vertices.Count)
        {
            vertices.RemoveRange(j, vertices.Count-j);
        }
        return vertices.Count;
    }

    public double Area()
    {
        if (vertices.Count <= 2)
        {
            return 0;
        }
        Vector3D s1 = vertices[1] - vertices[0];

        double area = 0.0;
        for (int i = 2; i < vertices.Count; i++)
        {
            Vector3D s2 = vertices[i] - vertices[0];
            area += Vector3D.Cross(s1, s2).magnitude * 0.5;
            s1 = s2;
        }
        return area;
    }

    public enum SplitType
    {
        SP_Coplanar		= 0, // Poly wasn't split, but is coplanar with plane
        SP_Front		= 1, // Poly wasn't split, but is entirely in front of plane
        SP_Back			= 2, // Poly wasn't split, but is entirely in back of plane
        SP_Split		= 3, // Poly was split into two new editor polygons
    }

    enum VType
    {
        Front,
        Back,
        Either
    }

    public SplitType SplitWithPlane(Plane3D plane, out Polygon3D front, out Polygon3D back,
        bool veryPrecise)
    {
        Vector3D intersection;

        front = null;
        back = null;

        double dist = 0;
        double maxDist = 0;
        double minDist = 0;
        double prevDist;
        double threshold;
        VType status;
        VType prevStatus = VType.Either;

        // double scale
        if (veryPrecise)
        {
            threshold = 0.01;
        }
        else
        {
            threshold = 0.25;
        }

        for (int i = 0; i<vertices.Count; i++)
        {
            dist = plane.GetDistanceToPoint(vertices[i]);
            if (i == 0 || dist > maxDist)
            {
                maxDist = dist;
            }
            if (i == 0 || dist < minDist)
            {
                minDist = dist;
            }
            if (dist > +threshold)
            {
                prevStatus = VType.Front;
            }
            else if (dist < -threshold)
            {
                prevStatus = VType.Back;
            }
        }
        if (maxDist < threshold && minDist > -threshold)
        {
            return SplitType.SP_Coplanar;
        }
        else if (maxDist < threshold)
        {
            return SplitType.SP_Back;
        }
        else if (minDist > -threshold)
        {
            return SplitType.SP_Front;
        }
        else
        {
            front = Clone();
            front.vertices.Clear();

            back = Clone();
            back.vertices.Clear();

            int j = vertices.Count - 1;

            for (int i = 0; i < vertices.Count; i++)
            {
                prevDist = dist;
                dist = plane.GetDistanceToPoint(vertices[i]);

                if (dist > +threshold) status = VType.Front;
                else if (dist < -threshold) status = VType.Back;
                else  status = prevStatus;

                if( status != prevStatus )
                {
                    // Crossing.  Either Front-to-Back or Back-To-Front.
                    // Intersection point is naturally on both front and back polys.
                    if( (dist >= -threshold) && (dist < +threshold) )
                    {
                        // This point lies on plane.
                        if( prevStatus == VType.Front )
                        {
                            front.vertices.Add(vertices[i]);
                            back.vertices.Add(vertices[i]);
                        }
                        else
                        {
                            back.vertices.Add(vertices[i]);
                            front.vertices.Add(vertices[i]);
                        }
                    }
                    else if( (prevDist >= -threshold) && (prevDist < +threshold) )
                    {
                        // Previous point lies on plane.
                        if (status == VType.Front)
                        {
                            front.vertices.Add(vertices[j]);
                            front.vertices.Add(vertices[i]);
                        }
                        else
                        {
                            back.vertices.Add(vertices[j]);
                            back.vertices.Add(vertices[i]);
                        }
                    }
                    else
                    {
                        // Intersection point is in between.
                        intersection = plane.LineIntersection(vertices[j],vertices[i]);

                        if( prevStatus == VType.Front )
                        {
                            front.vertices.Add(intersection);
                            back.vertices.Add(intersection);
                            back.vertices.Add(vertices[i]);
                        }
                        else
                        {
                            back.vertices.Add(intersection);
                            front.vertices.Add(intersection);
                            front.vertices.Add(vertices[i]);
                        }
                    }
                }
                else
                {
                    if (status==VType.Front) front.vertices.Add(vertices[i]);
                    else                 back.vertices.Add(vertices[i]);
                }
                j          = i;
                prevStatus = status;
            }

            // Handle possibility of sliver polys due to precision errors.
            if( front.Fix()<3 )
            {
                return SplitType.SP_Back;
            }
            else if( back.Fix()<3 )
            {
                return SplitType.SP_Front;
            }
            else return SplitType.SP_Split;
        }
    }

    public List<Triangle3D> Triangulate()
    {
        List<Triangle3D> res = new List<Triangle3D>();
        if (vertices.Count < 3)
        {
            return res;
        }
        if (vertices.Count == 3)
        {
            res.Add(new Triangle3D(vertices[0], vertices[1], vertices[2]));
        }
        var polyVerts = new List<Vector3D>(vertices);
        while (true)
        {
            if (polyVerts.Count < 3)
            {
                break;
            }
            // Look for an 'ear' triangle
            bool bFoundEar = false;
            for(int EarVertexIndex = 0;EarVertexIndex < polyVerts.Count;EarVertexIndex++)
            {
                // Triangle is 'this' vert plus the one before and after it
                int AIndex = (EarVertexIndex==0) ? polyVerts.Count-1 : EarVertexIndex-1;
                int BIndex = EarVertexIndex;
                int CIndex = (EarVertexIndex+1)%polyVerts.Count;

                // Check that this vertex is convex (cross product must be positive)
                var ABEdge = polyVerts[BIndex] - polyVerts[AIndex];
                var ACEdge = polyVerts[CIndex] - polyVerts[AIndex];
                double TriangleDeterminant = Vector3D.Dot(Vector3D.Cross(ABEdge, ACEdge) , normal);
                bool IsNegativeFloat = TriangleDeterminant < 0;
                if(IsNegativeFloat)
                {
                    continue;
                }

                bool bFoundVertInside = false;
                // Look through all verts before this in array to see if any are inside triangle
                for(int VertexIndex = 0;VertexIndex < polyVerts.Count;VertexIndex++)
                {
                    if(	VertexIndex != AIndex && VertexIndex != BIndex && VertexIndex != CIndex &&
                       	new Triangle3D(polyVerts[AIndex], polyVerts[BIndex], polyVerts[CIndex]).PointInTriangle(polyVerts[VertexIndex]) )
                    {

                        bFoundVertInside = true;
                        break;
                    }
                }

                // Triangle with no verts inside - its an 'ear'!
                if(!bFoundVertInside)
                {
                    // Add to output list..
                    res.Add(new Triangle3D(polyVerts[AIndex], polyVerts[BIndex], polyVerts[CIndex]));

                    // And remove vertex from polygon
                    polyVerts.RemoveAt(EarVertexIndex);

                    bFoundEar = true;
                    break;
                }
            }

            // If we couldn't find an 'ear' it indicates something is bad with this polygon - discard triangles and return.
            if(!bFoundEar)
            {
                res.Clear();
                return res;
            }
        }
        return res;
    }

    public Polygon3D Clone()
    {
        var res = new Polygon3D();
        res.basePoint = basePoint;
        res.normal = normal;
        res.vertices = new List<Vector3D>(vertices);
        return res;
    }
}
