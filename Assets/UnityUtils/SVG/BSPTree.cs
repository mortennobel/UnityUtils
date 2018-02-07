/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// prepartisions the domain if fixed regions to enforce a more balanced tree
public class BSPTreeQuad<T> : BSPTree<T>
{
    private void Split(int axis, BoundsD bounds, int subdivisions)
    {
        //Debug.Log("Axis " + axis);
        //Debug.Log("Bounds " + bounds+" path "+path);
        if (subdivisions <= 0)
        {
            return;
        }
        Vector3D axisVector = Vector3D.zero;
        axisVector[axis] = 1;
        var plane = new Plane3D(axisVector, -bounds.center[axis]);
        Insert(plane);
        //Debug.Log("Layer " +layer+ " Insert split " + new Plane3D(axisVector, bounds.center[axis])+" dist to center "+plane.GetDistanceToPoint(bounds.center));
        BoundsD left = new BoundsD(bounds.center + (bounds.extents * 0.5), bounds.size * 0.5); // larger
        BoundsD right = new BoundsD(bounds.center - (bounds.extents * 0.5), bounds.size * 0.5); // smaller
        Split(axis, left, subdivisions - 1);
        Split(axis, right, subdivisions - 1);
    }

    private BoundsD bounds;
    private Vector3i subdivisions;

    static int IntPow(int a, int b)
    {
        int result = 1;
        for (long i = 0; i < b; i++)
            result *= a;
        return result;
    }

    // note that subdivisions will result n^s-1 split (for each s in subdivions)
    public BSPTreeQuad(BoundsD bounds, Vector3i subdivisions)
    {
        Debug.Log("BSPTreeQuad " + bounds);
        this.bounds = bounds;
        this.subdivisions = subdivisions;
        for (int i = 0; i < 3; i++)
        {
            Split(i, bounds, subdivisions[i]);
        }
    }


    public void DebugQuadPlanes(String s, BSPTreeNode n)
    {
        if (n == null || (n.triangles.Count > 0))
        {
            // Debug.Log("Tree "+s+" "+(n==null?-1:n.ToList().Count));
        }
        else
        {

            DebugQuadPlanes(s+n.plane.normal+n.plane.distance+"L",n.left);
            DebugQuadPlanes(s+n.plane.normal+n.plane.distance+"R",n.right);
        }
    }

    public void DebugQuadPlanes()
    {
        DebugQuadPlanes("", node);
    }

    public Vector3i GetCellCount()
    {
        return new Vector3i(IntPow(2, subdivisions.x), IntPow(2, subdivisions.y), IntPow(2, subdivisions.z));
    }

    public BSPTreeNode GetLeafNode(Vector3i index)
    {
        BSPTreeNode n = node;
        Vector3i cells = GetCellCount();

        for (int i = 0; i < 3; i++)
        {
            int min = 0;
            int max = cells[i];
            while (max - 1 > min)
            {
                int mid = (max + min) / 2;
                string s;
                if (index[i] > mid)
                {
                    n = n.right;
                    min = mid;
                }
                else
                {
                    n = n.left;
                    max = mid;
                }
            }
        }

        // Debug.Log("cells " + cells);
        return n;
    }
}

public class BSPTree<T>
{
    public class BSPTriangle
    {
        public Triangle3D pos;
        public T val;

        public BSPTriangle(Triangle3D pos, T val)
        {
            this.pos = pos;
            this.val = val;
        }

        public BSPTriangle(Vector3D p0, Vector3D p1, Vector3D p2, T val)
        {
            pos = new Triangle3D(p0, p1, p2);
            this.val = val;
        }

        public Vector3D this[int key]
        {
            get { return pos[key]; }
            set { pos[key] = value; }
        }

        public override string ToString()
        {
            return "'" +pos+" "+val+"'";
        }
    }

    public enum IntersectionResult
    {
        Larger,
        Intersection,
        Smaller,
        InPlane
    }

