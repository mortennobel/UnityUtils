using UnityEngine;
using System.Collections.Generic;

// Create Delaynay triangulation of 2D points
public class DelaunayTriangulation2D
{
    public enum ProjectionPlane
    {
        XZ
    };

    public static double minVertexDistance = 0.002;

    public static ProjectionPlane projectPlane = ProjectionPlane.XZ;

    public static HMesh Triangulate (List<Vector3D> positions) {
		HMesh mesh = new HMesh();
		Stack<Halfedge> flipStack = new Stack<Halfedge>();

		CreateBoundingTriangle(mesh, positions);

        foreach (var pos in positions) {
			var triangle = FindTriangleWithinPoint(mesh, pos);
			AddEdgesToFlipStack(triangle, flipStack);
			bool inserted = InsertPointInTriangle(triangle, pos);
            if (!inserted)
            {
                continue;
            }
            while (flipStack.Count > 0){
				var e = flipStack.Pop();
                if (IsLocalDelaunay(e)) continue;
                var opp = e.opp;
                var check1 = opp.next;
                var check2 = check1.next;
                if (!e.FlipPrecondition()) continue;
                flipStack.Push(check1);
                flipStack.Push(check2);
                e.Flip();
            }
		}

		return mesh;
	}

	static bool IsLocalDelaunay(Halfedge e){
		if (e.IsBoundary()){
			return true;
		}
		var p1 = e.vert.positionD;
	    var p2 = e.next.vert.positionD;
	    var p3 = e.next.next.vert.positionD;
	    var p4 = e.opp.next.vert.positionD;
	    if (projectPlane == ProjectionPlane.XZ)
	    {
	        return HMeshMath.InCircleXZ(p1, p2, p3, p4) || HMeshMath.InCircleXZ(p2, p1, p4, p2);
	    }
	    else
	    {
	        Debug.LogError("Not implemented");
	        return false;
	    }
	}

	static bool InsertPointInTriangle(Face triangle, Vector3D point)
	{
	    var minVertexDistance2 = minVertexDistance * minVertexDistance;
	    foreach (var he in triangle.Circulate())
	    {
	        if ((he.vert.positionD - point).sqrMagnitude < minVertexDistance2)
	        {
	            return false;
	        }
	    }
		triangle.Split().positionD = point;
	    return true;
	}

	static List<Vertex> CreateBoundingTriangle(HMesh mesh, List<Vector3D> position) {
		BoundsD b = new BoundsD(position[0], Vector3D.zero);
		for (int i=1;i<position.Count;i++){
			b.Encapsulate(position[i]);
		}
		// encapsulate triangle
		b.center = b.center + b.extents*1.25f;
		b.extents = b.extents*40.0f;
		Vector3D v1 = b.min;
	    Vector3D v2 = b.min + Vector3D.right * b.size.x;
	    Vector3D v3;
	    if (projectPlane == ProjectionPlane.XZ)
	    {
	        // ensure normal pointing upwards
	        var tmp = b.min + Vector3D.forward * b.size.z;
	        v3 = v2;
	        v2 = tmp;
	    }
	    else
	    {
	        Debug.LogError("Not implemented");
	        v3 = v2;
	    }
	    Face face = mesh.CreateTriangle(v1, v2, v3);
		List<Vertex> boundingVertices = new List<Vertex>();
		foreach (var he in face.Circulate()){
			//Gizmos.DrawLine(he.vert.position, he.next.vert.position);
			boundingVertices.Add(he.vert);
		}
		return boundingVertices;
	}

	static void AddEdgesToFlipStack(Face f, Stack<Halfedge> flipStack){
		foreach (var he in f.Circulate()){
			flipStack.Push(he);
		}
	}

    static bool IsPointInTriangle(Face face, Vector3D pos)
    {
        var edges = face.Circulate();
        var p1 = edges[0].vert.positionD;
        var p2 = edges[1].vert.positionD;
        var p3 = edges[2].vert.positionD;

        if (projectPlane == ProjectionPlane.XZ)
        {
            if (HMeshMath.RightOfXZ(p1, p2, pos) && HMeshMath.RightOfXZ(p2, p3, pos) &&
                HMeshMath.RightOfXZ(p3, p1, pos))
            {
                return true;
            }
        }
        else
        {
            Debug.LogError("Not implemented");
        }
        return false;
    }

    static Face FindTriangleWithinPoint(HMesh mesh, Vector3D pos){
		foreach (var face in mesh.GetFacesRaw()){
		    if (IsPointInTriangle(face, pos))
		    {
		        return face;
		    }
		}
		return null;
	}

    public static HMesh TriangulateOrderedTriangles(List<Vector3D> triangles)
    {
        var mesh = Triangulate(triangles);

        string vectorsStr = "{";
        foreach (var t in triangles)
        {
            vectorsStr+="new Vector3D"+t.ToString("R")+",\n";
        }
        vectorsStr+="}";
        Debug.Log(vectorsStr);

        Bounds b = new Bounds();
        bool initialized = false;
        foreach (var vert in mesh.GetVerticesRaw())
        {
            if (vert.IsBoundary())
            {
                continue;
            }
            if (!initialized)
            {
                initialized = true;
                b = new Bounds(vert.position, Vector3.zero);
            }
            else
            {
                b.Encapsulate(vert.position);
            }
        }

        // build acceleration structure
        KDTree<Vertex> vertexTree = new KDTree<Vertex>(7, b);
        foreach (var vert in mesh.GetVerticesRaw())
        {
            if (vert.IsBoundary())
            {
                continue;
            }
            vertexTree.Insert(vert, new Bounds(vert.position,Vector3.zero));

        }
        Debug.Log(vertexTree.Debug());
        // enforce triangle flipping
        for (int i = 0; i < triangles.Count; i = i + 3)
        {
            var p0 = triangles[i];
            var p1 = triangles[i+1];
            var p2 = triangles[i+2];

            var v0 = FindClosestVertex(vertexTree,p0.ToVector3());
            var v1 = FindClosestVertex(vertexTree,p1.ToVector3());
            var v2 = FindClosestVertex(vertexTree,p2.ToVector3());

            if (v0 == null)
            {
                Debug.Log("Cannot find "+p0.ToString("R"));
            } else {
                Debug.Log(Vector3.Distance(p0.ToVector3(), v0.position));
            }
            if (v1 == null)
            {
                Debug.Log("Cannot find "+p1.ToString("R"));
            } else
            {
                Debug.Log(Vector3.Distance(p1.ToVector3(), v1.position));
            }
            if (v2 == null)
            {
                Debug.Log("Cannot find "+p2.ToString("R"));
            }
            else
            {
                Debug.Log(Vector3.Distance(p2.ToVector3(), v2.position));
            }

        }

        return mesh;
    }

    static Vertex FindClosestVertex(KDTree<Vertex> vertexTree, Vector3 pos)
    {
        var res = vertexTree.Query(new Bounds(pos, Vector3.one * (float)(minVertexDistance*1.000001)));
        double minDist = double.MaxValue;
        Vertex currentV = null;
        foreach (var v in res)
        {
            foreach (var vv in v.values)
            {
                double dist = Vector3.Distance(pos, vv.value.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    currentV = vv.value;
                }
            }
        }
        return currentV;
    }
}
