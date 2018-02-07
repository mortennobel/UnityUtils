/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */
using System;
using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Halfedge data structure
/// 
/// See:
/// https://www.openmesh.org/Daily-Builds/Doc/a00016.html
/// 
/// </summary>
public class Halfedge : System.IEquatable<Halfedge>, IBounds, IComparable<Halfedge>  {
	public Halfedge next;
	public Halfedge prev;
	public Halfedge opp;
	private Vertex _vert;

	public Vertex vert {
		get { return _vert; }
		set
		{
			if (_vert == value) return;
			
			if (_vert != null)
			{
				_vert.UnlinkHalfedge(this);
			}
			_vert = value;
			if (_vert != null)
			{
				_vert.LinkHalfedge(this);
			}
		}
	}

	public Face face;

	public int label;
    public readonly int id;

    public HMesh hmesh
    {
        get { return _hmesh; }
    }

    private HMesh _hmesh;
#if HMDebug
    public string  stacktrace;
#endif

    // should only be called by HMesh.CreateHalfEdge
	public Halfedge(HMesh hmesh){
		_hmesh = hmesh;
	    id = ++hmesh.halfedgeMaxId;
	}

    public void SetDestroyed()
    {
        _hmesh = null;
#if HMDebug
        stacktrace = StackTraceUtility.ExtractStackTrace();
#endif
    }

    public bool IsDestroyed()
    {
        return _hmesh == null;
    }

	// May return null if result in fully collapsed structure
    // Returns Vertex that survives the collapse
    public Vertex Collapse(bool center = false)
	{
		Debug.Assert(!IsDestroyed());
        Vector3D pos = (vert.positionD + prev.vert.positionD) * 0.5;
        Vector3 uv1 = (vert.uv1 + prev.vert.uv1) * 0.5f;
        Vector3 uv2 = (vert.uv2 + prev.vert.uv2) * 0.5f;

	    var res = Collapse(vert.positionD);
		if (res != null)
	    if (center)
	    {
	        res.positionD = pos;
	        res.uv1 = uv1;
	        res.uv2 = uv2;
	    }
	    return res;
	}
	
	[Flags]
	public enum CollapsePreconditionReason
	{
		Ok = 0,
		NormalFlipped 				= 1<<1,
		CollapsePointTooCloseToLine = 1<<2,
		EdgeIsBoundary 				= 1<<3,
		VertexIsBoundary 			= 1<<4,
		All = 0xff
	}

	public CollapsePreconditionReason CollapsePrecondition(Vector3D newPosition,CollapsePreconditionReason bits = CollapsePreconditionReason.All)
	{
		if ((bits & CollapsePreconditionReason.EdgeIsBoundary) == CollapsePreconditionReason.EdgeIsBoundary){
			if (IsBoundary())
			{
				return CollapsePreconditionReason.EdgeIsBoundary;
			}
		}
		if ((bits & CollapsePreconditionReason.VertexIsBoundary) == CollapsePreconditionReason.VertexIsBoundary)
		{
			if (vert.IsBoundary())
			{
				return CollapsePreconditionReason.VertexIsBoundary;
			}

			if (prev.vert.IsBoundary())
			{
				return CollapsePreconditionReason.VertexIsBoundary;
			}
		}
		var prevVert = prev.vert;
		Vector3D prevVertPos = prevVert.positionD;

		bool checkForNormalFlipped =
			(bits & CollapsePreconditionReason.NormalFlipped) == CollapsePreconditionReason.NormalFlipped;
		bool checkCollapsePointTooCloseToLine =
			(bits & CollapsePreconditionReason.CollapsePointTooCloseToLine) == CollapsePreconditionReason.CollapsePointTooCloseToLine;
		// test for flipped normal
		var hes = prevVert.Circulate();
		foreach (var he in hes)
		{
			if (he == this || he.prev.opp == this) continue;

			Vector3D dirA = he.vert.positionD - prevVertPos;
			Vector3D dirB = he.prev.prev.vert.positionD - prevVertPos;
			Vector3D dirC = Vector3D.Cross(dirA,dirB);

			Vector3D dirMovedA = he.vert.positionD - newPosition;
			Vector3D dirMovedB = he.prev.prev.vert.positionD - newPosition;
			Vector3D dirMovedC = Vector3D.Cross(dirMovedA,dirMovedB);

			if (checkForNormalFlipped){
				bool isNormalFlipped = Vector3D.Dot(dirC, dirMovedC) < 0;
				if (isNormalFlipped)
				{
					return CollapsePreconditionReason.NormalFlipped;
				}
			}
			if (checkCollapsePointTooCloseToLine){
				// test for tree points line up
				RayD ray = new RayD(he.vert.positionD, (he.vert.positionD-he.prev.prev.vert.positionD));
				double distToLine = ray.LinePointDistance(newPosition);
				if (distToLine < hmesh.zeroMagnitudeTreshold)
				{
					return CollapsePreconditionReason.CollapsePointTooCloseToLine;
				}
			}
		}

		return CollapsePreconditionReason.Ok;
	}

