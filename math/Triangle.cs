/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


// A triangle but stores the x range in Range
public class Triangle {
	Vector3D[] points;
	Vector2D[] uvs;

	public Triangle(params Vector3D[] ps){
		points = new Vector3D[3];
		uvs = new Vector2D[3];
		for (int i=0;i<ps.Length && i < 3;i++){
			points[i] = ps[i];
		}
	}

	public Triangle(Vector3D[] ps,Vector2D[] uvs){
		points = new Vector3D[3];
		this.uvs = new Vector2D[3];
		for (int i=0;i<ps.Length && i < 3;i++){
			points[i] = ps[i];
			if (uvs != null && i < uvs.Length){
				this.uvs[i] = uvs[i];
			}
		}
	}

	public double GetShortestEdge(){
		double s = double.MaxValue;
		for (int i=0;i<3;i++){
			var mag = (points[i] - points[(i+1)%3]).sqrMagnitude;
			if (mag < s){
				s = mag;
			}
		}
		return System.Math.Sqrt(s);
	}

	public static string ConvertToOBJFile(List<Triangle> triangles, string name = "TriangleMesh", bool removeDuplicateVertices = true){
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> indices = new List<int>();
		foreach (var tri in triangles){
			for (int i=0;i<3;i++){
				vertices.Add(tri.points[i].ToVector3());
				uvs.Add(tri.uvs[i].ToVector2());
				indices.Add(indices.Count);
			}
		}

		/*if (removeDuplicateVertices) {
			List<KDTreeData> list = new List<KDTreeData>();
			for (int i=0;i<vertices.Count;i++){
				list.Add(new KDTreeData(i, vertices[i]));
			}

			List<Vector3> verticesNew = new List<Vector3>();
			List<Vector2> uvsNew = new List<Vector2>();
			List<int> indicesNew = new List<int>();

			KDTreePoint kdTree = new KDTreePoint(list);
			for (int i=0;i<vertices.Count;i++){
				var possibleIntersections = kdTree.Intersect(vertices[i]);	
				bool isLowestIntersection = true;
				if (possibleIntersections.Count==0){
					Debug.LogError("Warning no intersection found");
				}
				KDTreeData lowest = possibleIntersections[0];
				foreach (KDTreeData inters in possibleIntersections){
					if (inters.index < lowest.index){
						lowest = inters;
					}
				}
				if (lowest.index == i){
					// add new vertex
					indicesNew.Add(verticesNew.Count);
					// update reference
					lowest.index = verticesNew.Count;
					verticesNew.Add(vertices[i]);
					uvsNew.Add(uvs[i]);

				} else {
					// add new vertex
					indicesNew.Add(lowest.index);
				}
			}
			vertices = verticesNew;
			uvs = uvsNew;
			indices = indicesNew;
		}
*/ 
		// export 
		System.Text.StringBuilder sb = new StringBuilder("# Custom geometry export (UNITY) OBJ File: '"+name+"'\n");
		sb.Append("o ").Append(name).Append("\n");
		foreach (var v in vertices){
			sb.AppendFormat("v {0} {1} {2}\n", v.x,v.y,v.z);	
		}
		foreach (var u in uvs){
			sb.AppendFormat("vt {0} {1}\n", -u.x,u.y);	
		}
		for (int i=0;i<indices.Count;i=i+3){
			sb.AppendFormat("f {0}/{1} {2}/{3} {4}/{5}\n", 
				indices[0+i]+1, indices[0+i]+1, 
				indices[1+i]+1, indices[1+i]+1, 
				indices[2+i]+1, indices[2+i]+1);	
		}
		return sb.ToString (); // "# Blender v2.76 (sub 0) OBJ File: ''\n# www.blender.org\nmtllib monkey.mtl\no Plane\nv -1.000000 0.000000 1.000000\nv 1.000000 0.000000 1.000000\nv -1.000000 0.000000 -1.000000\nv 1.000000 0.000000 -1.000000\nvn 0.000000 1.000000 0.000000\nusemtl None\ns off\nf 1//1 2//1 4//1 3//1\n"
	}

