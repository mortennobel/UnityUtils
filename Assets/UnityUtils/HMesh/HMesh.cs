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
using System.Text;
using SimpleJSON;
using UnityEngine.Rendering;

public enum HMeshValidationRules
{
    Standard = 0,
    EdgeCheckForZeroLength = 1,
    CheckForTriangulationError = 2,
    CheckForDegeneratePolygonCorners = 4,
    CheckForInvalidJoins = 8,
    All = 0xFFFF
}

public class HMesh {
	List<Vertex> vertices = new List<Vertex>();
	List<Face> faces = new List<Face>();
	List<Halfedge> halfedges = new List<Halfedge>();
    public int halfedgeMaxId = 0;
    public int faceMaxId = 0;
    public int vertexMaxId = 0;

	// if dotproduct between two normals is larger than this, they are considered equal
	public const double normalDotThreshold = 0.999847695158751; // 1.0 degrees
	
    public double zeroMagnitudeTreshold = 0.002;
	
	// Maximum of 1m elements
	public const int MAX_SIZE = 1000000;  

	public double zeroMagnitudeTresholdSqr
    {
        get { return zeroMagnitudeTreshold * zeroMagnitudeTreshold; }
    }

	public bool AreNormalsEqual(Vector3D normal1,Vector3D normal2)
	{
		return Vector3D.Dot(normal1, normal2) > normalDotThreshold;

	}

	public HashSet<Vertex> GetVertices(){
		return new HashSet<Vertex>(vertices);
	}
	
	public HashSet<Face> GetFaces(){
		return new HashSet<Face>(faces);
	}

	public HashSet<Halfedge> GetHalfedges(){
		return new HashSet<Halfedge>(halfedges);
	}

    public List<Halfedge> GetBoundaryHalfedges(){
        List<Halfedge> res = new List<Halfedge>();
        foreach (Halfedge he in halfedges)
        {
            if (he.IsBoundary())
            {
                res.Add(he);
            }
        }
        return res;
    }

    public List<Vertex> GetVerticesRaw(){
		return vertices;
	}

	public List<Face> GetFacesRaw(){
		return faces;
	}

	public List<Halfedge> GetHalfedgesRaw(){
		return halfedges;
	}

	public HMesh(){
	}

	/// <summary>
	/// Set label to all sharp edges to 1 and rest to 0
	/// </summary>
	public void MarkSharpEdges(double angle = 30.0)
	{
		foreach (var he in halfedges)
		{
			if (he.IsBoundary())
			{
				he.label = 0;
				continue;
			}
			if (he.opp.id < he.id) continue;

			double edgeAngle = Vector3D.Angle(he.face.GetNormal(), he.opp.face.GetNormal());
			int label = edgeAngle <= angle ? 0 : 1;
			he.label = label;
			he.opp.label = label;
		}
	}

	void SetFaceCluster(Face face, int label, Dictionary<int, int> faceCluster)
	{
		faceCluster[face.id] = label;

		foreach (var edge in face.Circulate())
		{
			bool isSharpEdge = edge.label == 1;
			if (edge.opp == null || isSharpEdge) continue;

			if (faceCluster.ContainsKey(edge.opp.face.id)) continue;

			SetFaceCluster(edge.opp.face, label, faceCluster);
		}
	}

	// returns face.id->clusterid
	private Dictionary<int, int> SeparateMeshByGeometry(out int clusterCount)
	{
		Dictionary<int, int> faceCluster = new Dictionary<int, int>();

		int clusterIndex = 0;
		for (int i = 0; i < faces.Count; i++)
		{
			if (faceCluster.ContainsKey(faces[i].id)) continue;

			SetFaceCluster(faces[i], clusterIndex, faceCluster);
			clusterIndex++;
		}
		clusterCount = clusterIndex;
		return faceCluster;
	}

	public int SplitNonManifoldVertices()
	{
		int count = 0;
		
        HashSet<int> heAroundVertex = new HashSet<int>();
        foreach (var vert in GetVertices())
        {
	        heAroundVertex.Clear();
	        var allIngoing = vert.CirculateAllIngoing();
	        
	        foreach (var inGoingHe in allIngoing)
	        {
		        bool isFirst = heAroundVertex.Count == 0;
				
		        if (heAroundVertex.Contains(inGoingHe.id))
		        {
			        continue;
		        }
		        Vertex v = isFirst ? inGoingHe.vert : CreateVertex(inGoingHe.vert.positionD);
		        if (!isFirst)
		        {
			        count++;
		        }
		        var iter = inGoingHe;
		        bool circular = false;
		        while (iter != null)
		        {
			        iter.vert = v;
			        circular = heAroundVertex.Contains(iter.id);
			        if (circular)
			        {
				        break;
			        }
			        heAroundVertex.Add(iter.id);
			        iter = iter.next.opp;
		        }
		        if (circular)
		        {
			        continue;
		        }
		        iter = inGoingHe;
		        while (iter != null)
		        {
			        iter.vert = v;
			        heAroundVertex.Add(iter.id);
			        iter = iter.opp;
			        if (iter != null)
			        {
				        iter = iter.prev;
			        }
		        }
	        }
        }
		return count;
	}