	public CollapsePreconditionReason CollapsePrecondition(bool center = false,CollapsePreconditionReason bits = CollapsePreconditionReason.All)
	{
		return CollapsePrecondition(center?GetCenter():vert.positionD,bits);
	}

    /// <summary>
    /// May return null if the collapse result in a fully collapsed structure
    /// Returns Vertex that survives the collapse (if any)
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vertex Collapse(Vector3D position)
    {
        Debug.Assert(!IsDestroyed());
        var vertToKeep = vert;
        var vertToDestroy = prev.vert;
#if HMDebug
        Debug.Assert(CollapsePrecondition(),"CollapsePrecondition failed");
#endif
        var oppHe = opp;
        var oppFace = opp != null?opp.face:null;
        var thisFace = face;
        var hm = hmesh;

	    var toReplace = vertToDestroy.CirculateAllIngoing();
	    toReplace.AddRange(vert.CirculateAllIngoing());

	    if (oppHe != null)
	    {
		    bool fullCollapseTwoTriangles = hmesh.GetVerticesRaw().Count == 4;
		    if (fullCollapseTwoTriangles)
		    {
			    hmesh.Clear();
			    return null;
		    }
		    oppHe.CollapseInternal(vertToKeep);
	    }
	    bool fullCollapseOneTriangle = hmesh.GetVerticesRaw().Count == 3;
	    if (fullCollapseOneTriangle)
	    {
		    hmesh.Clear();
		    return null;
	    }
        CollapseInternal(vertToKeep);
	    if (!vertToDestroy.IsDestroyed()){
	    	hm.Destroy(vertToDestroy);
	    }
	    
	    foreach (var he in toReplace)
	    {
	        if (!he.IsDestroyed())
	        {
	            he.vert = vertToKeep;
	        }
	    }
        
        vertToKeep.positionD = position;
        if (vertToKeep.halfedge == null || vertToKeep.halfedge.IsDestroyed())
        {
            // full collapse
	        if (vertToKeep.halfedge != null && !vertToKeep.halfedge.IsDestroyed()){
            	hm.Destroy(vertToKeep);
	        }
            vertToKeep = null;
        }
	    if (oppFace!=null && !oppFace.IsDestroyed()){
			
		    oppFace.Dissolve2Edges();
	    }
	    if (!thisFace.IsDestroyed()) thisFace.Dissolve2Edges();
	    return vertToKeep;
	}

    // collapses a halfedge. May result in a degenerate face with two edges
	void CollapseInternal(Vertex toKeep)
	{
        prev.vert = toKeep;
        face.halfedge = next;
        if (opp != null){
            opp.opp = null; // break opp connection
        }
        prev.Link(next);
	    hmesh.Destroy(this);

	}

    /// <summary>
    /// Return the unnormalized direction of the halfedge
    /// </summary>
    /// <returns></returns>
    public Vector3D GetDirection()
    {
        return vert.positionD - prev.vert.positionD;
    }


