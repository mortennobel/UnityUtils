/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)


using UnityEngine;
using System.Collections;

public class BasicLineRenderer : MonoBehaviour {
	public Vector3[] positions = new Vector3[0];
	public Vector3[] normals = new Vector3[0];
	public Material material;
	
	public float width = 1.0f;
	
	private GameObject renderObject;
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private Mesh mesh;
	
	void Start(){
		Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetMaterial(Material newMaterial){
		this.material = newMaterial;
	}
	
	public void SetNormalCount(int count){
		Vector3[] newNormals = new Vector3[count];
		// copy existing data
		for (int i=0;i<Mathf.Min(count,normals.Length);i++){
			newNormals[i] = normals[i];
		}
		normals = newNormals;
	}
	
	public void SetNormal(int index, Vector3 normal){
		normals[index] = normal;
	}
	
	public void SetPosition(int index, Vector3 position){
		positions[index] = position;
	}
	
	public void SetVertexCount(int count){
		Vector3[] newPositions = new Vector3[count];
		// copy existing data
		for (int i=0;i<Mathf.Min(count,positions.Length);i++){
			newPositions[i] = positions[i];
		}
		positions = newPositions;
	}
	
	public Vector3[] GetNormals(){
		return normals;
	}
	
	public Vector3[] GetPositions(){
		return positions;
	}
	
	public float GetWidth(){
		return width;
	}
	
	public void SetWidth(float width){
		this.width = width;
	}
	
	public void Initialize(){
		renderObject = new GameObject();
		renderObject.name = "BasicLineRenderer - internal renderer";
	//	renderObject.hideFlags = HideFlags.HideInHierarchy;
		renderObject.transform.parent = gameObject.transform;
		meshFilter = renderObject.AddComponent<MeshFilter>();
		meshRenderer = renderObject.AddComponent<MeshRenderer>();
		meshRenderer.material = material;
		
		mesh = new Mesh();
	}
	
	public void UpdateMesh(){
		if (renderObject==null){
			Initialize();
		}
		
		if (positions.Length>1){
		
			Vector3[] vertices = new Vector3[(positions.Length-1)*4];
			Vector2[] uvs = new Vector2[(positions.Length-1)*4];
			int[] vertexIndices = new int[(positions.Length-1)*6];
			
			float halfWidth = width*0.5f;
			
			Vector3 normal = Vector3.up;
			for (int i=0,vIndex = 0,vIIndex = 0;i<positions.Length-1;i++,vIndex+=4,vIIndex+=6){
				Vector3 p1 = positions[i];
				Vector3 p2 = positions[i+1];
				
				if (i<normals.Length){
					normal = normals[i];
				}
				
				Vector3 p1p2 = p2-p1;
				Vector3 ortho = Vector3.Cross(normal, p1p2).normalized*halfWidth;
				
				vertices[vIndex] = p1+ortho;
				vertices[vIndex+1] = p1-ortho;
				vertices[vIndex+2] = p2+ortho;
				vertices[vIndex+3] = p2-ortho;
				
				uvs[vIndex]   = new Vector2(0,0);
				uvs[vIndex+1] = new Vector2(1,0);
				uvs[vIndex+2] = new Vector2(0,1);
				uvs[vIndex+2] = new Vector2(1,1);
				
				// triangle 1
				vertexIndices[vIIndex] = vIndex;
				vertexIndices[vIIndex+1] = vIndex+1;
				vertexIndices[vIIndex+2] = vIndex+2;
				// triangle 2
				vertexIndices[vIIndex+3] = vIndex+2;
				vertexIndices[vIIndex+4] = vIndex+1;
				vertexIndices[vIIndex+5] = vIndex+3;
			}
			
			mesh.vertices = vertices;
			mesh.triangles = vertexIndices;
			mesh.uv = uvs;
			meshFilter.mesh = mesh;
			
			meshRenderer.enabled = true;
		} else {
			meshRenderer.enabled = false;
		}
	}
}
