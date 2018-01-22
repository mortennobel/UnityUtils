
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Optimizes a HMesh in order to reduce the the vertex count, remove problematic triangles (e.g. needles).
/// </summary>
public static class HMeshSimplification {

    const bool debugOneStep = false;

    public static void RemoveFacesWithTwoEdges(HMesh hmesh)
    {
        var faces = new List<Face>(hmesh.GetFaces());
        for (int i=0;i<faces.Count;i++){// (var face in hmesh.GetFaces())
            var edges = faces[i].Circulate();
            if (edges.Count == 2)
            {
                var opp1 = edges[0].opp;
                var opp2 = edges[1].opp;
                var vert1 = edges[0].vert;
                var vert2 = edges[1].vert;
                // reassign vertex halfedges to a he not to be destroyed
                Halfedge.Glue(opp1, opp2);
                hmesh.Destroy(faces[i]);
                hmesh.Destroy(edges[0]);
                hmesh.Destroy(edges[1]);
            }
        }
    }

    // Dissolves adjacent edges when face normals are equal. Adjecent vertices are dissolved
    /*public static int RemoveAdjecentEdges(HMesh hmesh, double thresholdDistance = 0.001, double thresholdAngle = 0.1)
    {
        bool changed = false;
        int count = 0;
        do
        {
            changed = false;
            var heCopy = new List<Halfedge>(hmesh.GetHalfedgesRaw());
            for (int i = 0; i < heCopy.Count; i++)
            {
                var he1 = heCopy[i];
                if (he1.IsDestroyed())
                {
                    continue;
                }
                for (int j = i + 1; j < heCopy.Count; j++)
                {
                    var he2 = heCopy[j];
                    if (he2.IsDestroyed())
                    {
                        continue;
                    }
                    if (he1.opp == he2)
                    {
                        continue;
                    }
                    var distVert1 = he1.prev.vert.positionD - he2.vert.positionD;
                    var distVert2 = he1.vert.positionD - he2.prev.vert.positionD;

                    bool isAdjacentHalfedges = distVert1.magnitude < thresholdDistance &&
                                               distVert2.magnitude < thresholdDistance;
                    if (isAdjacentHalfedges){
                        var he1Normal = he1.face.GetNormal();
                        var he2Normal = he2.face.GetNormal();
                        bool isSameNormal = Vector3D.Angle(he1Normal, he2Normal) < thresholdAngle;
                        if (isSameNormal)
                        {
                            changed = true;

                            //Debug.Log("Joining he "+he1.id+" and "+he2.id);
                            //Debug.Log("Joining he "+he1.face.ToString() +" and "+he2.face.ToString());

                            var he2Vert = he2.vert;
                            var he2PrevVert = he2.prev.vert;
                            //Debug.Log("Replace "+he2Vert.id+" with "+he1.prev.vert.id);
                            //Debug.Log("Replace "+he2PrevVert.id+" with "+he1.vert.id);
                            he2Vert.ReplaceVertex(he1.prev.vert);
                            he2PrevVert.ReplaceVertex(he1.vert);

                            //Debug.Log("Destroy vert "+he2Vert.id);
                            //Debug.Log("Destroy vert "+he2PrevVert.id);
                            hmesh.Destroy(he2Vert);
                            hmesh.Destroy(he2PrevVert);

                            he1.Glue(he2);
                            //hmesh.IsValid(HMeshValidationRules.All);
                            //he1.Dissolve();
                            hmesh.IsValid(HMeshValidationRules.All);
                            count++;
                        }
                    }
                }
            }
        } while (changed);
        return count;
    }*/