    public bool FlipPrecondition()
    {

        Face hf = face;
        Face hof = opp == null?null: opp.face;

        // boundary case
        if(hf == null || hf.IsDestroyed() || hof == null || hof.IsDestroyed())
            return false;


        // We can only flip an edge if both incident polygons are triangles.
        var circThis = hf.Circulate();
        var circOther = hf.Circulate();
        if(circThis.Count != 3 || circOther.Count != 3)
            return false;


        // non boundary vertices with a valency of less than 4(less than 3 after operation) degenerates mesh.
        var hv = vert;
        var hov = opp.vert;
        if(hv.Valency < 4 && !hv.IsBoundary()  || hov.Valency < 4 && !hov.IsBoundary()){
            return false;
        }

        // Disallow flip if vertices being connected already are.
        var hnv = next.vert;
        var honv = opp.next.vert;
        if (hnv.Connected(honv)){
            return false;
        }

        return true;
    }


    public void Flip(){
		Halfedge oldNext = next;
		Halfedge oldPrev = prev;
		Halfedge oldOppNext = opp.next;
		Halfedge oldOppPrev = opp.prev;

		Vertex thisVert = vert;
		Vertex oppVert = opp.vert;
		Vertex thisOppositeVert = next.vert;
		Vertex oppOppositeVert = opp.next.vert;

		// flip halfedges
		oldOppNext.Link(this);
		this.Link(oldPrev);

		oldNext.Link(opp);
		opp.Link(oldOppPrev);

		oldOppPrev.Link(oldNext);
		oldPrev.Link (oldOppNext);

		// reassign vertices
		this.vert = thisOppositeVert;
		opp.vert = oppOppositeVert;

		face.halfedge = this;
		face.ReassignFaceToEdgeLoop();
		opp.face.halfedge = opp;
		opp.face.ReassignFaceToEdgeLoop();

	}

	public bool IsBoundary(){
		return opp==null;
	}

	// Set next reference to nextEdge and nextEdge.prev to this
	public void Link(Halfedge nextEdge){
	    Debug.Assert(nextEdge.hmesh == hmesh);
	    if (this == nextEdge){
	        Debug.LogError(hmesh.DebugFaceNeighbourhood(face));
			throw new UnityException("Link of self he.id "+id);
		}
		next = nextEdge;
		nextEdge.prev = this;
	}

	public void Link(Face newFace){
		Debug.Assert(newFace.hmesh == hmesh);
		face = newFace;
		face.halfedge = this;
	}

    // Split and edge and cut adjacent faces with the new vertex.
    // E.g. splitting a shared on two triangles with result in four triangles
    public Vertex SplitAndCut(out Face[] newFaces,double splitFraction  = 0.5f)
    {
        List<Face> existingFaces = new List<Face>();
        existingFaces.Add(face);
        if (opp != null)
        {
            existingFaces.Add(opp.face);
        }
        var res = SplitAndCut(splitFraction);
        List<Face> newFacesList = new List<Face>();
        foreach (var he in res.Circulate())
        {
            if (!existingFaces.Contains(he.face))
            {
                newFacesList.Add(he.face);
            }
        }
        newFaces = newFacesList.ToArray();
        return res;

    }

    // Split and edge and cut adjacent faces with the new vertex.
    // E.g. splitting a shared on two triangles with result in four triangles
    public Vertex SplitAndCut(double splitFraction = 0.5f)
    {
        List<Vertex> connectToVertices = new List<Vertex>();

        var he = next;
        while (he != this)
        {
            if (he.next != this)
            {
                connectToVertices.Add(he.vert);
            }
            he = he.next;
        }
        if (opp != null)
        {
            he = opp.next;
            while (he != opp)
            {
                if (he.next != opp)
                {
                    connectToVertices.Add(he.vert);
                }
                he = he.next;
            }
        }

        var res = Split(splitFraction);
        foreach (var otherVert in connectToVertices)
        {
            var faces = res.GetSharedFaces(otherVert);
            if (faces.Count != 1)
            {
                Debug.LogError("Expected to have one shared face!");
                continue;
            }
            faces[0].Cut(res, otherVert);
        }
        return res;
    }