    public class BSPTreeNode
    {
        // the triangle also acts as a splitting plance
        public List<BSPTriangle> triangles;

        public Plane3D plane;

        public BSPTreeNode left; // larger than splitting plane
        public BSPTreeNode right; // smaller than splitting plane

        public BSPTreeNode(Triangle3D tri, T val)
        {
            triangles = new List<BSPTriangle>();
            triangles.Add(new BSPTriangle(tri, val));
            plane = new Plane3D();
            plane.Set3Points(tri[0], tri[1], tri[2]);
        }

        public BSPTreeNode(Plane3D p)
        {
            triangles = new List<BSPTriangle>();
            plane = p;
        }

        // return 1 if triangle is larger than splitting plane (in front of)
        // return 0 if triangle intersect
        // return -1 if triangle is smaller than splitting plane (begin)
        // return 2 if triangle is in same plane as triangle (currently assuming that triangle is non intersecting)
        public IntersectionResult Intersect(Triangle3D newTriangle, double distanceThreshold = 0.000027)
        {
            int larger = 0;
            int equal = 0;
            int smaller = 0;
            for (int i = 0; i < 3; i++)
            {
                double val = plane.GetDistanceToPoint(newTriangle[i]);
                double distanceRelativeError = distanceThreshold * System.Math.Abs(plane.distance);
                if (val > distanceRelativeError)
                {
                    larger++;
                }
                else if (val < -distanceRelativeError)
                {
                    smaller++;
                }
                else
                {
                    equal++;
                }
            }
            // Debug.Log(string.Format("Larger {0} Equal {1} Smaller {2}",larger, equal, smaller));
            if (equal == 3)
            {
                return IntersectionResult.InPlane;
            }
            if (larger + equal == 3)
            {
                return IntersectionResult.Larger;
            }
            if (smaller + equal == 3)
            {
                return IntersectionResult.Smaller;
            }
            return IntersectionResult.Intersection; // intersection
        }

        public void Split(Triangle3D newTriangle, out Triangle3D[] resT)
        {
            var res = Triangle3D.SplitPlaneTriangleIntersection(plane,
                new Triangle3D(newTriangle[0], newTriangle[1], newTriangle[2]));
            resT = res.ToArray();
        }

        // split both child spaces
        public void Insert(Plane3D p)
        {
            if (this.plane.normal == p.normal)
            {
                Vector3D point = p.normal * -p.distance;
                double distanceToPoint = this.plane.GetDistanceToPoint(point);
                if (distanceToPoint > 0)
                {
                    if (left == null)
                    {
                        left = new BSPTreeNode(p);
                    }
                    else
                    {
                        left.Insert(p);
                    }
                }
                else if (distanceToPoint < 0)
                {
                    if (right == null)
                    {
                        right = new BSPTreeNode(p);
                    }
                    else
                    {
                        right.Insert(p);
                    }
                }
                else
                {
                    Debug.LogError("Splitting plane already defined");
                }
            }
            else
            {
                if (left == null)
                {
                    left = new BSPTreeNode(p);
                }
                else
                {
                    left.Insert(p);
                }
                if (right == null)
                {
                    right = new BSPTreeNode(p);
                }
                else
                {
                    right.Insert(p);
                }
            }
        }