    public HMesh Copy(){
		List<Vertex> fromVert = new List<Vertex>();
		List<Face> fromFaces = new List<Face>();
		List<Halfedge> fromHalfedge = new List<Halfedge>();

		List<Vertex> toVert = new List<Vertex>();
		List<Face> toFace = new List<Face>();
		List<Halfedge> toHalfedge = new List<Halfedge>();

		HMesh newMesh = new HMesh();

		int index = 0;
		foreach (var v in vertices){
			v.label = index;
			fromVert.Add(v);
			var nv = newMesh.CreateVertex(v.positionD);
			nv.uv1 = v.uv1;
			nv.uv2 = v.uv2;
			toVert.Add(nv);
			index++;
		}
		index = 0;
		foreach (var f in faces){
			f.label = index;
			fromFaces.Add(f);
			var nf = newMesh.CreateFace();
			nf.label = f.label;
			toFace.Add(nf);
			index++;
		}
		index = 0;
		foreach (var e in halfedges){
			e.label = index;
			fromHalfedge.Add(e);
			toHalfedge.Add(newMesh.CreateHalfedge());
			index++;
		}

		foreach (var from in faces){
			Face to = toFace[from.label];
			if (from.halfedge != null){to.halfedge = toHalfedge[from.halfedge.label];}
		}

		foreach (var from in halfedges){
			Halfedge to = toHalfedge[from.label];
			if (from.face != null){to.face = toFace[from.face.label];}
			if (from.opp  != null){to.opp  = toHalfedge[from.opp.label];}
			if (from.next != null){to.next = toHalfedge[from.next.label];}
			if (from.prev != null){to.prev = toHalfedge[from.prev.label];}
			if (from.vert != null){to.vert = toVert[from.vert.label];}

		}
		return newMesh;
	}