    public Vertex Split(double splitFraction = 0.5f){
		Vertex vertex = hmesh.CreateVertex();
		vertex.positionD = Vector3D.Lerp(prev.vert.positionD,vert.positionD,splitFraction);
		vertex.uv1 = Vector2.Lerp(prev.vert.uv1,vert.uv1,(float)splitFraction);
		vertex.uv2 = Vector2.Lerp(prev.vert.uv2,vert.uv2,(float)splitFraction);
		var newHE = SplitInternal(vertex);
#if HMDebug
	    Debug.Assert(newHE.IsValid());
		Debug.Assert(vertex.IsValid());
#endif
		return vertex;
	}

	// glue two halfedges together
	public void Glue(Halfedge oppEdge){
		Debug.Assert(oppEdge.hmesh == hmesh);
	    Debug.Assert(oppEdge != this,"Glue to self");

		opp = oppEdge;
		oppEdge.opp = this;
	}

    // Glue two halfedges together, either can be null
    public static void Glue(Halfedge h1, Halfedge h2)
    {
        if (h1 == null && h2 == null)
        {
            //"No glue";
        }
        else if (h1 == null)
        {
            h2.opp = null;
            //"set halfedge " + h2.id + " opp to null";
        }
        else if (h2 == null)
        {
            h1.opp = null;
            //"set halfedge " + h1.id + " opp to null";
        }
        else
        {
            h1.Glue(h2);
            //"glue " + h1.opp.id+ " to "+h2.opp.id;

        }
    }

    public override bool Equals (object obj)
	{
	    return obj == this;
	}

    bool System.IEquatable<Halfedge>.Equals(Halfedge obj){
        return obj == this;
    }

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

    public BoundsD GetBoundsD()
    {
        BoundsD res = new BoundsD(vert.positionD, Vector3D.zero);
        res.Encapsulate(prev.vert.positionD);
        return res;
    }

    public Bounds GetBounds()
    {
        Bounds res = new Bounds(vert.position, Vector3.zero);
        res.Encapsulate(prev.vert.position);
        return res;
    }


    private static Halfedge NextBoundaryEdge(Halfedge he){
	    
        var h = he;
	    while (h.next.opp != null)
	    {
		    h = h.next.opp;
		    Debug.Assert(h != he);
	    }
	    return h.next;
    }

    public List<Halfedge> CirculateBoundary()
    {
        List<Halfedge> res = new List<Halfedge>();
        if (!IsBoundary())
        {
            return res;
        }
        res.Add(this);
        Halfedge edge = NextBoundaryEdge(this);
        while (edge != this) {
	        res.Add(edge);
            edge = NextBoundaryEdge(edge);
	        if (res.Count > HMesh.MAX_SIZE)
	        {
		        Debug.LogWarning("Infinite loop detected");
		        return null;
	        }
        }
        return res;
    }

	public Vertex CollapseBoundaryLoop()
	{
		var hm = hmesh;
		var boundary = CirculateBoundary();
		var faces = new List<Face>();
		var verticesToDestroy = new List<Vertex>();
		if (boundary.Count == 0) return null;

		var heToVertex = new List<Halfedge>();
		
		Vector3D center = Vector3D.zero;

		var vert = boundary[0].vert;
		
		foreach (var e in boundary)
		{
			center += e.vert.positionD * (1.0 / boundary.Count);
			faces.Add(e.face);
			heToVertex.AddRange(e.vert.CirculateOpp());
			if (e.vert != vert)
			{
				verticesToDestroy.Add(e.vert);
			}
		}
		
		foreach (var e in boundary)
		{
			e.prev.Link(e.next);
			e.prev.face.halfedge = e.prev;
		}
		foreach (var incommingHe in heToVertex)
		{
			incommingHe.vert = vert;
		}
		
		vert.positionD = center;
		foreach (var v in verticesToDestroy)
		{
			if (v.CirculateAllIngoing().Count == 0)
			{
				hm.Destroy(v);
			}
		}
		
		foreach (var f in faces)
		{
			if (!f.IsDestroyed()) f.Dissolve2Edges();
		}
		
		foreach (var e in boundary)
		{
			if (!e.IsDestroyed()) hm.Destroy(e);
		}
		return vert;
	}