    /// <summary>
    /// Enforces triangular mesh
    /// </summary>
    /// <param name="hmesh"></param>
    /// <returns></returns>
    public static int FixDegenerateFaces(HMesh hmesh)
    {
        int count = 0;
        var faces = new List<Face>(hmesh.GetFaces());
        for (int i=0;i<faces.Count;i++){// (var face in hmesh.GetFaces())}
            var face = faces[i];
            //var faceWas = face.ToString();
            if (face.IsDestroyed()) continue;

            // triangulate
            if (face.Circulate().Count > 3)
            {
                count++;
#if HMDebug                
                var str = face.ToString();
                var debugFaceExp = face.ExportLocalNeighbourhoodToObj();
                StringWriter sw = new StringWriter();
                var res = face.Triangulate(false, sw);
#else
                var res = face.Triangulate();
#endif
                if (res.Count == 0)
                {
#if HMDebug                                    
                    Debug.LogWarning("Cannot triangulate "+str+" face is now "+face.ToString()+" "+sw.ToString());
                    Debug.LogWarning(debugFaceExp);
#endif
                    // face destroyed
                    continue;
                }
                // add new faces
                for (int j = 0; j < res.Count; j++)
                {
                    if (res[j] != face)
                    {
                        faces.Add(res[j]);
                    }
                }
            }
            
            // fix degenerate due to zero length edge
            foreach (var he in face.Circulate())
            {
                if (he.IsDestroyed()) continue;

                if (he.GetDirection().sqrMagnitude <= hmesh.zeroMagnitudeTresholdSqr)
                {
                    Vector3D[] positions = { he.GetCenter(), he.vert.positionD, he.prev.vert.positionD};
                    bool collapsed = false;
                    foreach (var p in positions)
                    {
                        var collapsePrecondition = he.CollapsePrecondition(p, Halfedge.CollapsePreconditionReason.EdgeIsBoundary|Halfedge.CollapsePreconditionReason.VertexIsBoundary|Halfedge.CollapsePreconditionReason.NormalFlipped); 
                        if ( collapsePrecondition == Halfedge.CollapsePreconditionReason.Ok)
                        {
                            count++;
                            he.Collapse();
                            collapsed = true;
                            break;
                        }
                        else
                        {
                            Debug.Log("Cannot collapse - precondition failed " + collapsePrecondition);
                        }    
                    }
                    if (!collapsed)
                    {
                        he.Collapse();
                    }
                }
            }
        }

        RemoveFacesWithTwoEdges(hmesh);

        // fix degenerate due to zero area
        faces = new List<Face>(hmesh.GetFaces());
        for (int i=faces.Count-1;i>=0;i--){// (var face in hmesh.GetFaces())}
            var face =faces[i];
            if (face.IsDestroyed()) continue;

            if (face.IsDegenerate())
            {
                // find longest edge
                Halfedge longestEdge = null;
                double maxLength=-1;

                var edges = face.Circulate();
                
                Debug.Assert(edges.Count == 3,"Edges was "+edges.Count);
                
                foreach (var he in edges)
                {
                    double length = he.GetDirection().sqrMagnitude;
                    if (length > maxLength)
                    {
                        maxLength = length;
                        longestEdge = he;
                    }
                }
                if (longestEdge == null)
                {
                    Debug.LogError("Face "+face.id+" only has zero length edges");
                    continue;
                }
                var oppVert = longestEdge.next.vert;
                Vertex oppHeOppVert = null;
                if (longestEdge.opp != null)
                {
                    oppHeOppVert = longestEdge.opp.next.vert;
                }
                var newVert = longestEdge.Split();
                newVert.positionD = oppVert.positionD;
                var longestEdgeFace = longestEdge.face;
                var res = longestEdgeFace.Cut(oppVert, newVert);

                if (res == longestEdgeFace)
                {
                    Debug.Log("vertices");
                }

                if (oppHeOppVert != null)
                {
                    var longestEdgeOppFace = longestEdge.opp.face;  
                    var newFace = longestEdgeOppFace.Cut(newVert, oppHeOppVert);
                    faces.Add(newFace);
                    faces.Add(longestEdgeOppFace); // reevaluate face
                }
                var sharedEdge = oppVert.GetSharedEdge(newVert);
                if (sharedEdge == null)
                {
                    Debug.Log("Cannot find shared edge between "+oppVert+" "+newVert);
                    continue;
                }
                var sharedEdgeFace1 = sharedEdge.face;
                var sharedEdgeFace2 = sharedEdge.opp.face; // ensured to exist (just created)
                var precond = sharedEdge.CollapsePrecondition(true,Halfedge.CollapsePreconditionReason.NormalFlipped);
                if (precond == Halfedge.CollapsePreconditionReason.Ok)
                {
                    sharedEdge.Collapse(true);
                    Debug.Assert(sharedEdgeFace1.IsDestroyed());
                    Debug.Assert(sharedEdgeFace2.IsDestroyed());
                    count++;
                }
                else
                {
                    Debug.Log(precond);
                }
            }
        }
        return count;
    }