    public Face CreateTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return CreateTriangle(new Vector3D(p1), new Vector3D(p2), new Vector3D(p3));
    }

    public Face CreateTriangle(Vector3D p1, Vector3D p2, Vector3D p3){
		var face = CreateFace();
		var edges = new[]{
			CreateHalfedge(),
			CreateHalfedge(),
			CreateHalfedge()
		};
		var verts = new[]{
			CreateVertex(p1),
			CreateVertex(p2),
			CreateVertex(p3),
		};
		for (int i=0;i<3;i++){
			edges[i].Link(face);
			edges[i].next = edges[(i+1)%3];
			edges[(i+1)%3].prev = edges[i];

			edges[i].vert = verts[i];
		}
		return face;
	}

	public static HMesh CreateTestMesh() {
		HMesh mesh = new HMesh();
		GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		var meshFilter = plane.GetComponent<MeshFilter>();
		mesh.Build(meshFilter.sharedMesh,Matrix4x4.identity);
		GameObject.DestroyImmediate(plane);
		return mesh;
	}

	public static HMesh CreateTestMeshTriangle(bool upwardsNormal = false) {
		HMesh mesh = new HMesh();
	    if (upwardsNormal)
	    {
	        mesh.CreateTriangle(new Vector3(0,0,0),new Vector3(0,0,1),new Vector3(1,0,0));
	    } else {
	        mesh.CreateTriangle(new Vector3(0,0,0),new Vector3(1,0,0),new Vector3(0,0,1));
	    }
		return mesh;
	}

	public static HMesh CreateTestMeshQuad() {
		HMesh mesh = new HMesh();
		var face = mesh.CreateFace();
		var edges = new[]{
			mesh.CreateHalfedge(),
			mesh.CreateHalfedge(),
			mesh.CreateHalfedge(),
			mesh.CreateHalfedge(),
		};
		var verts = new[]{
			mesh.CreateVertex(new Vector3(0,0,0)),
			mesh.CreateVertex(new Vector3(1,0,0)),
			mesh.CreateVertex(new Vector3(1,0,1)),
			mesh.CreateVertex(new Vector3(0,0,1)),
		};
		for (int i=0;i<4;i++){
			edges[i].Link(face);
			edges[i].next = edges[(i+1)%4];
			edges[(i+1)%4].prev = edges[i];

			edges[i].vert = verts[i];
		}
		return mesh;
	}

	ulong EdgeKey(int vertex1, int vertex2){
		ulong vertex1s = (ulong)vertex1;
		ulong vertex2s = (ulong)vertex2;
		return (vertex1s + (vertex2s<<32));
	}

	public void Build(Mesh mesh){
		Build(mesh, Matrix4x4.identity);
	}

    public void BuildFromObj(string objFileContent, bool splitNonManifoldVertices = true)
    {
        StringReader stringReader = new StringReader(objFileContent);
        string line;
        List<Vertex> vertices = new List<Vertex>();
        Dictionary<IntPair, Halfedge> heLookup = new Dictionary<IntPair, Halfedge> ();
        int label = -1;
        while ((line = stringReader.ReadLine()) != null)
        {
            var tokens = line.Trim().Split(' ');
            if (tokens.Length == 0)
            {
                continue;
            }
            if (tokens[0] == "o")
            {
                label++;
            }
            if (tokens[0] == "v")
            {
                var v = CreateVertex(new Vector3D(double.Parse(tokens[1]), double.Parse(tokens[2]),
                    double.Parse(tokens[3])));
                vertices.Add(v);
            }
            if (tokens[0] == "f")
            {
                List<int> vertexIndices = new List<int>();
                for (var i = 1; i < tokens.Length; i++)
                {
                    var vertexIdx = int.Parse(tokens[i].Split('/')[0]) - 1;
                    vertexIndices.Add(vertexIdx);
                }
                var face = CreateFace();
                face.label = Mathf.Max(0, label);
                for (var i = 0; i < vertexIndices.Count; i++)
                {
                    var halfEdge = CreateHalfedge();
                    halfEdge.Link(face);
                    heLookup[new IntPair(vertexIndices[i], vertexIndices[(i + 1) % vertexIndices.Count])] = halfEdge;
                    Halfedge opp = null;
                    if (heLookup.TryGetValue(
                        new IntPair(vertexIndices[(i + 1) % vertexIndices.Count], vertexIndices[i]), out opp))
                    {
                        // if (invalid) 2-gon face, then don't glue
                        // this would connect opp to itself
                        if (vertexIndices.Count > 2){
                            halfEdge.Glue(opp);
                        }
                    }
                    halfEdge.vert = vertices[vertexIndices[(i + 1) % vertexIndices.Count]];
                }
                for (var i = 0; i < vertexIndices.Count; i++)
                {
                    var thisHe = heLookup[new IntPair(vertexIndices[i], vertexIndices[(i + 1) % vertexIndices.Count])];
                    var nextHe = heLookup[
                        new IntPair(vertexIndices[(i + 1) % vertexIndices.Count],
                            vertexIndices[(i + 2) % vertexIndices.Count])];
                    thisHe.Link(nextHe);
                }
            }
        }
        if (splitNonManifoldVertices){
            SplitNonManifoldVertices();
        }
    }

    public Bounds ComputeBounds(){
		Bounds bounds = new Bounds (vertices [0].position, Vector3.zero);
		foreach (var vert in vertices) {
			bounds.Encapsulate (vert.position);
		}
		return bounds;
	}

	public void Build(Mesh mesh, Matrix4x4 transform, int submesh = 0){
		if (mesh.GetTopology(0) != MeshTopology.Triangles){
			Debug.LogError("Only triangles supported.");
		}

		Dictionary<ulong, Halfedge> halfedgeByVertexID = new Dictionary<ulong, Halfedge>();

		// Create a list of (HMesh) Vertices
	    var meshVertices = mesh.vertices;
	    var meshUv = mesh.uv;
	    var meshUv2 = mesh.uv2;
	    List<Vertex> vertexList = new List<Vertex>(meshVertices.Length);
	    bool hasUv1 = meshUv != null && meshUv.Length == meshVertices.Length;
	    bool hasUv2 = meshUv2 != null && meshUv2.Length == meshVertices.Length;
		for (int i=0;i<meshVertices.Length;i++){
		    var newV = CreateVertex();
		    newV.position = transform.MultiplyPoint(meshVertices[i]);
			if (hasUv1){
				newV.uv1 = meshUv[i];
			}
			if (hasUv2){
				newV.uv2 = meshUv2[i];
			}
			vertexList.Add(newV);
		}


	    // create faces and half edges
	    var meshTriangles = mesh.GetTriangles(submesh);
	    for (int i=0;i<meshTriangles.Length;i+=3){
	        int[] idx = new int[]{
				meshTriangles[i],
				meshTriangles[i+1],
				meshTriangles[i+2]};
			Halfedge[] edges = new Halfedge[3];
			Face face = CreateFace();
			for (int j=0;j<3;j++){
				Halfedge edge = CreateHalfedge();
				edge.Link(face);
				edges[j] = edge;
			}
			for (int j=0;j<3;j++){
				int from = idx[j];
				int to = idx[(j+1)%3];
				edges[j].Link(edges[(j+1)%3]);
				edges[j].vert = vertexList[to];
			    edges[j].vert.label++;
				ulong edgeId = EdgeKey(from,to);
			    if (halfedgeByVertexID.ContainsKey(edgeId))
			    {
			        var oldEdge = halfedgeByVertexID[edgeId];

			        Debug.LogError("Edge old edge from "+oldEdge.vert.position+" to "+oldEdge.prev.vert.position);
			        Debug.LogError("Edge already exists from "+vertexList[to].position+" to "+vertexList[from].position);
			    }
			    halfedgeByVertexID.Add(edgeId, edges[j]);
			}
		}
	    for (int i = vertexList.Count - 1; i >= 0; i--)
	    {
	        if (vertexList[i].label == 0)
	        {
	            Destroy(vertexList[i]);
	        }
	    }
	    int glued = 0;
		// glue all opposite half edges
		for (int i=0;i<meshTriangles.Length;i+=3){
		    int[] idx = {
			    meshTriangles[i],
			    meshTriangles[i+1],
			    meshTriangles[i+2]};
			for (int j=0;j<3;j++){
				int from = idx[j];
				int to = idx[(j+1)%3];
				ulong key = EdgeKey(from,to);
				ulong oppKey = EdgeKey(to,from);
				Halfedge edge = halfedgeByVertexID[key];
				bool isOppUnassigned = edge.opp == null;
				if (isOppUnassigned && key < oppKey && halfedgeByVertexID.ContainsKey(oppKey)){
					Halfedge oppEdge = halfedgeByVertexID[oppKey];
					edge.Glue(oppEdge);
					glued++;
				}
			}
		}

		SplitNonManifoldVertices();

	}

    /// <summary>
    /// Export the HMesh as a number of meshes, split into a number of subregions.
    /// If the parameter used material is provided, the mesh only contains submeshes with geometry and the submesh index
    /// is added to the used material.
    /// Is a mesh does not contain any vertices it is skipped.
    /// </summary>
    public List<Mesh> ExportSplit(Vector3i axisSplit, List<List<int>> usedMaterials = null, double sharpEdgeAngle = 360, IndexFormat indexFormat = IndexFormat.UInt16)
    {
        var bounds = ComputeBoundsD();


        bounds.extents += Vector3D.one * 0.000001; // add delta size to ensure no vertex is on bounds

        var resList = new List<Mesh>();

        // Enumerate vertices
        for (int i=0;i<vertices.Count;i++)
        {
            vertices[i].label = i;
        }

        var maxFaceLabel = 0;
        foreach (var face in faces)
        {
            maxFaceLabel = Mathf.Max(maxFaceLabel, face.label);
        }

	    int clusterCount = 0;
	    MarkSharpEdges(sharpEdgeAngle);
	    var faceClusters = SeparateMeshByGeometry(out clusterCount);

	    Debug.Log("Face clusters: "+clusterCount);
	    
		var remap = new int[vertices.Count];
        for (var i = 0; i < axisSplit[0]; i++)
        {
            double minX = bounds.min.x + i * (bounds.size.x / axisSplit[0]);
            double maxX = bounds.min.x + (i+1) * (bounds.size.x / axisSplit[0]);
            for (var j = 0; j < axisSplit[1]; j++)
            {
                double minY = bounds.min.y + j * (bounds.size.y / axisSplit[1]);
                double maxY = bounds.min.y + (j+1) * (bounds.size.y / axisSplit[1]);
                for (var k = 0; k < axisSplit[2]; k++)
                {
                    double minZ = bounds.min.z + k * (bounds.size.z / axisSplit[2]);
                    double maxZ = bounds.min.z + (k+1) * (bounds.size.z / axisSplit[2]);

                    // DebugExt.DrawBox(new Vector3D(minX, minY, minZ).ToVector3(), new Vector3D(maxX, maxY, maxZ).ToVector3(),Color.white, 10);

                    var min = new Vector3D(minX, minY, minZ);
                    var max = new Vector3D(maxX, maxY, maxZ);

                    var res = new Mesh();
	                res.indexFormat = indexFormat;
                    res.name = "HMesh_" + i + "," + j + "," + k;
                    var vertexArray = new List<Vector3>();
                    var normalArray = new List<Vector3>();
                    var uv1 = new List<Vector2>();
                    var uv2 = new List<Vector2>();

                    res.subMeshCount = maxFaceLabel + 1;
	                List<Face> facesInRegion = new List<Face>();

	                foreach (var face in faces)
	                {
		                var center = face.GetCenter();
		                if (Vector3D.AllLessThan(min, center) && Vector3D.AllLessEqualThan(center, max))
		                {
			                facesInRegion.Add(face);
		                }
	                }
	                var triangles = new List<List<int>>();
	                for (int ii = 0; ii <= maxFaceLabel; ii++)
	                {
		                triangles.Add(new List<int>());
	                }

	                for (int clusterIndex=0;clusterIndex<clusterCount;clusterIndex++){
						// clear remap
						for (int x = 0; x < remap.Length; x++)
						{
							remap[x] = -1;
						}

		                for (var faceLabel = 0; faceLabel <= maxFaceLabel; faceLabel++)
		                {

			                foreach (var face in facesInRegion)
			                {
				                if (faceClusters[face.id] != clusterIndex) continue;
				                if (face.label == faceLabel)
				                {   
					                if (face.NoEdges() != 3)
					                {
						                Debug.LogError("Only triangles supported. Was " + face.NoEdges() + "-gon");
						                continue;
					                }
					                var he = face.halfedge;
					                var first = true;
					                while (he != face.halfedge || first)
					                {
						                var indexOfVertex = he.vert.label;
						                var indexOfVertexRemapped = remap[indexOfVertex];
						                if (indexOfVertexRemapped == -1)
						                {
							                indexOfVertexRemapped = vertexArray.Count;
							                remap[indexOfVertex] = indexOfVertexRemapped;
							                vertexArray.Add(vertices[indexOfVertex].position);
							                uv1.Add(vertices[indexOfVertex].uv1);
							                uv2.Add(vertices[indexOfVertex].uv2);
							                
							                // compute normal
							                Vector3D n = Vector3D.zero;
							                int count = 0;
							                foreach (var vertHe in he.vert.CirculateAllIngoing())
							                {
								                if (faceClusters[vertHe.face.id] == clusterIndex)
								                {
									                double angle = Vector3D.Angle(-vertHe.GetDirection(), vertHe.next.GetDirection());
									                n += angle * vertHe.face.GetNormal();
									                count++;
								                }
							                }
							                if (n.sqrMagnitude <= 0 || double.IsNaN(Vector3D.Dot(n,n)) )
							                {
								                
								                Debug.LogWarning("Cannot compute normal n is "+n.ToString("R")+" edges of vertex "+he.vert.Circulate().Count+" same cluster "+count+" "+ he.face.ToString());
								                foreach (var vertHe in he.vert.CirculateAllIngoing())
								                {
									                if (faceClusters[vertHe.face.id] == clusterIndex)
									                {
										                Debug.Log(vertHe.face.ToString());
									                }
								                }
								                n = new Vector3D(0,1,0);
							                } else {
							                	n.Normalize();
							                }
							                normalArray.Add(n.ToVector3());
						                }
						                triangles[faceLabel].Add(indexOfVertexRemapped);
						                he = he.next;
						                first = false;
					                }
				                }
			                }
		                }
	                }
                    if (vertexArray.Count == 0)
                    {
                        // empty mesh - skip
                        continue;
                    }
                    resList.Add(res);
	                if (vertexArray.Count > 65000)
	                {
		                Debug.LogWarning("Vertex count was "+vertexArray.Count);
	                }
	                res.vertices = vertexArray.ToArray();
                    res.uv = uv1.ToArray();
                    res.uv2 = uv2.ToArray();
                    res.normals = normalArray.ToArray();

                    // add mesh indices
                    // if usedMaterials exists then filter out any empty submesh
                    List<int> materialIndices = null;
                    if (usedMaterials != null)
                    {
                        materialIndices = new List<int>();
                        usedMaterials.Add(materialIndices);
                    }
                    for (int ii=0;ii<triangles.Count;ii++)
                    {
                        if (materialIndices != null)
                        {
                            if (triangles[ii].Count > 0)
                            {
                                res.SetIndices(triangles[ii].ToArray(),MeshTopology.Triangles, materialIndices.Count, true);
                                materialIndices.Add(ii);
                            }
                        }
                        else
                        {
                            res.SetIndices(triangles[ii].ToArray(), MeshTopology.Triangles, ii, true);
                        }
                    }
                    res.RecalculateBounds();
                }
            }
        }
        return resList;
    }

    public Mesh Export(bool faceIndexAsSubmeshes = false, IndexFormat indexFormat = IndexFormat.UInt16){
		Mesh res = new Mesh();
	    res.indexFormat = indexFormat;
		Vector3[] vertexArray = new Vector3[vertices.Count];
		Vector2[] uv1 = new Vector2[vertices.Count];
		Vector2[] uv2 = new Vector2[vertices.Count];
		for (int i=0;i<vertexArray.Length;i++)
		{
		    vertices[i].label = i;
		    vertexArray[i] = vertices[i].position;
			uv1[i] = vertices[i].uv1;
			uv2[i] = vertices[i].uv2;
		}
		res.vertices = vertexArray;
		res.uv = uv1;
		res.uv2 = uv2;
	    if (faceIndexAsSubmeshes)
	    {
	        int maxFaceLabel = 0;
	        foreach (var face in faces)
	        {
	            maxFaceLabel = Mathf.Max(maxFaceLabel, face.label);
	        }
	        res.subMeshCount = maxFaceLabel + 1;
	        for (int i = 0; i <= maxFaceLabel; i++)
	        {
	            List<int> triangles = new List<int>();
	            foreach (var face in faces)
	            {
	                if (face.label == i)
	                {
	                    if (face.NoEdges() != 3)
	                    {
	                        Debug.LogError("Only triangles supported - was "+face.ToString());
	                        continue;
	                    }
	                    var he = face.halfedge;
	                    bool first = true;
	                    while (he != face.halfedge || first)
	                    {
	                        int indexOfVertex = he.vert.label;
	                        triangles.Add(indexOfVertex);
	                        he = he.next;
	                        first = false;
	                    }
	                }
	            }
	            res.SetIndices(triangles.ToArray(),MeshTopology.Triangles, i,true);
	        }
	    } else {
            List<int> triangles = new List<int>();
            foreach (var face in faces){
                if (face.NoEdges() != 3){
                    Debug.LogError("Only triangles supported");
                    continue;
                }
                var he = face.halfedge;
                bool first = true;
                while (he != face.halfedge || first){
                    int indexOfVertex = he.vert.label;
                    triangles.Add(indexOfVertex);
                    he = he.next;
                    first = false;
                }
            }

            res.SetTriangles(triangles.ToArray(),0);
	    }
	    res.RecalculateBounds();
	    res.RecalculateNormals();
	    return res;
	}

	public void Clear(){
		foreach (var v in new List<Vertex>(vertices)){
			Destroy(v);
		}
		foreach (var f in new List<Face>(faces)){
			Destroy(f);
		}
		foreach (var h in new List<Halfedge>(halfedges)){
			Destroy(h);
		}
	}

	public Vertex CreateVertex(){
		var res = new Vertex(this);
		vertices.Add(res);
		return res;
	}

	public Vertex CreateVertex(Vector3D p){
		var res = new Vertex(this);
		res.positionD = p;
		vertices.Add(res);
		return res;
	}

	public Vertex CreateVertex(Vector3 p){
		var res = new Vertex(this);
		res.position = p;
		vertices.Add(res);
		return res;
	}

	public Face CreateFace(){
		var res = new Face(this);
		faces.Add(res);
		return res;
	}

	public Halfedge CreateHalfedge(){
		var res = new Halfedge(this);
		halfedges.Add(res);
		return res;
	}

	public bool Destroy(Face face){
	    Debug.Assert(face != null);
	    Debug.Assert(face.hmesh == this);
		Debug.Assert(!face.IsDestroyed(), "Face already destroyed");
		bool res = faces.Remove(face);
	    if (res)
	    {
	        face.halfedge = null;
	        face.SetDestroyed();
	    }
	    else
	    {
	        Debug.Log("Already destroyed");
	    }
	    return res;
	}

	public bool Destroy(Halfedge halfedge){
	    Debug.Assert(halfedge != null);
	    Debug.Assert(halfedge.hmesh == this);
		Debug.Assert(!halfedge.IsDestroyed(), "Halfedge already destroyed");
	    bool res = halfedges.Remove(halfedge);
	    if (res)
	    {
	        halfedge.face = null;
	        halfedge.next = null;
	        halfedge.opp = null;
	        halfedge.prev = null;
	        halfedge.vert = null;
	        halfedge.SetDestroyed();
	    }
	    else
	    {
	        Debug.Log("Already destroyed");
	    }
	    return res;
	}

	public bool Destroy(Vertex vertex){
		Debug.Assert(vertex != null);
		Debug.Assert(!vertex.IsDestroyed(), "Vertex already destroyed");
	    Debug.Assert(vertex.hmesh == this);
	    bool res = vertices.Remove(vertex);
	    if (res)
	    {
		    var he = vertex.halfedge; 
		    if (he != null && he.prev != null)
		    {
			    he.prev.vert = null;
		    }
	        vertex.position = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
	        vertex.SetDestroyed();
	    }
	    else
	    {
	        Debug.Log("Already destroyed "+vertex.id);
	    }
	    return res;
	}

    public bool IsValid(HMeshValidationRules validationRules = HMeshValidationRules.Standard){
		bool valid = true;
		foreach (var v in vertices){
			valid &= v.IsValid(validationRules);
		}
		foreach (var he in halfedges){
			valid &= he.IsValid(validationRules);
		}
		foreach (var f in faces){
			valid &= f.IsValid(validationRules);
		}

		return valid;
	}

    public BoundsD ComputeBoundsD()
    {
        if (vertices.Count == 0)
        {
            return new BoundsD(Vector3D.zero,Vector3D.zero);
        }
        BoundsD res = new BoundsD(vertices[0].positionD,Vector3D.zero );
        foreach (var v in vertices)
        {
            res.Encapsulate(v.positionD);
        }
        return res;
    }

    public void Triangulate(bool step = false)
    {

        foreach (var face in GetFaces())
        {
	        if (face.IsDestroyed())
	        {
		        continue;
	        }
	        var iter = face.Circulate();
            if (iter.Count > 3)
            {

	            // collapse edges below minimu length
	            int collapsed = 0;
	            foreach (var he in iter)
	            {
		            if (he.GetDirection().sqrMagnitude < this.zeroMagnitudeTresholdSqr)
		            {
			            if (he.Collapse() != null){
							collapsed++;
							if (iter.Count - collapsed <= 3)
							{
								break;
							}
			            }
		            }
	            }
	            
	            var res = face.Triangulate(step);
                if (step)
                {
                    return;
                }
	            if (res == null)
	            {
		            Debug.LogWarning("problem triangulating face "+face);
	            }
            }
        }
    }

    public static HMesh CreateTestMeshNGon(int n)
    {
        HMesh mesh = new HMesh();
        var face = mesh.CreateFace();
        List<Halfedge> edges = new List<Halfedge>();
        List<Vertex> verts = new List<Vertex> ();
        for (var i = 0; i < n; i++)
        {
            edges.Add(mesh.CreateHalfedge());
            verts.Add(mesh.CreateVertex(new Vector3(Mathf.Sin((i*Mathf.PI*2)/n),0,Mathf.Cos((i*Mathf.PI*2)/n))));
        }

        for (var i=0;i<n;i++){
            edges[i].Link(face);
            edges[i].next = edges[(i+1)%n];
            edges[(i+1)%n].prev = edges[i];

            edges[i].vert = verts[i];
        }
        return mesh;
    }

    public override string ToString()
    {
        return "Hmesh verts " + vertices.Count + " edges " + halfedges.Count + " faces " + faces.Count;
    }

    public String CreateDebugData()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Vertices: "+vertices.Count+"\n");
        foreach (var vert in vertices)
        {
            sb.Append(" " + vert.id + " p (" + vert.position + ") he "+DebugHalfedge(vert.halfedge)+" ingoing "+DebugIngoing(vert)+"\n");
        }
        sb.Append("Halfedges: "+halfedges.Count+"\n");
        foreach (var he in halfedges)
        {
            sb.Append(" "+he.id+" next "+DebugHalfedge(he.next)+" prev "+DebugHalfedge(he.prev)+" opp "+DebugHalfedge(he.opp)+" vert "+DebugVertex(he.vert)+" face "+DebugFace(he.face)+"\n");
        }
        sb.Append("Faces: "+faces.Count+"\n");
        foreach (var fe in faces)
        {

            sb.Append(" " + fe.id + " he "+DebugHalfedge(fe.halfedge)+"\n");
        }
        return sb.ToString();
    }

	private string DebugIngoing(Vertex vert)
	{
		StringBuilder sb = new StringBuilder("{");
		foreach (var he in vert.CirculateAllIngoing())
		{
			sb.Append(he.id+" ");
		}
		sb.Append("}");
		return sb.ToString();
	}

	private string DebugVertex(Vertex v)
    {
        if (v == null)
        {
            return "null (invalid)";
        } else if (v.IsDestroyed())
        {
            return "not null "+v.id+" (destroyed)";
        }
        else
        {
            return v.id.ToString();
        }
    }
    private string DebugFace(Face f)
    {
        if (f == null)
        {
            return "null (invalid)";
        } else if (f.IsDestroyed())
        {
            return "not null "+f.id+" (destroyed)";
        }
        else
        {
            return f.id.ToString();
        }
    }

    private string DebugHalfedge(Halfedge he)
    {
        if (he == null)
        {
            return "null";
        } else if (he.IsDestroyed())
        {
            return "not null "+he.id+" (destroyed)";
        }
        else
        {
            return he.id.ToString();
        }
    }

    public string ExportObj()
    {
        StringBuilder sb = new StringBuilder();
        for (int i=0;i<vertices.Count;i++)
        {

            sb.Append("v " + vertices[i].positionD.x.ToString("R") + " " + vertices[i].positionD.y.ToString("R") + " " + vertices[i].positionD.z.ToString("R")+"\n");
            vertices[i].label = i;
        }

        int maxFaceLabel = 0;
        foreach (var face in faces)
        {
            maxFaceLabel = Mathf.Max(maxFaceLabel, face.label);
        }
        for (int i = 0; i <= maxFaceLabel; i++)
        {

            sb.Append("o label" + i+"\n");
            foreach (var f in faces)
            {
                if (i == f.label){
                    sb.Append("f");
                    Debug.Assert(f.IsDestroyed()==false);
                    foreach (var he in f.Circulate())
                    {
                        Debug.Assert(he.IsDestroyed()==false);
                        Debug.Assert(he.vert != null);
                        Debug.Assert(he.vert.IsDestroyed()==false);
                        Debug.Assert(he.vert.label < vertices.Count);
                        sb.Append(" " + (he.vert.label+1));
                    }
                    sb.Append("\n");
                }
            }
        }

        return sb.ToString();
    }

    public string DebugFaceNeighbourhood(Face face)
    {

        HashSet<Face> faces = new HashSet<Face>();
        HashSet<Halfedge> halfedges = new HashSet<Halfedge>();
        HashSet<Vertex> vertices = new HashSet<Vertex>();

        // add neighbour faces
        foreach (var he in face.Circulate())
        {
            foreach (var heCirculate in he.CirculateVertex())
            {
                faces.Add(heCirculate.face);
            }
        }
        foreach (var f in faces)
        {
            foreach (var he in f.Circulate())
            {
                halfedges.Add(he);
                vertices.Add(he.vert);
            }
        }
        JSONClass o = new JSONClass();
        o.Add("face", new JSONData(face.id));
        var faceArray = new JSONArray();
        foreach (var f in faces)
        {
            var faceObj = new JSONClass();
            faceObj.Add("id",new JSONData(f.id));
            faceObj.Add("label",new JSONData(f.label));
            faceObj.Add("he",new JSONData(f.halfedge.id));
            faceArray.Add(faceObj);
        }
        o.Add("faces",faceArray);
        var halfedgeArray = new JSONArray();
        foreach (var he in halfedges)
        {
            var heObj = new JSONClass();
            heObj.Add("id",new JSONData(he.id));
            heObj.Add("opp",new JSONData(he.opp!=null?he.opp.id:-1));
            heObj.Add("next",new JSONData(he.next.id));
            heObj.Add("label",new JSONData(he.label));
            heObj.Add("face",new JSONData(he.face.id));
            heObj.Add("vert",new JSONData(he.vert.id));
            halfedgeArray.Add(heObj);
        }
        o.Add("halfedges", halfedgeArray);

        var vertexArray = new JSONArray();
        foreach (var vert in vertices)
        {
            var vertObj = new JSONClass();
            vertObj.Add("id",new JSONData(vert.id));
            vertObj.Add("he",new JSONData(vert.halfedge.id));
            vertObj.Add("label",new JSONData(vert.label));
            var vertexPosition = new JSONArray();
            vertexPosition.Add(new JSONData(vert.positionD.x));
            vertexPosition.Add(new JSONData(vert.positionD.y));
            vertexPosition.Add(new JSONData(vert.positionD.z));
            vertObj.Add("position",vertexPosition);
            vertexArray.Add(vertObj);
        }
        o.Add("vertices", vertexArray);

        return o.ToString();
    }
}