        public void Insert(Triangle3D newTriangle, T val)
        {
            var intersection = Intersect(newTriangle);
            if (intersection == IntersectionResult.InPlane)
            {
                triangles.Add(new BSPTriangle(newTriangle, val));
            }
            else if (intersection == IntersectionResult.Larger)
            {
                if (left == null)
                {
                    left = new BSPTreeNode(newTriangle, val);
                }
                else
                {
                    left.Insert(newTriangle, val);
                }
            }
            else if (intersection == IntersectionResult.Smaller)
            {
                if (right == null)
                {
                    right = new BSPTreeNode(newTriangle, val);
                }
                else
                {
                    right.Insert(newTriangle, val);
                }
            }
            else if (intersection == IntersectionResult.Intersection)
            {
                Triangle3D[] res;
                Split(newTriangle, out res);
                foreach (var l in res)
                {
                    double dist = plane.GetDistanceToPoint(l.ComputeCenter());
                    if (dist > 0)
                    {
                        if (left == null)
                        {
                            left = new BSPTreeNode(l, val);
                        }
                        else
                        {
                            left.Insert(l, val);
                        }
                    }
                    else
                    {
                        if (right == null)
                        {
                            right = new BSPTreeNode(l, val);
                        }
                        else
                        {
                            right.Insert(l, val);
                        }
                    }
                }
            }
        }

        public List<List<BSPTriangle>> ToList()
        {
            List<List<BSPTriangle>> list = new List<List<BSPTriangle>>();
            Iterate(list);
            return list;
        }

        public void Iterate(List<List<BSPTriangle>> list)
        {
            if (right != null)
            {
                right.Iterate(list);
            }
            list.Add(triangles);
            if (left != null)
            {
                left.Iterate(list);
            }
        }

        // Return the number of layers (including this layer)
        public int GetDepth()
        {
            int max = 0;
            if (left != null)
            {
                max = left.GetDepth();
            }
            if (right != null)
            {
                max = Mathf.Max(max, right.GetDepth());
            }

            return 1 + max;
        }

        public int GetSize()
        {
            int sum = 1;
            if (left != null)
            {
                sum += left.GetSize();
            }
            if (right != null)
            {
                sum += right.GetSize();
            }
            return sum;
        }

        public void ToDebug(StringWriter sw)
        {
            sw.Write("{");

            sw.Write("'plane':'"+plane.ToString()+"',\n");

            sw.Write("'triangles':'");
            foreach (var v in triangles)
            {
                sw.Write(v.ToString());
            }
            sw.Write("',\n");
            sw.Write("'left':");
            if (left != null)
            {
                left.ToDebug(sw);
            }
            else
            {
                sw.Write("null");
            }
            sw.Write(",\n");
            sw.Write("'right':");
            if (right != null)
            {
                right.ToDebug(sw);
            }
            else
            {
                sw.Write("null");
            }
            sw.Write("\n");
            sw.Write("}");
        }
    }

    public BSPTreeNode node;

    public BSPTree()
    {
    }

    public void Insert(Plane3D p)
    {
        if (node == null)
        {
            node = new BSPTreeNode(p);
        }
        else
        {
            node.Insert(p);
        }
    }

    public void Insert(Triangle3D t, T val)
    {
        if (node == null)
        {
            node = new BSPTreeNode(t, val);
        }
        else
        {
            node.Insert(t, val);
        }
    }

    public List<List<BSPTriangle>> ToList()
    {
        List<List<BSPTriangle>> list = new List<List<BSPTriangle>>();
        if (node != null)
        {
            node.Iterate(list);
        }
        return list;
    }

    public int GetDepth()
    {
        if (node == null)
        {
            return 0;
        }
        return node.GetDepth();
    }

    public int GetSize()
    {
        if (node == null)
        {
            return 0;
        }
        return node.GetSize();
    }

    // return how well the nodes are distributed related to the maximum depth
    // size / maxNodex (of tree height)
    public float BalancedFraction()
    {
        int depth = GetDepth();
        float maxNodes = Mathf.Pow(2, depth) - 1;
        return GetSize() / maxNodes;
    }

    public String ToDebug()
    {
        StringWriter sw = new StringWriter();
        sw.Write("{'Name': 'BSPTree',\n");
        sw.Write("'Node': \n");
        if (node == null)
        {
            sw.Write("null");
        }
        else
        {
            node.ToDebug(sw);
        }
        sw.Write("}");
        return sw.ToString();
    }
}
