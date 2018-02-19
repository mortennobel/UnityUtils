/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */
using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class Face : System.IEquatable<Face>, IBounds, IComparable<Face> {
	public Halfedge halfedge;
    public HMesh hmesh
    {
        get { return _hmesh; }
    }

    private HMesh _hmesh;

    public int label = 0;

    public readonly int id = 0;

    // should only be called by HMesh.CreateFace
	public Face(HMesh hmesh){
		_hmesh = hmesh;
	    id = ++hmesh.faceMaxId;
	}

	public bool HasBoundaryVertex()
	{
		foreach (var he in Circulate())
		{
			if (he.vert.IsBoundary())
			{
				return true;
			}
		}
		return false;
	}
	
	public bool HasBoundaryHalfedge()
	{
		foreach (var he in Circulate())
		{
			if (he.IsBoundary())
			{
				return true;
			}
		}
		return false;
	}
	
	public void SetDestroyed()
    {
        _hmesh = null;
    }

    public bool IsDestroyed()
    {
        return _hmesh == null;
    }

    public int NoEdges(){
		int count = 1;
		Halfedge current = halfedge.next;
		while (current != halfedge){
			count++;
			current = current.next;
		}
		return count;
	}

	public List<Halfedge> Circulate()
	{
	    Debug.Assert(!IsDestroyed());
		List<Halfedge> res = new List<Halfedge>();
		Halfedge iter = halfedge;
	    Debug.Assert(iter != null);
	    Debug.Assert(!iter.IsDestroyed());
		bool first = true;
		while (iter != halfedge || first){
			res.Add(iter);
			first = false;
			iter = iter.next;
		}
		return res;
	}

	public void ReassignFaceToEdgeLoop(){
		foreach (var he in Circulate()){
			he.Link(this);
		}
	}

    private bool SameSide(Vector3D p1, Vector3D p2, Vector3D a, Vector3D b, bool include = false)
    {
        var cp1 = Vector3D.Cross(b - a, p1 - a);
        var cp2 = Vector3D.Cross(b - a, p2 - a);
        if (include == false)
        {
            return Vector3D.Dot(cp1, cp2) > 0;
        } else {
            return Vector3D.Dot(cp1, cp2) >= 0;
        }
    }

    private bool PointInTriangle(Vector3D p, Vector3D a, Vector3D b, Vector3D c)
    {
        return SameSide(p,a, b,c) &&  SameSide(p,b, a,c)
        && SameSide(p, c, a, b);
    }

	private bool IsVerticesLinear()
	{
		List<Vector3D> list = new List<Vector3D>();
		double maxDist = 0;
		int maxIndexFrom = -1;
		int maxIndexTo = -1;
		var hes = Circulate();
		for (int i = 0; i < hes.Count; i++)
		{

			var pos = hes[i].vert.positionD;
			if (list.Count > 2)
			{
				for (int j = 0; j < list.Count; j++)
				{
					double dist = Vector3D.Distance(pos, list[j]);
					if (dist > maxDist)
					{
						maxDist = dist;
						maxIndexFrom = i;
						maxIndexTo = j;
					}
				}
			}

			list.Add(pos);
		}
		if (maxIndexFrom == maxIndexTo)
		{
			// all same position
			return true;
		}

		RayD r = new RayD(list[maxIndexFrom], list[maxIndexFrom] - list[maxIndexTo]);
		foreach (var p in list)
		{
			if (r.LinePointDistance(p) > hmesh.zeroMagnitudeTreshold)
			{
				// not linear
				return false;
			}
		}
		return true;
	
	}

	// Assumes that the vertices are located in the same plane
    // https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
    // Return null if triangulation does not succeed
	// can potential also delete edges (if edges of length 0 is found)
    public List<Face> Triangulate(bool step = false, StringWriter debug = null)
    {
#if HMDebug	    
	    if (debug != null)
	    {
		    debug.WriteLine("Triangulate face with debug");
	    }
#endif	
        Debug.Assert(!IsDestroyed());
        List<Face> res = new List<Face>();
        res.Add(this);

        var face = this;

	    bool isVerticesLinear = IsVerticesLinear();
#if HMDebug	    
	    if (debug!=null)
	    {
		    debug.WriteLine("isVerticesLinear "+isVerticesLinear);
	    }
#endif	
	    if (isVerticesLinear)
	    {
		    return TriangulateFaceOnLine();
	    }
	    var halfedges = Circulate();
	    var normal = GetNormal();

        int iterations = halfedges.Count * 2;
        while (halfedges.Count > 3 && iterations > 0)
        {
            iterations--;

            Halfedge zeroEdge = null;
            Halfedge degenerateEdge = null;
            Halfedge sharpCorner = null;
	        double minimumAngle = 999;

            // find ear to clip
            for (int i = 0; i < halfedges.Count; i++) {
                var he1 = halfedges[i];
                var he2 = he1.next;
                var dir1 = he1.GetDirection();
                var dir2 = he2.GetDirection();
	            Vector3D crossDir = Vector3D.Cross(dir1, dir2);
	            var angle = Vector3D.Angle(dir1, dir2);

                if (dir1.sqrMagnitude < face.hmesh.zeroMagnitudeTresholdSqr)
                {
                    zeroEdge = he1;
                    break;
                }

                bool isParallel = Vector3D.IsParallelDist(dir1, dir2, hmesh.zeroMagnitudeTresholdSqr);
                bool isSameDir = Vector3D.Dot(dir1, dir2) > 0;
                bool isNormalSameDir = Vector3D.Dot(crossDir, normal) > 0;
                if (isParallel && isSameDir == false)
                {
                    degenerateEdge = he1;
                    break;
                }
                bool intersection = false;
                if (isNormalSameDir && !isParallel){
                    for (int j = i + 2; j < i + halfedges.Count; j++)
                    {
                        var heJ = halfedges[j % halfedges.Count];
                        if (PointInTriangle(heJ.prev.vert.positionD, he1.prev.vert.positionD, he1.vert.positionD, he2.vert.positionD) ||
                            PointInTriangle(heJ.vert.positionD, he1.prev.vert.positionD, he1.vert.positionD, he2.vert.positionD))
                        {
                            intersection = true;
                            break; // break intersection test
                        }
                    }
                    // clip ear
                    if (intersection == false)
                    {
	                    if (angle < minimumAngle)
	                    {
		                    minimumAngle = angle;
                        	sharpCorner = he1;
	                    }
                    }
                }
            }

            if (zeroEdge != null)
            {
#if HMDebug	            
	            if (debug != null)
	            {
		            debug.WriteLine("zeroEdge "+zeroEdge);
	            }
#endif	
                zeroEdge.Collapse();
	            if (face.IsDestroyed())
	            {
		            Debug.LogError("Fix");
		            return new List<Face>(); // Fix
	            }
	            halfedges = face.Circulate();
            }
            else if (degenerateEdge != null)
            {
	            // the main idea, is to cut away the degenerate edge (and collapse the degenerate face)
	            // 
	            // 
	            //  Example (4 vertices)
	            //  ||
	            //  ||  
	            //  |\
	            //  | \
	            //  ----
#if HMDebug	            
	            if (debug != null)
	            {
		            debug.WriteLine("degenerateEdge "+degenerateEdge);
	            }
#endif	
                var he1 = degenerateEdge;
                var he2 = he1.next;
                // degenerate edges detected
                Halfedge next = he2.next;
	            double thisDist = he1.GetDirection().magnitude;
	            double nextDist = he2.GetDirection().magnitude;
	            Vertex nextVert = he2.vert;
                Vertex prevVert = he1.prev.vert;

	            if (Math.Abs(nextDist - thisDist) <= face.hmesh.zeroMagnitudeTreshold)
	            {
		            // cut from nextVert to prevVert
	            } else  if (nextDist > thisDist) {
                    nextVert = he2.Split();
                    nextVert.positionD = prevVert.positionD;
                }
                else if (thisDist > nextDist) {
                    prevVert = he1.Split();
                    prevVert.positionD = nextVert.positionD;
                }
                var cutFace = face.Cut(prevVert, nextVert);
                var sharedEdge = nextVert.GetSharedEdge(prevVert);
                
                sharedEdge.Collapse();
	            if (!cutFace.IsDestroyed())
	            {
#if HMDebug		            
		            Debug.LogWarning("Cut face survived \n"+cutFace.ExportLocalNeighbourhoodToObj());
#endif	
		            return new List<Face>(); // Fix
	            }
                
				halfedges = face.Circulate();
            }
            else if (sharpCorner != null)
            {
#if HMDebug	            
	            if (debug != null)
	            {
		            debug.WriteLine("sharpCorner ");
	            }
#endif	
                var he1 = sharpCorner;
                var he2 = he1.next;

                var newFace = face.Cut(he1.prev.vert, he2.vert);
                newFace.label = label;
                res.Add(newFace);
                if (newFace.Circulate().Count > 3)
                {
                    face = newFace;
                }
                halfedges = face.Circulate();
            }
            if (step)
            {
                return res;
            }
        }
        for (int i = res.Count - 1; i >= 0; i--)
        {
            if (res[i] == null || res[i].IsDestroyed())
            {
                res.RemoveAt(i);
            }
        }

        return res;
    }

	private List<Face> TriangulateFaceOnLine()
	{
		List<Face> res = new List<Face>();
		var hes = Circulate();
		// find vertex pair on the end. (only keep reference to one of them
		// Here use halfedge id to represent vertices (he->vert)
		int vertex1id = 0;
		double longestDist = 0;
		for (int i = 0; i < hes.Count; i++)
		{
			for (int j = i + 1; j < hes.Count; j++)
			{
				double dist = Vector3D.Distance(hes[i].vert.positionD, hes[j].vert.positionD);
				if (dist >= longestDist)
				{
					vertex1id = i;
					longestDist = dist;
				}
			}
		}
		// do simple triangulation
		List<Vertex> plannedSplitPairs = new List<Vertex>();
		for (int i = 2; i < hes.Count - 1; i++)
		{
			plannedSplitPairs.Add(hes[vertex1id].vert);
			plannedSplitPairs.Add(hes[(vertex1id+i)%hes.Count].vert);
		}
		Face multiPolyFace = this;
		res.Add(this);
		for (int i = 0; i < plannedSplitPairs.Count; i = i + 2)
		{
			var otherFace = multiPolyFace.Cut(plannedSplitPairs[i], plannedSplitPairs[i + 1]);
			res.Add(otherFace);
			if (otherFace.Circulate().Count > multiPolyFace.Circulate().Count)
			{
				multiPolyFace = otherFace;
			}
		}
		
		return res;
	}

	public double GetTriangleArea()
    {
        var heCirculate = Circulate();
        if (heCirculate.Count != 3)
        {
            Debug.LogError("Cannot compute area of "+heCirculate.Count+"-gon");
            return -1;
        }
        return Vector3D.Cross(heCirculate[0].GetDirection(), heCirculate[1].GetDirection()).magnitude / 2;
    }

    public Halfedge GetSharedHalfedge(Face otherFace)
    {
        foreach (Halfedge he in Circulate())
        {
            if (he.opp != null && he.opp.face == otherFace)
            {
                return he;
            }
        }
        return null;
    }

    // assumes that all vertices on a face is located in the same plane
    public Vector3D GetNormal()
    {
        var halfedges = Circulate();
        Vector3D sum = Vector3D.zero;
        if (halfedges.Count == 2)
        {
            return sum;
        }
        // circulate halfedges to find two vectors that are not
        foreach (var he in halfedges)
        {
            Vector3D direction1 = he.GetDirection();
            Vector3D direction2 = -he.prev.GetDirection();
	        double len1 = direction1.magnitude;
	        double len2 = direction2.magnitude;
	        if (len1 < float.Epsilon || len2 < float.Epsilon)
	        {
		        continue;
	        }
            sum += Vector3D.Cross(direction1/len1, direction2/len2);
        }
	    if (sum.sqrMagnitude < float.Epsilon)
	    {
		    return Vector3D.zero;
	    }
        return sum.normalized;
    }

    // Note that the face can be problematic in other ways (be non-planar, self-intersecting, etc.)
    public bool IsDegenerate()
    {
        return GetNormal() == Vector3D.zero;
    }

    // Where 'x', 'y' and 'z' describe a vector orthogonal to the plane, and 'w' is the distance of the plane from the origin as a multiple of the vector (x, y, z)
    public Plane3D GetPlaneEquation()
    {
        return new Plane3D(GetNormal(), halfedge.vert.positionD);
    }

    public Vector3D GetCenter()
    {
        Vector3D c = Vector3D.zero;
        var halfedges = Circulate();
        foreach (var he in halfedges)
        {
            c += he.vert.positionD;
        }

        return c / halfedges.Count;
    }

	// destroys linked edges (and vertices if no longer used)
	public void Dissolve()
	{
		var cir = Circulate();
		foreach (var c in cir)
		{
			var vert = c.vert;
			if (c.opp != null)
			{
				c.opp.opp = null;
			}
			hmesh.Destroy(c);
			if (vert.CirculateAllIngoing().Count == 0)
			{
				hmesh.Destroy(vert);	
			}
		}
		hmesh.Destroy(this);
	}

	// Remove (degenerate) face with two edges
	public bool Dissolve2Edges()
	{
		
		var hes = Circulate();
		if (hes.Count != 2)
		{
			return false;
		}
		// make vertex is not pointing to edge to be destroyed
		var verts = new[]
		{
			hes[0].vert,
			hes[1].vert
		};
		Halfedge.Glue(hes[0].opp, hes[1].opp);
		hmesh.Destroy(hes[0]);
		hmesh.Destroy(hes[1]);
		foreach (var v in verts)
		{
			if (v.CirculateAllIngoing().Count == 0)
			{
				hmesh.Destroy(v);	
			}	
		}
		hmesh.Destroy(this);
		
		return true;
	}

	/// <summary>
	/// Return true if the collapse results in a valid mesh (otherwise normals may be flipped)
	/// Return false if boundary face
	/// </summary>
	/// <returns></returns>
	public bool Collapse()
    {
	    var hm = hmesh;
        var hes = Circulate();
        
		var center = GetCenter();
		Halfedge opp = null;
		var firstVert = hes[0].vert;

	    List<Vector3D> potentialCollapsedPositions = new List<Vector3D>();
	    // faces that share edges with this face
		List<Face> adjacentFaces = new List<Face>();
	    
	    potentialCollapsedPositions.Add(center);
		foreach (var he in hes)
		{
			potentialCollapsedPositions.Add(he.vert.positionD);
			if (he.opp != null)
			{
				adjacentFaces.Add(he.opp.face);
			}
			if (he.vert.IsBoundary())
			{
				// cannot collapse boundary 
				return false;
			}
		}
	    adjacentFaces.Add(this);
	    
	    // faces that are affected by a collapse and their associated face normal
	    Dictionary<Face, Vector3D> affectedFaceNormals = new Dictionary<Face, Vector3D>();
	    foreach (var he in hes)
	    {
		    if (!he.vert.IsBoundary())
		    {
			    foreach (var halfedge in he.vert.Circulate())
			    {
				    var face = halfedge.face;
				    if (!adjacentFaces.Contains(face))
				    {
					    affectedFaceNormals.Add(face, face.GetNormal());
				    }
			    }
		    }
	    }
	    
	    // prepare removal/disconnect of face
	    foreach (var he in hes)
		{
			if (he.opp != null)
			{
				opp = he.opp;
				he.opp.opp = null; // remove reference to opp
			}
		}

		Vertex survivingVertes = null;
		
		foreach (var he in hes)
		{
			hm.Destroy(he);
		}
		hm.Destroy(this);
		if (opp != null)
		{
			var vert = opp.CollapseBoundaryLoop();
			bool validPosition = true;
			// find position where adjacent faces are not flipped
			for (int i = 0; i < potentialCollapsedPositions.Count; i++)
			{
				vert.positionD = potentialCollapsedPositions[i];
				validPosition = true;
				foreach (var faceNormal in affectedFaceNormals)
				{
					var newNormal = faceNormal.Key.GetNormal();
					bool isNormalFlipped = Vector3D.Dot(newNormal, faceNormal.Value) < 0; 
					if (isNormalFlipped)
					{
						validPosition = false;
					}
				}
				if (validPosition)
				{
					return true;
				}
				bool didNotSucceedInFindingValidPosition = (i == potentialCollapsedPositions.Count);
				if (didNotSucceedInFindingValidPosition)
				{
					// assume center position has least damage
					vert.positionD = potentialCollapsedPositions[i];
				}
			}
		}
	    return false;
    }

    // Cut the face in two parts. Assumes that the face is convex
	// The new face is always created as the one that contains vertices between v1 and v2
	public Face Cut(Vertex v1, Vertex v2){
		Debug.Assert(v1.hmesh == hmesh);
		Debug.Assert(v2.hmesh == hmesh);
		Debug.Assert(v1.halfedge != null);
		Debug.Assert(v2.halfedge != null);
		Debug.Assert(v1 != v2);
		Halfedge v1He = null;
		Halfedge v2He = null;
		var edges = Circulate();
		Debug.Assert(edges.Count > 3);
		foreach (var he in edges){
			if (he.vert == v1){
				v1He = he; 
			}
			else if (he.vert == v2){
				v2He = he;
			}
		}

		if (v1He == null || v2He == null){
		    if (v1He == null)
		    {
		        Debug.Log("Could not find first parameter");
		    }
		    if (v2He == null)
		    {
		        Debug.Log("Could not find second parameter");
		    }
		    return null;
		}
		Debug.Assert(v1He != v2He);
		if (v1He.prev.vert == v2)
		{
			Debug.LogWarning("face "+id+" cut adjacent vertices "+v1+" to "+v2);
			return this;
		}
		if (v2He.prev.vert == v1)
		{
			Debug.LogWarning("face "+id+" cut adjacent vertices "+v1+" to "+v2);
			return this;
		}
		Halfedge v1HeNext = v1He.next;
		Halfedge v2HeNext = v2He.next;
		Face f = hmesh.CreateFace();
		Halfedge he1 = hmesh.CreateHalfedge();
		Halfedge he2 = hmesh.CreateHalfedge();

		he1.Glue(he2);

		v1He.Link(he1);
		he1.Link(v2HeNext);
		v2He.Link(he2);
		he2.Link(v1HeNext);
		he1.vert = v2;
		he2.vert = v1;

		halfedge = he1;
		ReassignFaceToEdgeLoop();

		f.halfedge = he2;
		f.ReassignFaceToEdgeLoop();

		return f;
	}

	// Creates a new vertex inside the face and connect it to the cutFrom vertex using two halfedges.
	// Note that this function leaves the face in an invalid state, since the new halfedges will have face == opp.face.
	public Vertex CutInto(Vertex cutFrom, Vector3 pos){
		return CutInto (cutFrom, new Vector3D (pos));
	}

	public Vertex CutInto(Vertex cutFrom, Vector3D pos){
	    Debug.Assert(cutFrom.hmesh == hmesh);
		Halfedge cutFromEdge = null;
		foreach (var e in Circulate()){
			if (e.vert == cutFrom){
				cutFromEdge = e;
			}
		}
		if (cutFromEdge == null){
			Debug.LogError("Cannot find vertex in face");
			return null;
		}

		Vertex newVertex = hmesh.CreateVertex();
		newVertex.positionD = pos;
		Halfedge he1 = hmesh.CreateHalfedge();
		Halfedge he2 = hmesh.CreateHalfedge();
		he1.Glue(he2);

		Halfedge cutFromEdgeNext = cutFromEdge.next;
		cutFromEdge.Link(he1);
		he1.Link(he2);
		he2.Link(cutFromEdgeNext);

		he1.Link(this);
		he2.Link(this);
		he1.vert = newVertex;
		
		Debug.Assert(newVertex.halfedge != null);
		he2.vert = cutFrom;
		
		return newVertex;
	}

	// split the face in the center
	// return the new Vertex
	public Vertex Split(){
		Vertex v = hmesh.CreateVertex();

		List<Halfedge> newHalfedges = new List<Halfedge>();
		List<Vertex> vertices = new List<Vertex>();
		// create double halfedges
		foreach (var he in Circulate()){
			v.positionD += he.vert.positionD;
			v.uv1 += he.vert.uv1;
			v.uv2 += he.vert.uv2;

			Halfedge toNewVertex = hmesh.CreateHalfedge();
			Halfedge fromNewVertex = hmesh.CreateHalfedge();
			toNewVertex.Glue(fromNewVertex);
			newHalfedges.Add(toNewVertex);
			vertices.Add(he.vert);
		}
		int count = 0;

		// second iteration - link everything together
		foreach (var he in Circulate()){
			Vertex prevVertex = vertices[(count - 1 + newHalfedges.Count) % newHalfedges.Count];
			Vertex nextVertex = vertices[count];
			Halfedge prevNewOppEdge = newHalfedges[(count - 1 + newHalfedges.Count) % newHalfedges.Count].opp;
			Halfedge newEdge = newHalfedges[count];
			// link halfedges
			newEdge.Link(prevNewOppEdge);
			prevNewOppEdge.Link(he);
			he.Link(newEdge);

			// link vertices
			newEdge.vert = v;
			prevNewOppEdge.vert = prevVertex;

			bool first = count==0;
			Face f = he.face;
			if (!first){
				f = hmesh.CreateFace();
			    f.label = label;
			} 
			f.halfedge = he;
			f.ReassignFaceToEdgeLoop();

			count++;
		}

		v.positionD = v.positionD * (1.0/count); // set average position
		v.uv1 = v.uv1 * (1.0f/count);
		v.uv2 = v.uv2 * (1.0f/count);

		return v;
	}

	bool System.IEquatable<Face>.Equals(Face obj){
	    return obj == this;
	}

	public override bool Equals (object obj)
	{
	    return obj == this;
	}
	
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

    public BoundsD GetBoundsD()
    {
        if (IsDestroyed())
        {
            Debug.LogWarning("Face is destroyed");
        }
        /*if (!IsValid())
        {
            Debug.LogWarning("Face is invalid");
        }*/
        BoundsD res = new BoundsD(halfedge.vert.positionD, Vector3D.zero);
        foreach (var he in Circulate())
        {
            res.Encapsulate(he.vert.positionD);
        }
        return res;
    }

    public Bounds bound{
		get {
			var faces = Circulate();
			Bounds b = new Bounds(faces[0].vert.position, Vector3.zero);
			for (int i=1;i<faces.Count;i++){
				b.Encapsulate(faces[i].vert.position);
			}
			return b;
		}
	} 

	public bool IsValid(HMeshValidationRules validationRules = HMeshValidationRules.Standard){
		bool valid = true;
	    if (IsDestroyed())
	    {
	        Debug.LogWarning("Face is destroyed");
	        valid = false;
	    }
	    if (halfedge == null){
			Debug.LogWarning("Halfedge is null");
			valid = false;
		}
	    var circulate = Circulate();
	    if (circulate.Count == 2)
	    {
	        Debug.LogWarning("Degenerate face (2 vertices)");
	        valid = false;
	    }
		if (IsDegenerate())
		{
			Debug.LogWarning("Degenerate face "+this);
			valid = false;
		}
		if ((validationRules & HMeshValidationRules.CheckForTriangulationError) == HMeshValidationRules.CheckForTriangulationError)
	    {
	        if (circulate.Count != 3)
	        {
	            Debug.LogWarning("Invalid face - triangle expected but was "+circulate.Count+"-gon");
	            Debug.LogWarning(this);
	            
	            valid = false;
	        }
	    }
	    foreach (var he in circulate){
			if (he.face != this){
				Debug.LogWarning("Halfedge.face is not correct");
				valid = false;
			}
		}
		return valid;
	}

    public override string ToString()
    {
        string face = "Face " + id + " he's ";
        if (IsDestroyed())
        {
            face += "destroyed";
            return face;
        }
        var c = Circulate();
        foreach (var he in c)
        {
            face += he.id+", ";
        }
        face += " verts: ";
        foreach (var he in c)
        {
	        face += "("+he.vert.id +") "+ he.vert.positionD.ToString("R")+", ";
        }
        return face;
    }

	public string ExportLocalNeighbourhoodToObj()
	{
		StringWriter objWriter = new StringWriter();
		StringWriter objFaces = new StringWriter();
		objWriter.WriteLine("# HMesh export - neighbourhood of face "+this.id);
		HashSet<int> exportedFaces = new HashSet<int>();
		Dictionary<int,int> exportedVertices = new Dictionary<int,int> ();
		
		// circulate face edges
		foreach (var hes in Circulate())
		{
			// circulate face vertices (for outgoing halfedges)
			foreach (var vertHes in hes.vert.Circulate())
			{
				if (exportedFaces.Contains(vertHes.face.id))
				{
					continue;
				}
				exportedFaces.Add(vertHes.face.id);
				
				objFaces.WriteLine("# face id "+vertHes.face.id);
				objFaces.Write("f ");
				foreach (var faceEdge in vertHes.face.Circulate())
				{
					int vertexId;
					if (!exportedVertices.TryGetValue(faceEdge.vert.id, out vertexId))
					{
						vertexId = exportedVertices.Count + 1;
						exportedVertices[faceEdge.vert.id] = vertexId;
						objWriter.WriteLine("v {0} {1} {2}",faceEdge.vert.position.x,faceEdge.vert.position.y,faceEdge.vert.position.z);
					}
					objFaces.Write(vertexId+" ");
				}				
				objFaces.WriteLine();
			}
		}
		objWriter.Write(objFaces.ToString());
		return objWriter.ToString();
	}

	public int CompareTo(Face other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return id.CompareTo(other.id);
    }
}