    // <summary>
    // Collapses boundary edges, such any point of the shape does not move more than (approx) hmesh.zeroMagnitudeTreshold.
    // </summary>
    public static int DissolveUnneededBoundaryVertices(HMesh hmesh, bool keepFaceLabels = true)
    {
        Debug.Assert(hmesh.IsValid(HMeshValidationRules.All),"Precondition failed: Mesh should be valid");
        
        List<Halfedge> boundaryHalfedges = new List<Halfedge>();
        var count = 0;
        var verts = new List<Vertex>(hmesh.GetVerticesRaw());
        foreach (var v in verts)
        {
            if (v.IsDestroyed())
            {
                continue;
            }
            if (v.IsBoundary())
            {
                var hes = v.Circulate();
                var heOpps = v.CirculateOpp();

                boundaryHalfedges.Clear();

                bool isSameLabel = true;

                foreach (var he in hes)
                {
                    if (keepFaceLabels && he.face.label != hes[0].face.label)
                    {
                        isSameLabel = false;
                        break;
                    }
                    if (he.IsBoundary())
                    {
                        boundaryHalfedges.Add(he);
                    }
                }

                foreach (var he in heOpps)
                {
                    if (keepFaceLabels && he.face.label != hes[0].face.label)
                    {
                        isSameLabel = false;
                        break;
                    }
                    if (he.IsBoundary())
                    {
                        boundaryHalfedges.Add(he);
                    }
                }
                if (!isSameLabel)
                {
                    continue;
                }
                double largeThreshold = hmesh.zeroMagnitudeTresholdSqr;
                if (!Vector3D.IsParallelDist(boundaryHalfedges[0].GetDirection(), boundaryHalfedges[1].GetDirection(),
                    largeThreshold))
                {
                    continue;
                }
                
                // find face plane that are not degenerate
                bool facesAreOnSamePlane = IsFacesOnSamePlane(hmesh, hes);
                if (!facesAreOnSamePlane)
                {
                    continue;
                }

                LineSegment virtuelEdge = new LineSegment(
                    boundaryHalfedges[0].vert.positionD,
                    boundaryHalfedges[1].prev.vert.positionD
                );
                if (virtuelEdge.DistancePoint(v.positionD) < hmesh.zeroMagnitudeTreshold)
                {
                    boundaryHalfedges[0].Collapse(boundaryHalfedges[0].vert.positionD);
                    count++;
                }
            }
        }
        return count;
    }

    private static bool IsFacesOnSamePlane(HMesh hmesh, List<Halfedge> hes, int faceLabel = -1)
    {
        bool facesAreOnSamePlane = true;
        Plane3D plane = new Plane3D(Vector3D.zero, 0);
        foreach (var he in hes)
        {
            if (faceLabel != -1 && he.face.label != faceLabel)
            {
                continue;
            }
            if (!he.face.IsDegenerate())
            {
                plane = he.face.GetPlaneEquation();
                break;
            }
        }
        if (plane.normal == Vector3D.zero)
        {
            Debug.Assert(false, "Warning - face is degenerate");
            return false;
        }


        // allowing the center of the face to move maximum 50% of zeroMagnitudeTreshold (to the plane of an arbitary face
        double planeDistThreshold = hmesh.zeroMagnitudeTreshold * 0.5;
        foreach (Halfedge he in hes)
        {
            if (faceLabel != -1 && he.face.label != faceLabel)
            {
                continue;
            }
            if (plane.GetDistanceToPoint(he.GetCenter()) > planeDistThreshold)
            {
                facesAreOnSamePlane = false;
                break;
            }
        }
        return facesAreOnSamePlane;
    }