	public static List<Mesh> ConvertToMeshes(List<Triangle> triangles, string name = "TriangleMesh"){
		List<Mesh> res = new List<Mesh> ();

		Mesh mesh = new Mesh();
		res.Add (mesh);
		mesh.name = name+"_"+res.Count;
		List<int> indices = new List<int>();
		List<Vector3> positions = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		int i = 0;
		foreach (var t in triangles){
			for (int j=0;j<3;j++){
				indices.Add(i);
				positions.Add(t.points[j].ToVector3());
				uvs.Add(t.uvs[j].ToVector2());
				i++;
			}
			if (i >= 65534) {
				
				// save current mesh data
				mesh.vertices = positions.ToArray();
				mesh.uv = uvs.ToArray();
				mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles,0);
				// reset mesh data
				mesh = new Mesh();
				res.Add (mesh);
				mesh.name = name+"_"+res.Count;
				indices = new List<int>();
				positions = new List<Vector3>();
				uvs = new List<Vector2>();

				i = 0;
			}
		}
		mesh.vertices = positions.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles,0);
		return res;
	}

	public static Mesh ConvertToMesh(List<Triangle> triangles, string name = "TriangleMesh"){
		Mesh mesh = new Mesh();
		mesh.name = name;
		int[] indices = new int[triangles.Count*3];
		Vector3[] positions = new Vector3[triangles.Count*3];
		Vector2[] uvs = new Vector2[triangles.Count*3];
		int i = 0;
		foreach (var t in triangles){
			for (int j=0;j<3;j++){
				indices[i] = i;
				positions[i] = t.points[j].ToVector3();
				uvs[i] = t.uvs[j].ToVector2()	;
				i++;
			}
		}
		mesh.vertices = positions;
		mesh.uv = uvs;
		mesh.SetIndices(indices, MeshTopology.Triangles,0);
		return mesh;
	}

	public void FlipNormal(){
		var tmp = points[1];
		points[1] = points[2];
		points[2] = tmp;
		var tmp2 = uvs[1];
		uvs[1] = uvs[2];
		uvs[2] = tmp2;
	}

	void ReprojectUVNoY (List<Triangle> res, bool forceXZLayerProject = true)
	{
		/*double minX = points[0].x, maxX = points[0].x, minZ = points[0].z, maxZ = points[0].z;
		double minXUV = uvs[0].x,maxXUV = uvs[0].x, minZUV = uvs[0].y, maxZUV = uvs[0].y;

		for (int i=1;i<3;i++){
			if (minX > points[i].x){
				minX = points[i].x;
				minXUV = uvs[0].x;
			}
			if (maxX < points[i].x){
				maxX = points[i].x;
				maxXUV = uvs[0].x;
			}

			if (minZ > points[i].z){
				minZ = points[i].z;
				minZUV = uvs[0].y;
			}
			if (maxZ < points[i].z){
				maxZ = points[i].z;
				maxZUV = uvs[0].y;
			}
		}
		double deltaX = maxX - minX;
		double deltaZ = maxZ - minZ;
		double deltaUVX = maxXUV - minXUV;
		double deltaUVZ = maxZUV - minZUV;

		foreach (var t in res) {
			for (int i = 0; i < 3; i++) {
				var pos = t.Vertex (i);

				t.uvs [i] = new Vector2D(
					minXUV + deltaUVX*((pos.x -minX)/(deltaX)),
					minZUV + deltaUVZ*((pos.z -minZ)/(deltaZ))
				);
				if (deltaX == 0) {
					Debug.LogWarning ("deltaX is 0");
					t.uvs[i].x = minXUV;
				}
				if (deltaZ == 0) {
					Debug.LogWarning ("deltaZ is 0");
					t.uvs[i].y = minZUV;
				}
			}
		}
*/
		// http://answers.unity3d.com/questions/383804/calculate-uv-coordinates-of-3d-point-on-plane-of-m.html
		var p1 = points[0];
		var p2 = points[1];
      	var p3 = points[2];
		if (forceXZLayerProject) {
			p1.y = 0;
			p2.y = 0;
			p3.y = 0;
		}
		foreach (var t in res){
			for (int i=0;i<3;i++){
				var pos = t.Vertex(i);
				if (forceXZLayerProject) {
					pos.y = 0;
				}
				// calculate vectors from point f to vertices p1, p2 and p3:
				var f1 = p1-pos;
				var f2 = p2-pos;
				var f3 = p3-pos;
				// calculate the areas and factors (order of parameters doesn't matter):
				double a = Vector3D.Cross(p1-p2, p1-p3).magnitude; // main triangle area a
				double a1 = (Vector3D.Cross(f2, f3).magnitude / a); // p1's triangle area / a
				double a2 = (Vector3D.Cross(f3, f1).magnitude / a); // p2's triangle area / a 
				double a3 = (Vector3D.Cross(f1, f2).magnitude / a); // p3's triangle area / a
				// find the uv corresponding to point f (uv1/uv2/uv3 are associated to p1/p2/p3):

				t.uvs[i] = uvs[0] * a1 + uvs[1] * a2 + uvs[2] * a3;
			}
		}
		/*
		var p1 = points[0];
		var p2 = points[1];
		var p3 = points[2];
		if (forceXZLayerProject) {
			p1.y = 0;
			p2.y = 0;
			p3.y = 0;
		}
		// calculate vectors from point f to vertices p1, p2 and p3:
		foreach (var t in res) {
			for (int i = 0; i < 3; i++) {
				var pos = t.Vertex (i);
				if (forceXZLayerProject) {
					pos.y = 0;
				}
				var f1 = p1 - pos;
				var f2 = p2 - pos;
				var f3 = p3 - pos;
				// calculate the areas (parameters order is essential in this case):
				var va = Vector3D.Cross (p1 - p2, p1 - p3); // main triangle cross product
				var va1 = Vector3D.Cross (f2, f3); // p1's triangle cross product
				var va2 = Vector3D.Cross (f3, f1); // p2's triangle cross product
				var va3 = Vector3D.Cross (f1, f2); // p3's triangle cross product
				double a = va.magnitude; // main triangle area
				// calculate barycentric coordinates with sign:
				double a1 = va1.magnitude / a * Mathf.Sign ((float)Vector3D.Dot (va, va1));
				double a2 = va2.magnitude / a * Mathf.Sign ((float)Vector3D.Dot (va, va2));
				double a3 = va3.magnitude / a * Mathf.Sign ((float)Vector3D.Dot (va, va3));
				// find the uv corresponding to point f (uv1/uv2/uv3 are associated to p1/p2/p3):
				t.uvs [i] = uvs[0] * (float)a1 + uvs[1] * (float)a2 + uvs[2] * (float)a3;
			}
		}*/
	}

	void ReprojectY (List<Triangle> res, Plane3D plane){
		double pos = 0;
		foreach (var t in res){
			for (int i=0;i<3;i++){
				var p = t.points[i];
				p.y = 0;
				RayD r = new RayD(p, Vector3D.up);
				if (plane.Raycast(r,out pos)){
					p.y = (float)pos;
					t.points[i] = p;
				}
			}
		}
	}

	public Plane3D Plane{
		get { 
			return new Plane3D(Normal, points[0]);
		}
	}

	void ReprojectY (List<Triangle> res)
	{
		ReprojectY (res, Plane);
	}

	void ReprojectNewVerticesOntoTriangles (List<Triangle> res,Triangle[] clippingTriangles)
	{
		foreach (var tri in res){
			for (int i=0;i<3;i++){
				var point = tri.points[i];
				if (!ContainsVertex2D(point)){
					foreach (var cp in clippingTriangles){
						var lineSegments = cp.GetLineSegments();
						bool found = false;
						for (int j = 0;j<3;j++){
							var ls = lineSegments[j];
							double t;
							if (ls.MinimumDistance(new Vector2D(point.x, point.z),out t)< 0.00000001f){
								var from = cp.Vertex(j);
								var to = cp.Vertex(j);
								tri.points[i].y = (from + t*(to-from)).y;
								found = true;
								break;
							}
						}
						if (found){
							break;
						}
					}

				} 
			}
		}
	}

	// if the line segment is inside or is intersecting with the triangle then cut
	// otherwise return this as an array
	public Triangle[] Cut(LineSegment2D line){
		if (IsPointInside (line.first) || IsPointInside (line.second) || IsIntersecting (line)) {
			return new Triangle[]{ this };	
		} else {
			return new Triangle[]{ this};	
		}
	}

	Vector2D V2(Vector3D v){
		return new Vector2D(v.x,v.z);
	}



	Vector3D ProjectToTriangle(Vector3D point){
		Plane3D plane = new Plane3D(Normal, points[0]);
		bool isPointInPlane = System.Math.Abs(plane.GetDistanceToPoint(point))<0.0001;
		if (!isPointInPlane) {
			double dist;
			point.y = 0;
			var ray = new RayD(point, Vector3D.up);
			plane.Raycast(ray, out dist);
			point.y = dist;
		} 
		return point;
	}

	public Vector3D Center{
		get {
			return (points[0] + points[1] + points[2])*(1/3.0);
		}
	}

	public Vector3D Normal{
		get {
			return Vector3D.Cross((points[1] - points[0]).normalized, (points[2] - points[1]).normalized).normalized;
		}
	}
	// return unnormalized normal direction
	public Vector3D NormalDir{
		get {
			return Vector3D.Cross((points[1] - points[0]).normalized, (points[2] - points[1]).normalized);
		}
	}

	public Vector3D Vertex(int i){
		return points[i];
	}

	public Vector2D UV(int i){
		return uvs[i];
	}

	public bool IsPointInside(Vector2D point){
		Vector2D[] farFarAwayPoints = new Vector2D[]{
			point+new Vector2(99999,99998),
			point+new Vector2(-99999,89990),
		};
		LineSegment2D[] lineSegments = new LineSegment2D[]{
			new LineSegment2D(point, farFarAwayPoints[0]),
			new LineSegment2D(point, farFarAwayPoints[1]),
		};
		foreach (var ls in lineSegments) {
			int hits = 0;
			for (int i=0;i<3;i++){
				LineSegment2D l1 = new LineSegment2D(Polygon.V2(points[i]), Polygon.V2(points[(i+1)%3]));
				Vector2D res = Vector2D.zero;
				if (ls.LineIntersect(l1,out res)){
					hits++;
				}
			}
			if (hits%2==1){
				return true;
			}
		}
		return false;
	}

	private bool ContainsVertex(Vector3D p){
		for (int i=0;i<points.Length;i++){
			if ((p-points[i]).sqrMagnitude < 0.001f){
				return true;
			}
		}
		return false;
	}

	private bool ContainsVertex2D(Vector3D p){
		Vector2D p2 = new Vector2D(p.x, p.z);
		for (int i=0;i<points.Length;i++){
			if ((p2-new Vector2D(points[i].x, points[i].z)).sqrMagnitude < 0.001f){
				return true;
			}
		}
		return false;
	}

	public double Area(){
		return Vector3D.Cross(points[1]-points[0],points[2]-points[0]).magnitude *0.5;
	}

	public LineSegment2D[] GetLineSegments(){
		LineSegment2D[] res = new LineSegment2D[3];
		for (int i=0;i<3;i++){
			res[i] = new LineSegment2D(new Vector2D(points[i].x,points[i].z), new Vector2D(points[(i+1)%3].x,points[(i+1)%3].z));
		}
		return res;
	}

	public bool IsIntersecting(LineSegment2D ls){
		foreach (var l in GetLineSegments()){
			Vector2D p = new Vector2D ();
			if (ls.LineIntersect(l, out p)) {
				return true;		
			}
		}
		return false;
	}


	public Vector3D Midpoint2D(){
		Vector3D point = Vector3D.zero;
		for (int i=0;i<3;i++){
			point += points[i];
		}
		point.y = 0;
		return point * (1.0f/3);
	}

	public double[] ToArray2D(){
		double[] res = new double[6];
		int idx = 0;
		for (int i=0;i<3;i++){
			for (int j=0;j<3;j++){	
				if (j!=1){
					res[idx] = points[i][j];
					idx++;
				}
			}
		}
		return res;
	}

	public static List<Triangle> ToTriangleList2D(double[] data, int count){
		List<Triangle> res = new List<Triangle>();
		for (int i=0;i<count ;i+=6){
			Triangle t = new Triangle(
				new Vector3D(data[i+0],0,data[i+1]),
				new Vector3D(data[i+2],0,data[i+3]),
				new Vector3D(data[i+4],0,data[i+5])
			);
			res.Add(t);
		}
		return res;
	}
}
