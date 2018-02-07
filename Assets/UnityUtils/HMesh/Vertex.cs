/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */
using System;
using UnityEngine;
using System.Collections.Generic;

public class Vertex : System.IEquatable<Vertex>, IBounds, IComparable<Vertex>
{

	private List<Halfedge> hes;
	
	// associates a HE with the vert. he may be null
	// This also updates the halfedge list of the vertex
	// Note that the vertex must point towards the vertex
	public void LinkHalfedge(Halfedge he)
	{
#if HMDebug		
		Debug.Assert(!hes.Contains(he));
#endif	
		hes.Add(he);
		
	}
	
	// associates a HE with the vert. he may be null
	// This also updates the halfedge list of the vertex
	public void UnlinkHalfedge(Halfedge he)
	{
		bool res = hes.Remove(he);
#if HMDebug		
		Debug.Assert(res);
#endif	
	}
	
	// return the next pointer of a ingoing halfedge
	public Halfedge halfedge
	{
		get
		{
			return hes.Count==0?null:hes[0].next;
		}
	}

	public Vector3 position{
		get{ 
			return positionD.ToVector3 ();
			}
		set{ 
			positionD = new Vector3D(value);
		}
	}

    public int Valency {
        get {
            return Circulate().Count;
        }
    }

	public List<Halfedge> CirculateAllIngoing()
	{
		return new List<Halfedge>(hes);
	}

	public readonly int id;
#if HMESH_CHECK_NAN
    public Vector3D positionD
    {
        get { return positionD_; }
        set
        {
            for (int i = 0; i < 3; i++)
            {
                if (double.IsNaN(value[i]))
                {
                    Debug.LogError("Set NaN value = "+value);
                }
            }
            positionD_ = value;
        }
    }

    public Vector3D positionD_;

#else
    public Vector3D positionD;
#endif

	public Vector3 uv1;
	public Vector3 uv2;

	public int label;

#if HMDebug
    private string stacktrace;
#endif
    public HMesh hmesh
    {
        get { return _hmesh; }
    }

    private HMesh _hmesh;

    // should only be called by HMesh.CreateVertex
	public Vertex(HMesh hmesh){
		hes = new List<Halfedge>();
		_hmesh = hmesh;
		position = Vector3.zero;
		uv1 = Vector2.zero;
		uv2 = Vector2.zero;
	    id = ++hmesh.vertexMaxId;
	}

	public override bool Equals (object obj)
	{
	    return obj == this;
	}

	// Replaces this vertex with another
	// does not destroy any vertex
	public void ReplaceVertex(Vertex vertex){
		Debug.Assert(vertex.hmesh == hmesh);
		foreach (var he in CirculateOpp()){
			he.vert = vertex;
		}
	}

    public List<Face> GetSharedFaces(Vertex v)
    {
        Debug.Assert(v.hmesh == hmesh);
        List<Face> localFaces = new List<Face>();
        foreach (var he in Circulate())
        {
            localFaces.Add(he.face);
        }
        List<Face> res = new List<Face>();
        foreach (var he in v.Circulate())
        {
            if (localFaces.Contains(he.face))
            {
                res.Add(he.face);
            }
        }
        return res;
    }

    // If boundary vertex Dissolve face.
    // Otherwise return false
    public bool Dissolve()
    {
        List<Halfedge> hes = Circulate();
	    if (!IsBoundary()) {
		    return false;
	    }
        if (hes.Count == 1) {
            var faceEdges = halfedge.face.Circulate();
            if (faceEdges.Count > 2)
            {
                Halfedge oldHe = halfedge;
                Halfedge prev = halfedge.prev;
                Halfedge next = halfedge.next;
                prev.Link(next);
                prev.vert = oldHe.vert;
                oldHe.face.halfedge = prev;
                hmesh.Destroy(oldHe);
                hmesh.Destroy(this);
            }
            else
            {
                halfedge.face.Dissolve2Edges();
            }
        } else if (hes.Count == 2) {
            foreach (var he in hes)
            {
                if (he.opp != null)
                {
                    var newPos = he.vert.positionD;
                    var vertex = he.Collapse();
                    vertex.positionD = newPos;
	                return true;
                }
            }
        } else {
	        // iterate each neighbour face
	        foreach (var faceHe in hes)
	        {
		        faceHe.face.Dissolve();
	        }
        }
	    return true;
    }

    public bool IsBoundary(){
		Halfedge iter = halfedge;
		bool first = true;
		while (iter != null && (iter != halfedge || first == true)){
			first = false;
			// goto next out-going halfedge
			iter = iter.opp;
			if (iter != null){
				iter = iter.next;
			}
		} 
		return iter == null;
	}

