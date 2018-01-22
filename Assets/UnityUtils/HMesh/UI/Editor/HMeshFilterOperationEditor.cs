using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HMeshFilterOperation))]
public class HMeshFilterOperationEditor : Editor {
	public override void OnInspectorGUI(){
		HMeshFilterOperation myTarget = (HMeshFilterOperation)target;
	    var filter = myTarget.GetComponent<HMeshFilter>();
	    if (filter == null)
	    {
	        Debug.LogWarning("HMeshFilter not found");
	        return;
	    }
	    DrawDefaultInspector();
		if (GUILayout.Button("CreateTriangle"))
		{
		    CreateTriangle(filter);
		}
	    if (GUILayout.Button("CreateQuad"))
	    {
	        CreateQuad(filter);
	    }
	    if (GUILayout.Button("CreateNGon"))
	    {
	        CreateNGon(filter,myTarget.edges);
	    }
	    if (GUILayout.Button("ToMesh"))
	    {
	        CreateHMesh(filter, myTarget.meshMaterials);
	    }
	}

    private void CreateHMesh(HMeshFilter filter, Material[] myTargetMeshMaterial)
    {
        GameObject GO = new GameObject("HMesh_To_Mesh");
        var meshFilter = GO.AddComponent<MeshFilter>();
        meshFilter.mesh = filter.hMesh.Export(true);
        var meshRenderer = GO.AddComponent<MeshRenderer>();
        meshRenderer.materials = myTargetMeshMaterial;
    }

    private void CreateTriangle(HMeshFilter filter)
    {
        filter.SetHMesh(HMesh.CreateTestMeshTriangle(true));
    }

    private void CreateQuad(HMeshFilter filter)
    {
        filter.SetHMesh(HMesh.CreateTestMeshQuad());
    }

    private void CreateNGon(HMeshFilter filter,int n)
    {
        filter.SetHMesh(HMesh.CreateTestMeshNGon(n));
    }
}