	// Circulate all halfedges around vert
    // returns halfedges pointing towards vert
    public List<Halfedge> CirculateVertex()
    {
        List<Halfedge> res = new List<Halfedge>();
        Halfedge iter = this;
        bool first = true;
        while (iter != null && (iter != this || first)){
            res.Add(iter);
            first = false;
            // goto next out-going halfedge
            iter = iter.opp;
            if (iter != null){
                iter = iter.prev;
            }
        }

        bool meetBoundary = iter == null;
        if (meetBoundary){
            // circulate other way
            iter = next.opp;
            while (iter != null){
#if HMDebug
                Debug.Assert(!res.Contains(iter));
#endif
                res.Insert(0,iter);
                iter = iter.next;
                iter = iter.opp;
            }
        }
        return res;
    }

    Halfedge SplitInternal(Vertex vertex){
		Debug.Assert(vertex.hmesh == hmesh);
		Halfedge oldNext = next;
		Vertex oldVert = vert;

		Halfedge splitEdge1 = hmesh.CreateHalfedge();

		if (opp != null){
			Vertex oppositeVert = prev.vert;
			Halfedge oppOldPrev = opp.prev;

			Halfedge splitEdge2 = hmesh.CreateHalfedge();
			splitEdge1.Glue(splitEdge2);

			splitEdge2.vert = vertex;
			oppOldPrev.Link(splitEdge2);
			splitEdge2.Link(opp.face);
			splitEdge2.Link(opp);
		}

		// link face
		splitEdge1.Link(face);

		splitEdge1.Link(oldNext);

		// link vertex
		splitEdge1.vert = oldVert;

		vert = vertex;
		Link(splitEdge1);

		return splitEdge1;
	}

	public bool IsValid(HMeshValidationRules validationRules = HMeshValidationRules.Standard){
		bool valid = true;
	    if (IsDestroyed())
	    {
	        Debug.LogWarning("Halfedge is destroyed");
	        valid = false;
	        return valid;
	    }
		
		
	    if (opp == null)
	    {
		    var bounds = CirculateBoundary();
		    if (bounds == null)
		    {
			    Debug.LogWarning("Invalid bounds - too long bound loop : max value "+HMesh.MAX_SIZE);
			    valid = false;
		    } else  if (bounds.Count <= 2){
				Debug.LogWarning("Boundary is only "+bounds.Count+" edges long");
				valid = false;
		    }
	    }

	    if (opp != null && opp.opp != this){
			Debug.LogWarning("opp is different from this or null");
			valid = false;
		}
		if (opp != null && vert == opp.vert){
			Debug.LogWarning("opp is has same vertex as this");
			valid = false;
		}
		if (opp != null && face == opp.face){
			Debug.LogWarning("warn: opp is has same face as this");
		    valid = false;
		}
		if (prev.next != this){
			Debug.LogWarning("warn: prev.next is different from this");
		    valid = false;
		}
		if (next.prev != this){
			Debug.LogWarning("warn: next.prev is different from this");
		    valid = false;
		}
		if (prev.vert == vert)
		{
			Debug.LogWarning("warn: prev.vert == vert");
			valid = false;
		}

		if (prev == null)
	    {
	        Debug.LogWarning("Prev == null");
	        valid = false;
	    }
	    if (next == null)
	    {
	        Debug.LogWarning("Next == null");
	        valid = false;
	    }
	    if ((validationRules & HMeshValidationRules.EdgeCheckForZeroLength) ==  HMeshValidationRules.EdgeCheckForZeroLength){
            if (GetDirection().sqrMagnitude < hmesh.zeroMagnitudeTresholdSqr)
            {
                Debug.LogWarning("Edge with zero length");
                valid = false;
            }
	    }

	    return valid;
	}

    public Vector3D GetCenter()
    {
        return (prev.vert.positionD + vert.positionD)*0.5;
    }

    public override string ToString()
    {
        return "HE from vertId " + prev.vert.id + " (" + prev.vert.position + ") to vertId " + vert.id + " (" +
               vert.position + ")";
    }

    public int CompareTo(Halfedge other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return id.CompareTo(other.id);
    }
}