	// Circulate in-going halfedges
	public List<Halfedge> CirculateOpp()
	{
	    Debug.Assert(!IsDestroyed());
	    if (halfedge.opp != null){
	        return halfedge.opp.CirculateVertex();
	    } else if (halfedge.prev != null) {
	        return halfedge.prev.CirculateVertex();
	    } else {
	        Debug.LogError("Vertex does not have opp or prev ptr. It");
	        return new List<Halfedge>();
	    }
	}

	// Circulate out-going halfedges
	public List<Halfedge> Circulate(){
		Debug.Assert(!IsDestroyed(), "Vertex is destroyed");
		List<Halfedge> res = new List<Halfedge>();

		Halfedge iter = halfedge;
		bool first = true;
		int count = 0;
		while (iter != null && (iter != halfedge || first == true)){
			res.Add(iter);
			first = false;
			// goto next out-going halfedge
			iter = iter.opp;
			if (iter != null){
				iter = iter.next;
				count++;
				if (count > HMesh.MAX_SIZE)
				{
					throw new Exception("Invalid vertex - infinite iter");
				}
			}
			
		} 

		bool meetBoundary = iter == null;
		if (meetBoundary){
		    // circulate other way
		    iter = halfedge.prev;
		    iter = iter.opp;
			while (iter != null){
				res.Insert(0,iter);
				iter = iter.prev;
				iter = iter.opp;
				count++;
				if (count > HMesh.MAX_SIZE)
				{
					throw new Exception("Invalid vertex - infinite iter");
				}
			}
		}
		return res;
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

    public BoundsD GetBoundsD()
    {
        return new BoundsD(positionD, Vector3D.zero);
    }

    bool System.IEquatable<Vertex>.Equals(Vertex obj){
        return obj == this;
	}

	public bool IsValid(HMeshValidationRules validationRules = HMeshValidationRules.Standard){
		bool valid = true;
	    if (IsDestroyed())
	    {

	        Debug.LogWarning("Vertex is destroyed at \n "
#if HMDebug
	                         +stacktrace
#endif

	        );
	        valid = false;
	        return valid;
	    }
	    if (halfedge == null){
			Debug.LogWarning("Halfedge is null");
			valid = false;
			return valid;
		}
	    if (double.IsNaN( positionD.x)){
	        Debug.LogWarning("Position x is NaN");
	        valid = false;
	        return valid;
	    }
	    if (double.IsNaN( positionD.y)){
	        Debug.LogWarning("Position y is NaN");
	        valid = false;
	        return valid;
	    }
	    if (double.IsNaN( positionD.z)){
	        Debug.LogWarning("Position z is NaN");
	        valid = false;
	        return valid;
	    }
	    if (halfedge.IsDestroyed())
	    {
	        Debug.LogWarning("Vertex "+label+" Invalid he reference "+halfedge.label+". He is destroyed. "
#if HMDebug
	                         +halfedge.stacktrace
#endif
	        );
	        valid = false;
	    }
	    else if (halfedge.prev.vert != this) {
			Debug.LogWarning("Vertex "+id+" is not correctly associated with halfedge. Points to edge id "+halfedge.id+" "+halfedge.prev.vert.position+" to "+halfedge.vert.position);
			valid = false;
		}
        // test for invalid join
	    if ((validationRules&HMeshValidationRules.CheckForInvalidJoins)==HMeshValidationRules.CheckForInvalidJoins){
            var oppHes = CirculateOpp();
            foreach (var he in hmesh.GetHalfedgesRaw())
            {
                if (he.vert == this && !oppHes.Contains(he))
                {
                    string s = "";
                    foreach (var h in oppHes)
                    {
                        s += h.id + ", ";
                    }
                    Debug.LogWarning("Invalid mesh: Non manifold vertex (id "+id+") oppHes "+s+" he "+he.id);
                    valid = false;
                }
            }
	    }
	    return valid;
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

    public Halfedge GetSharedEdge(Vertex vertex)
    {
	    Debug.Assert(!IsDestroyed());
        Debug.Assert(hmesh == vertex.hmesh);
        foreach (var he in Circulate())
        {
            if (he.vert == vertex)
            {
                return he;
            }
        }
        // can happen on boundary edges
        foreach (var he in vertex.Circulate())
        {
            if (he.vert == this)
            {
                return he;
            }
        }
        return null;
    }

    public bool Connected(Vertex other)
    {
        return GetSharedEdge(other) != null;
    }

    public int CompareTo(Vertex other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return id.CompareTo(other.id);
    }

    public override string ToString()
    {
	    return "vertex{ id "+id+", pos "+position+"}";
    }
}
