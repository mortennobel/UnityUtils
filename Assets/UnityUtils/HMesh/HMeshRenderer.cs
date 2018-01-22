/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections.Generic;

public enum HMeshRendererType {
    Full,
    Normals,
    BoundaryEdges,
	Vertices
}

public enum OptimizationStrategy
{
    MinimizeDihedralAngle,
    MaximizeMinAngle,
    OptimizeValency
}

public class HMeshRenderer : MonoBehaviour {

	public HMesh hmesh;

	public Mesh mesh;

    public OptimizationStrategy optStategy;
    public bool OptimizeFaceLabelContrain = true;
    public bool OptimizeFaceNormalContrain = true;
    public float OptimizeMinMaxAngleThreshold = 0.1f;
    [TextArea(3,10)]
    public string objfile;

    public Color[] colors = new Color[]
	{
	    Color.red,
	    Color.green,
	    Color.blue,
	    Color.grey,
	    Color.cyan
	};

	public HMeshRendererType renderType = HMeshRendererType.Full;

    public MeshFilter meshFilter;

	Mesh GetMesh(){
		if (mesh == null){
			mesh = new Mesh();
		    mesh.name = "HMesh-to-mesh";
		}
		return mesh;
	}

	void OnDrawGizmos() {
	    var mesh = GetMesh();

	    for (int i = 0; i < Mathf.Min(colors.Length,mesh.subMeshCount); i++)
	    {
	        Gizmos.color = colors[i];
	        Gizmos.DrawWireMesh(mesh, i, transform.position, transform.rotation, transform.localScale);
	    }

	}

	// Update is called once per frame
	public Mesh UpdateMesh () {
		Mesh mesh = GetMesh();

		Debug.Log("UpdateMesh");

		List<Vector3> lines = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<List<int>> indices = new List<List<int>>();
		HMesh hmesh = this.hmesh;
	    int vertexId = 0;
		foreach (var face in hmesh.GetFacesRaw())
		{
		    int materialIndex = face.label % colors.Length;
		    while (indices.Count <= materialIndex)
		    {
		        indices.Add(new List<int>());
		    }
            if (renderType == HMeshRendererType.Normals){
			    	lines.Add(face.GetCenter().ToVector3());
                    lines.Add((face.GetCenter()+face.GetNormal()).ToVector3());
                    normals.Add(Vector3.up);
                    normals.Add(Vector3.up);
                    indices[materialIndex].Add(vertexId);
                    vertexId++;
                    indices[materialIndex].Add(vertexId);
                    vertexId++;
			} else {
                foreach (var edge in face.Circulate()){
                    if (renderType == HMeshRendererType.BoundaryEdges){
                        if (!edge.IsBoundary()){
                            continue;
                        }
                    }
                    lines.Add(edge.prev.vert.position);
                    lines.Add(edge.vert.position);
                    normals.Add(Vector3.up);
                    normals.Add(Vector3.up);
                    indices[materialIndex].Add(vertexId);
                    vertexId++;
                    indices[materialIndex].Add(vertexId);
                    vertexId++;
                }
            }
		}
		mesh.Clear();
		mesh.vertices = lines.ToArray();
		mesh.normals = normals.ToArray();
		var meshTopology = renderType == HMeshRendererType.Vertices ? MeshTopology.Points : MeshTopology.Lines;
	    mesh.subMeshCount = indices.Count;
	    for (int i = 0; i < indices.Count; i++)
	    {
	        mesh.SetIndices(indices[i].ToArray(), meshTopology, i);
	    }

		mesh.RecalculateBounds();
		mesh.UploadMeshData(false);

	    if (meshFilter == null)
	    {
	        meshFilter = GetComponent<MeshFilter>();
	    }
	    if (meshFilter != null)
	    {
	        meshFilter.mesh = mesh;
	    }

	    return mesh;
	}

	public void CreateHMesh(){
		hmesh = HMesh.CreateTestMesh();
	}

	public void CreateHMeshTriangle(){
		hmesh = HMesh.CreateTestMeshTriangle();
	}

	public void CreateHMeshQuad(){
		hmesh = HMesh.CreateTestMeshQuad();
	}

    public HMesh CreateHMeshNGon(int n)
    {
        hmesh = HMesh.CreateTestMeshNGon(n);
        return hmesh;
    }

    public HMesh Triangulate(bool step=false)
    {
        if (step)
        {
            var faces = new List<Face>(hmesh.GetFacesRaw());
            faces.Sort((face, face1) =>
            {
                return -face.Circulate().Count + face1.Circulate().Count;
            });
            Debug.Log("Triangulate "+faces[0]);
            faces[0].Triangulate(true);
        } else {
            hmesh.Triangulate(step);
        }
        Debug.Log("Valid hmesh "+hmesh.IsValid());

        return hmesh;
    }

    public void ApplyTransform(){
		var localToWorld = transform.localToWorldMatrix;
		foreach (var vert in hmesh.GetVerticesRaw()){
			vert.position = localToWorld.MultiplyPoint(vert.position);
		}
		transform.position = Vector3.zero;
		transform.localScale = Vector3.one;
		transform.rotation = Quaternion.identity;
	}
}
