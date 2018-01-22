using System.IO;
using UnityEngine;
using UnityEditor;

public class ColladaExportTest : MonoBehaviour {

	[MenuItem("Collada/ExportTest")]
	static void Start ()
	{
	    var g = Selection.gameObjects;
	    ExportToCollada export = new ExportToCollada("/Users/mnob/programming/Unity/dtu_3d_model/UnityProject/Assets/scripts/test/ColTest/");
	    foreach (var go in g){
            if (go == null)
            {
                continue;
            }
            var mfs = go.GetComponentsInChildren<MeshFilter>();
		    foreach (var mf in mfs){
				if (mf == null)
				{
					continue;
				}
				var mats = mf.GetComponent<MeshRenderer>().sharedMaterials;
				export.AddMeshWithMaterials(mf.sharedMesh,mats, mf.transform.localToWorldMatrix, mf.name);
		    }
	    }
		export.Save("/Users/mnob/programming/Unity/dtu_3d_model/UnityProject/Assets/scripts/test/ColTest/test2.dae");
	}
}