    private static double VertexDistOnDissolve(Vertex toDissolve, Vertex neighbour1, Vertex neighbour2)
    {
        RayD r = new RayD(neighbour1.positionD, neighbour2.positionD-neighbour1.positionD);
        return r.LinePointDistance(toDissolve.positionD);
    }

    //
    //   -----o------
    //
    // Remove boundary vertices with two edge (one ingoing + one outgoing) where edges are parallel
    // Remove non-boundary vertices with only two adjacent faces where edges are parallel
    //
    // The method undo edge split by removing the extra vertex
    public static int DissolveUnneededVertices(HMesh hmesh)
    {
        int count = 0;
        foreach (var vert in hmesh.GetVertices())
        {
            var circ = vert.Circulate();

            double parallelEpsilon = 1E-15;

            if (vert.IsBoundary() && circ.Count == 1 && VertexDistOnDissolve(vert, circ[0].vert, circ[0].prev.prev.vert) < hmesh.zeroMagnitudeTreshold)
            {
                vert.Dissolve();
                count++;
            }
            else if (!vert.IsBoundary() && circ.Count == 2)
            {
                if (VertexDistOnDissolve(vert, circ[0].vert, circ[1].vert) < hmesh.zeroMagnitudeTreshold){
                    vert.Dissolve();
                    count++;
                }
            }
        }
        return count;
    }

    // return the number of collapsed faces
    public static int SimplifyByCollapse(HMesh hmesh, int maxIter = int.MaxValue)
    {
        List<Halfedge> faceLabelBoundary = new List<Halfedge>();
        int collapsed = 0;

        bool changed;
        int iter = 0;
        int collapsedEven = 0;
        do
        {
            iter++;
            changed = false;
            // collapse he if vertex contains two parallel halfedges, which separates labels or is boundary)
            foreach (var vert in hmesh.GetVertices())
            {
                if (vert.IsDestroyed())
                {
                    continue;
                }
                if (vert.IsBoundary())
                {
                    // boundary collapses should be handled by DissolveUnneededBoundaryVertices
                    continue;
                }
                faceLabelBoundary.Clear();
                var vertCirculate = vert.Circulate();
                foreach (var he in vertCirculate)
                {
                    if (he.face.label != he.opp.face.label)
                    {
                        faceLabelBoundary.Add(he);
                    }
                }
                if (faceLabelBoundary.Count == 1) {
                    Debug.Assert(false, "Cannot have a single face label boundary (without the vertex is a boundary");
                }
                else if (faceLabelBoundary.Count == 2)
                {
                    var dirs = new[]{faceLabelBoundary[0].GetDirection(),faceLabelBoundary[1].GetDirection()};
                    var line = new LineSegment(faceLabelBoundary[0].vert.positionD, faceLabelBoundary[1].vert.positionD);
                    double distance = line.DistancePoint(faceLabelBoundary[0].prev.vert.positionD);
                    double distThreshold = hmesh.zeroMagnitudeTreshold;
                    var dot = Vector3D.Dot(dirs[0], dirs[1]);
                    bool sameDir =  dot < 0.0;
                    if (distance < distThreshold  && sameDir)
                    {
                        var position = faceLabelBoundary[0].vert.positionD;

                        if (IsFacesOnSamePlane(hmesh,vertCirculate,faceLabelBoundary[0].face.label) && IsFacesOnSamePlane(hmesh,vertCirculate,faceLabelBoundary[1].face.label)){
                            if (PreconditionLegalCollapse(faceLabelBoundary[0],position)){
                            //if (PreconditionLegalCollapse(faceLabelBoundary[0])){
                                var newVertex = faceLabelBoundary[0].Collapse(faceLabelBoundary[0].vert.positionD);
                                if (newVertex != null){
                                    newVertex.positionD = position;
                                }
                                collapsed++;
                                changed = true;
#if HMDebug
                                if (!hmesh.IsValid())
                                {
                                    Debug.Log("Invalid");
                                }
#endif
                                if (debugOneStep) return -1;
                            } else if (PreconditionLegalCollapse(faceLabelBoundary[1],position))
                            {
                                var newVertex = faceLabelBoundary[1].Collapse(faceLabelBoundary[1].vert.positionD);
                                if (newVertex != null){
                                    newVertex.positionD = position;
                                }
                                collapsed++;
                                changed = true;
#if HMDebug
                                if (!hmesh.IsValid())
                                {
                                    Debug.Log("Invalid");
                                }
#endif
                                if (debugOneStep) return -1;
                            }
                        }
                    }

                } else if (faceLabelBoundary.Count == 0)
                {

                    if (IsFacesOnSamePlane(hmesh,vertCirculate)){
                        // search for which to collapse (should not result in flipped vectors)

                        for (int i = 0; i < vertCirculate.Count; i++)
                        {
#if HMDebug
                            if (!hmesh.IsValid())
                            {
                                Debug.LogError("PreInvalid");
                            }
#endif
                            if (PreconditionLegalCollapse(vertCirculate[i],vertCirculate[i].vert.positionD))
                            {
                                var collapseStr = vertCirculate[i].ToString();
                                var position = vertCirculate[i].vert.positionD;
                                var newVertex = vertCirculate[i].Collapse();
                                collapsed++;
                                newVertex.positionD = position;
#if HMDebug
                                if (!hmesh.IsValid())
                                {
                                    Debug.Log("Invalid");
                                    Debug.LogError(str);
                                    Debug.LogError(collapseStr);
                                }
#endif
                                i = int.MaxValue - 1;
                                if (debugOneStep) return -1;
                                break;
                            }
                        }
                    }

                }

            }
        } while (changed && iter < maxIter);

        return collapsed;
    }

    private static bool PreconditionLegalCollapse(Halfedge he, Vector3D pos)
    {
        if (he.IsDestroyed())
        {
            return false;
        }
        if (he.CollapsePrecondition(pos, Halfedge.CollapsePreconditionReason.NormalFlipped) != Halfedge.CollapsePreconditionReason.Ok)
        {
            return false;
        }
        HMesh hmesh = he.hmesh;
        HashSet<int> uniqueLabels = new HashSet<int>();
        List<Vector3D> vertexPositions = new List<Vector3D>();
        int index = 0;
        int indexOfHe = 0;
        var prevVertCirculated = he.prev.vert.Circulate();


        // Check that all normals (of same label) are facing the same way
        var prev = he.prev;
        int thisLabel = he.face.label;
        Vector3D normal = he.face.GetNormal();
        Vector3D otherNormal = Vector3D.zero;

        foreach (var vHe in prev.vert.Circulate())
        {
            uniqueLabels.Add(vHe.face.label);
            if (vHe.face.label != thisLabel)
            {
                otherNormal = vHe.face.GetNormal();
            }
        }
        if (uniqueLabels.Count > 2)
        {
            // currently not simplified
            return false;
        }
        // 2. all face normals of the same label must be equal when having the same label
        foreach (var vHe in prev.vert.Circulate())
        {
            var circulateFaceNormal = vHe.face.GetNormal();
            if (vHe.face.label == thisLabel)
            {
                if (!hmesh.AreNormalsEqual(circulateFaceNormal, normal))
                {
                    return false;
                }
            } else {
                if (!hmesh.AreNormalsEqual(circulateFaceNormal,  otherNormal))
                {
                    return false;
                }
            }
        }

        // if more labels, make sure that the collapse does not change any boundary between labels
        if (uniqueLabels.Count == 2)
        {
            if (he.opp != null && he.face.label == he.opp.face.label)
            {
                return false;
            }

            foreach (var otherHe in prevVertCirculated)
            {
                bool compareToSelf = otherHe.id == he.id;
                if (compareToSelf)
                {
                    continue;
                }
                bool isLabelBoundary = otherHe.opp != null && otherHe.opp.face.label != otherHe.face.label;
                if (isLabelBoundary)
                {
                    var dir1 = he.GetDirection();
                    var dir2 = otherHe.GetDirection();
                    bool oppositeDir = Vector3D.Dot(dir1, dir2) > 0;
                    // if boundary between labels then they must be parallel in same direction
                    if (Vector3D.IsParallelDist(dir1, dir2, hmesh.zeroMagnitudeTresholdSqr) == false || oppositeDir)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}
