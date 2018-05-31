using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(HMeshRenderer))]
public class HMeshRendererEditor : Editor  {
	public override void OnInspectorGUI()
	{
		HMeshRenderer myTarget = (HMeshRenderer)target;
		DrawDefaultInspector();
		if (GUILayout.Button("UpdateMesh")){
			myTarget.UpdateMesh();
		}
		if (GUILayout.Button("Create HMesh plane")){
			myTarget.CreateHMesh();
			myTarget.UpdateMesh();
		}
		if (GUILayout.Button("Create HMesh triangle")){
			myTarget.CreateHMeshTriangle();
			myTarget.UpdateMesh();
		}
		if (GUILayout.Button("Create HMesh Quad")){
			myTarget.CreateHMeshQuad();
			myTarget.UpdateMesh();
		}
	    if (GUILayout.Button("Create HMesh 2-gon")){
	        var res = myTarget.CreateHMeshNGon(2);
	        myTarget.UpdateMesh();
	        Debug.Log(res.ToString());
	    }
	    if (GUILayout.Button("Create HMesh Hexagon")){
			var res = myTarget.CreateHMeshNGon(6);
			myTarget.UpdateMesh();
	        Debug.Log(res.ToString());
	    }
	    if (GUILayout.Button("Create HMesh Octogon")){
	        var res = myTarget.CreateHMeshNGon(8);
	        myTarget.UpdateMesh();
	        Debug.Log(res.ToString());
	    }
	    if (GUILayout.Button("Create HMesh Non convex")){
	        var res = myTarget.CreateHMeshNGon(32);
	        foreach (var ver in res.GetVertices())
	        {
	            if (ver.position.z < 0)
	            {
	                var pos = ver.positionD;
	                pos.z *= -1;
	                pos *= 0.8;
	                ver.positionD = pos;

	            }
	        }
	        myTarget.UpdateMesh();
	        Debug.Log(res.ToString());
	    }
	    if (GUILayout.Button("RandomRotation")){
	        HMesh res = myTarget.hmesh;
	        var randomRotation = Random.rotation;
	        foreach (var v in res.GetVerticesRaw())
	        {

	            v.position = randomRotation * v.position;
	        }
	        myTarget.UpdateMesh();
	        Debug.Log(res.ToString());
	    }
	    if (GUILayout.Button("Triangulate")){
	        var res = myTarget.Triangulate();
	        myTarget.UpdateMesh();
	        Debug.Log(res.ToString());
	    }
	    if (GUILayout.Button("Triangulate step")){
	        var res = myTarget.Triangulate(true);
	        myTarget.UpdateMesh();
	        Debug.Log(res.ToString());
	    }

	    if (GUILayout.Button("Apply transform")){
			myTarget.ApplyTransform();
			myTarget.UpdateMesh();
		}
	    if (GUILayout.Button("Set default colors ")){
	        myTarget.colors = new Color[]
	        {
	            Color.red,
	            Color.green,
	            Color.blue,
	            Color.yellow,
	            Color.cyan,
	            Color.grey
	        };
	    }

	    if (GUILayout.Button("Build from obj"))
	    {
	        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
                             "# www.blender.org\n"+
	                         "mtllib quad.mtl\n"+
	                         "o Plane\n"+
	                         "v -1.000000 0.000000 1.000000\n"+
	                         "v 0.000000 0.000000 0.000000\n"+
	                         "v -1.000000 0.000000 -1.000000\n"+
	                         "v 1.000000 0.000000 -1.000000\n"+
	                         "v 1.000000 0.000000 1.000000\n"+
	                         "vn 0.0000 1.0000 0.0000\n"+
	                         "usemtl None\n"+
	                         "s off\n"+
	                         "f 1//1 2//1 3//1\n"+
                             "f 4//1 2//1 5//1\n";
	        myTarget.hmesh = new HMesh();
	        myTarget.hmesh.BuildFromObj(objTest);
	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }
        if (GUILayout.Button("Build from obj - multi color"))
	    {
	        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
	                         "# www.blender.org\n"+
	                         "mtllib untitled.mtl\n"+
	                         "o Plane\n"+
	                         "v -1.000000 0.000000 1.000000\n"+ //
	                         "v 1.000000 1.000000 1.000000\n"+  // vertex moved
	                         "v -1.000000 0.000000 -1.000000\n"+//
	                         "v 1.000000 0.000000 -1.000000\n"+ //
	                         "v 0.000000 0.000000 0.000000\n"+  //
	                         "vn 0.0000 1.0000 0.0000\n"+
	                         "usemtl None\n"+
	                         "s off\n"+
	                         "f 1//1 2//1 5//1\n"+
	                         "f 4//1 3//1 5//1\n"+
	                         "f 5//1 2//1 4//1\n"+
	                         "f 5//1 3//1 1//1\n"+
	                         "";
	        myTarget.hmesh = new HMesh();
	        myTarget.hmesh.BuildFromObj(objTest);
	        myTarget.hmesh.GetFacesRaw()[0].label = 1;
	        myTarget.hmesh.GetFacesRaw()[2].label = 1;
	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }
	    if (GUILayout.Button("Build from obj - flat"))
	    {
	        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
            "# www.blender.org\n"+
	                         "mtllib test.mtl\n"+
	                         "    o Plane_Plane.002\n"+
	                         "v -1.000000 0.000000 1.000000\n"+
	                         "v 0.390552 0.000000 0.390552\n"+
	                         "v -0.390552 0.000000 -0.390552\n"+
	                         "v 1.000000 0.000000 -1.000000\n"+
	                         "vn 0.0000 1.0000 0.0000\n"+
	                         "usemtl None\n"+
	                         "s off\n"+
	                         "f 1//1 2//1 4//1\n"+
	                         "f 4//1 3//1 1//1\n";
	        myTarget.hmesh = new HMesh();
	        myTarget.hmesh.BuildFromObj(objTest);
	        myTarget.hmesh.GetFacesRaw()[0].label = 1;
	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }
	    if (GUILayout.Button("Build from obj - plane"))
	    {
	        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
	                         "# www.blender.org\n"+
	                         "mtllib test.mtl\n"+
	                         "o Plane\n"+
	                         "v -1.000000 0.000000 1.000000\n"+
	                         "v 1.000000 0.000000 1.000000\n"+
	                         "v -1.000000 0.000000 -1.000000\n"+
	                         "v 1.000000 0.000000 -1.000000\n"+
	                         "v -1.000000 0.000000 0.000000\n"+
	                         "v 0.000000 0.000000 1.000000\n"+
	                         "v 1.000000 0.000000 0.000000\n"+
	                         "v 0.000000 0.000000 -1.000000\n"+
	                         "v 0.000000 0.000000 0.000000\n"+
	                         "vn 0.0000 1.0000 0.0000\n"+
	                         "usemtl None\n"+
	                         "s off\n"+
	                         "f 7//1 8//1 9//1\n"+
	                         "f 9//1 3//1 5//1\n"+
	                         "f 6//1 5//1 1//1\n"+
	                         "f 2//1 9//1 6//1\n"+
	                         "f 7//1 4//1 8//1\n"+
	                         "f 9//1 8//1 3//1\n"+
	                         "f 6//1 9//1 5//1\n"+
	                         "f 2//1 7//1 9//1";


	        myTarget.hmesh = new HMesh();
	        myTarget.hmesh.BuildFromObj(objTest);
	        foreach (var f in myTarget.hmesh.GetFaces())
	        {
	            if (f.GetCenter().x > 0)
	            {
	            //    f.label = 1;
	            }
	        }
	        myTarget.hmesh.GetFacesRaw()[0].label = 1;
	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }
		if (GUILayout.Button("Build from obj - optimizeable"))
		{
			string objTest = "v -30.025 40.49966 -14.83595\n"+
			"v -94.63337 40.49973 -147.4859\n"+
			"v -113.9539 40.49965 15.86017\n"+
			"v -113.9539 40.49976 -214.1117\n"+
			"v -94.63337 40.49973 -155.2081\n"+
			"v -90.32684 40.49973 -147.4859\n"+
			"v -72.21552 40.49973 -147.4859\n"+
			"v -55.54735 40.49973 -147.4859\n"+
			"v -6.504288 40.49969 -76.80889\n"+
			"v -6.504288 40.49966 0.6029686\n"+
			"v -6.504288 40.49966 2.818535\n"+
			"v -6.504288 40.49966 3.218271\n"+
			"v -6.504288 40.49965 11.65778\n"+
			"v -31.79999 40.49967 -32.091\n"+
			"v -31.79999 40.49968 -35.57327\n"+
			"v -100.9474 40.49968 -35.57016\n"+
			"v -94.05646 40.49968 -32.09412\n"+
			"v -44.48075 40.49967 -32.09164\n"+
			"v -38.34414 40.49967 -32.09133\n"+
			"v -39.51749 40.49968 -35.57293\n"+
			"o label0\n"+
			"f 3 2 4\n"+
			"f 16 3 17\n"+
			"f 18 1 19\n"+
			"f 9 15 14\n"+
			"f 4 2 5\n"+
			"f 15 8 7\n"+
			"f 15 9 8\n"+
			"f 1 10 9\n"+
			"f 10 1 11\n"+
			"f 12 11 1\n"+
			"f 12 3 13\n"+
			"f 9 14 1\n"+
			"f 20 15 7\n"+
			"f 1 17 12\n"+
			"f 2 3 16\n"+
			"f 17 3 12\n"+
			"f 17 1 18\n"+
			"f 16 6 2\n"+
			"f 20 6 16\n"+
			"f 20 7 6\n"+
			"f 19 1 14\n"+
			"o label1\n"+
			"f 15 20 14\n"+
			"f 16 18 19\n"+
			"f 16 17 18\n"+
			"f 20 19 14\n"+
			"f 19 20 16\n"+
			"";


			myTarget.hmesh = new HMesh();
			myTarget.hmesh.BuildFromObj(objTest);
			foreach (var f in myTarget.hmesh.GetFaces())
			{
				if (f.GetCenter().x > 0)
				{
					//    f.label = 1;
				}
			}
			myTarget.hmesh.GetFacesRaw()[0].label = 1;
			myTarget.hmesh.IsValid();
			myTarget.UpdateMesh();
		}
	    if (GUILayout.Button("Build from obj - degenerate"))
	    {
	        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
                "# www.blender.org\n"+
                "mtllib degenerate.mtl\n"+
                "o Plane\n"+
                "v -0.477655 -1.045174 1.105640\n"+
                "v -0.477655 -1.045174 -0.894360\n"+
                "v -0.477655 -0.480058 0.105640\n"+
                "v 0.522345 -1.045174 1.105640\n"+
                "v 0.522345 -1.045174 -0.894360\n"+
                "v 0.522345 -1.045174 0.105640\n"+
                "v -0.477655 -1.045174 0.105640\n"+
                "v 0.522345 -1.045174 0.605640\n"+
                "v -0.477655 -1.045174 0.105640\n"+
                "v 0.522345 -1.045174 -0.394360\n"+
                "vn 0.0000 1.0000 0.0000\n"+
                "vn 0.0000 0.0000 1.0000\n"+
                "vn 0.0000 0.0000 -1.0000\n"+
                "usemtl None\n"+
                "s off\n"+
                "f 1//1 4//1 8//1\n"+
                "f 10//1 2//1 9//1\n"+
                "f 7//2 6//2 3//2\n"+
                "f 7//1 8//1 6//1\n"+
                "f 6//1 10//1 9//1\n"+
                "f 1//1 8//1 7//1\n"+
                "f 6//3 9//3 3//3\n"+
                "f 10//1 5//1 2//1\n";


	        myTarget.hmesh = new HMesh();
	        myTarget.hmesh.BuildFromObj(objTest);

	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }
	    if (GUILayout.Button("Build from obj - flat degenerate 1"))
	    {
	        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
                "# www.blender.org\n"+
                "mtllib degenerate1.mtl\n"+
                "    o degenerate1\n"+
                "v -1.000000 0.000000 1.000000\n"+
                "v 1.000000 0.000000 1.000000\n"+
                "v -1.000000 0.000000 -1.000000\n"+
                "v 1.000000 0.000000 -1.000000\n"+
                "v -1.000000 0.000000 0.000000\n"+
                "v 0.000000 0.000000 1.000000\n"+
                "v 1.000000 0.000000 0.000000\n"+
                "v 0.000000 0.000000 -1.000000\n"+
                "v 0.633916 0.000000 0.000000\n"+
                "v -0.636292 0.000000 0.000000\n"+
                "v 0.000000 0.000000 0.000000\n"+
                "v 0.000000 0.000000 0.000000\n"+
                "v -0.357594 0.000000 -0.016980\n"+
                "v -0.357594 0.000000 -0.016980\n"+
                "v 0.316958 0.000000 0.000000\n"+
                "vn 0.0000 1.0000 0.0000\n"+
                "vn 0.0000 -1.0000 0.0000\n"+
                "usemtl None\n"+
                "s off\n"+
                "f 5//1 10//1 14//1 12//1 8//1 3//1\n"+
                "f 1//1 6//1 11//1 13//1 10//1 5//1\n"+
                "f 6//1 2//1 7//1 9//1 11//1\n"+
                "f 11//2 9//2 15//2 12//2 14//2 10//2 13//2\n"+
                "f 12//1 15//1 9//1 7//1 4//1 8//1\n"+
                "";


	        myTarget.hmesh = new HMesh();
	        myTarget.hmesh.BuildFromObj(objTest);

	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }
	    if (GUILayout.Button("Build from obj - flat degenerate 2"))
	    {
	        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
                "# www.blender.org\n"+
                "mtllib degenerate2.mtl\n"+
                "    o Degenerate2\n"+
                "v -1.000000 0.000000 1.000000\n"+
                "v 1.000000 0.000000 1.000000\n"+
                "v -1.000000 0.000000 -1.000000\n"+
                "v 1.000000 0.000000 -1.000000\n"+
                "v -1.000000 0.000000 0.000000\n"+
                "v 0.000000 0.000000 1.000000\n"+
                "v 1.000000 0.000000 0.000000\n"+
                "v 0.000000 0.000000 -1.000000\n"+
                "v 0.584243 0.000000 0.000000\n"+
                "v -0.029986 0.000000 0.012126\n"+
                "v -0.029986 0.000000 0.012126\n"+
                "v 0.000000 0.000000 -0.681619\n"+
                "v -0.180637 0.000000 0.149689\n"+
                "v -0.029986 0.000000 0.012126\n"+
                "v -0.046522 0.000000 -0.408366\n"+
                "v -0.046522 0.000000 -0.408366\n"+
                "vn -0.0000 1.0000 -0.0000\n"+
                "usemtl None\n"+
                "s off\n"+
                "f 5//1 10//1 16//1 12//1 8//1 3//1\n"+
                "f 1//1 6//1 11//1 13//1 10//1 5//1\n"+
                "f 6//1 2//1 7//1 9//1 11//1\n"+
                "f 11//1 9//1 14//1 15//1 12//1 16//1 10//1 13//1\n"+
                "f 12//1 15//1 14//1 9//1 7//1 4//1 8//1\n"+
                "";


	        myTarget.hmesh = new HMesh();
	        myTarget.hmesh.BuildFromObj(objTest);

	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }
	    if (GUILayout.Button("Build from obj - flat degenerate 3"))
	    {
	        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
            "# www.blender.org\n"+
	        "mtllib degenerate3.mtl\n"+
	        "o Denegerate3\n"+
	        "v -1.000000 0.000000 1.000000\n"+
	        "v 1.000000 0.000000 1.000000\n"+
	        "v -1.000000 0.000000 -1.000000\n"+
	        "v 1.000000 0.000000 -1.000000\n"+
	        "v -1.000000 0.000000 0.000000\n"+
	        "v 0.000000 0.000000 1.000000\n"+
	        "v 1.000000 0.000000 0.000000\n"+
	        "v 0.000000 0.000000 -1.000000\n"+
	        "v 0.809701 0.000000 0.000000\n"+
	        "v -0.795454 0.000000 0.000000\n"+
	        "v 0.000000 0.000000 0.026553\n"+
	        "v 0.000000 0.000000 0.026553\n"+
	        "v 0.417223 0.000000 0.020130\n"+
	        "v 0.531795 0.000000 0.383705\n"+
	        "v 0.628144 -0.000000 -0.286403\n"+
	        "v 0.417223 0.000000 0.020130\n"+
	        "v -0.298932 0.000000 0.031520\n"+
	        "v -0.478770 0.000000 -0.367569\n"+
	        "v -0.622377 0.000000 0.355869\n"+
	        "v -0.298932 0.000000 0.031520\n"+
	        "vn -0.0000 1.0000 -0.0000\n"+
	        "usemtl None\n"+
	        "s off\n"+
	        "f 10//1 19//1 20//1 11//1 13//1 14//1 9//1 15//1 16//1 12//1 17//1 18//1\n"+
	        "f 5//1 10//1 18//1 17//1 12//1 8//1 3//1\n"+
	        "f 1//1 6//1 11//1 20//1 19//1 10//1 5//1\n"+
	        "f 6//1 2//1 7//1 9//1 14//1 13//1 11//1\n"+
	        "f 12//1 16//1 15//1 9//1 7//1 4//1 8//1\n"+
	        "";


	        myTarget.hmesh = new HMesh();
	        myTarget.hmesh.BuildFromObj(objTest);

	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }
		if (GUILayout.Button("Build from obj - cube"))
		{
			string objTest = "# Blender v2.79 (sub 0) OBJ File: ''\n"+
			"# www.blender.org\n"+
			"o Cube\n"+
			"v 1.000000 -1.000000 -1.000000\n"+
			"v 1.000000 -1.000000 1.000000\n"+
			"v -1.000000 -1.000000 1.000000\n"+
			"v -1.000000 -1.000000 -1.000000\n"+
			"v 1.000000 1.000000 -0.999999\n"+
			"v 0.999999 1.000000 1.000001\n"+
			"v -1.000000 1.000000 1.000000\n"+
			"v -1.000000 1.000000 -1.000000\n"+
			"vn 0.0000 -1.0000 0.0000\n"+
			"vn 0.0000 1.0000 0.0000\n"+
			"vn 1.0000 0.0000 0.0000\n"+
			"vn -0.0000 -0.0000 1.0000\n"+
			"vn -1.0000 -0.0000 -0.0000\n"+
			"vn 0.0000 0.0000 -1.0000\n"+
			"s off\n"+
			"f 1//1 2//1 3//1 4//1\n"+
			"f 5//2 8//2 7//2 6//2\n"+
			"f 1//3 5//3 6//3 2//3\n"+
			"f 2//4 6//4 7//4 3//4\n"+
			"f 3//5 7//5 8//5 4//5\n"+
			"f 5//6 1//6 4//6 8//6\n"+
			"";


	        myTarget.hmesh = new HMesh();
	        myTarget.hmesh.BuildFromObj(objTest);

	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }
		if (GUILayout.Button("Build from obj - cube smooth"))
		{
			string objTest = "# Blender v2.79 (sub 0) OBJ File: ''\n"+
			"# www.blender.org\n"+
			"mtllib cube.mtl\n"+
			"o Cube\n"+
			"v 1.000000 -1.000000 -1.000000\n"+
			"v 1.000000 -1.000000 1.000000\n"+
			"v -1.000000 -1.000000 1.000000\n"+
			"v -1.000000 -1.000000 -1.000000\n"+
			"v 1.000000 1.000000 -0.999999\n"+
			"v 0.999999 1.000000 1.000001\n"+
			"v -1.000000 1.000000 1.000000\n"+
			"v -1.000000 1.000000 -1.000000\n"+
			"vn 0.5773 -0.5773 -0.5773\n"+
			"vn 0.5773 -0.5773 0.5773\n"+
			"vn -0.5773 -0.5773 0.5773\n"+
			"vn -0.5773 -0.5773 -0.5773\n"+
			"vn 0.5773 0.5773 -0.5773\n"+
			"vn -0.5773 0.5773 -0.5773\n"+
			"vn -0.5773 0.5773 0.5773\n"+
			"vn 0.5773 0.5773 0.5773\n"+
			"usemtl Material\n"+
			"s 1\n"+
			"f 1//1 2//2 3//3 4//4\n"+
			"f 5//5 8//6 7//7 6//8\n"+
			"f 1//1 5//5 6//8 2//2\n"+
			"f 2//2 6//8 7//7 3//3\n"+
			"f 3//3 7//7 8//6 4//4\n"+
			"f 5//5 1//1 4//4 8//6\n"+
			"";


	        myTarget.hmesh = new HMesh();
	        myTarget.hmesh.BuildFromObj(objTest);

	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }
	    if (GUILayout.Button("Build from obj - test"))
	    {
	        string objTest = myTarget.objfile;

	        myTarget.hmesh = new HMesh();
	        myTarget.hmesh.BuildFromObj(objTest);
	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }

	    if (GUILayout.Button("Simplify"))
	    {

            int res = HMeshSimplification.SimplifyByCollapse(myTarget.hmesh);
	        Debug.Log("Res "+res);
	        Debug.Log("Valid "+myTarget.hmesh.IsValid());
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }

	    if (GUILayout.Button("Optimize"))
	    {

	        HMeshOptimizer optimizer = new HMeshOptimizer();
	        optimizer.FaceLabelContrain = myTarget.OptimizeFaceLabelContrain;
	        optimizer.FaceNormalContrain = myTarget.OptimizeFaceNormalContrain;
	        switch (myTarget.optStategy)
	        {
	            case OptimizationStrategy.MaximizeMinAngle:
	                optimizer.MaximizeMinAngle(myTarget.hmesh,myTarget.OptimizeMinMaxAngleThreshold);
	                break;
	            case OptimizationStrategy.MinimizeDihedralAngle:
	                optimizer.MinimizeDihedralAngle(myTarget.hmesh);
	                break;
	            case OptimizationStrategy.OptimizeValency:
	                optimizer.OptimizeValency(myTarget.hmesh);
	                break;
	        }
	        myTarget.hmesh.IsValid();
	        myTarget.UpdateMesh();
	        Debug.Log(myTarget.hmesh.CreateDebugData());
	    }

	    if (GUILayout.Button("Export Split 2x1x2"))
	    {
		    var hmesh = myTarget.hmesh;
	        var meshes = hmesh.ExportSplit(new Vector3i(2,1,2), null, 30);
	        foreach (var mesh in meshes)
	        {
	            GameObject go = new GameObject("SplitMesh");
	            var mr = go.AddComponent<MeshRenderer>();
	            var mat = new Material(Shader.Find("Standard"));
	            mat.color = Color.white;
	            mr.material = mat;
	            var mf = go.AddComponent<MeshFilter>();
	            mf.mesh = mesh;
	        }
	    }

	    if (GUILayout.Button("Split (edge/material)"))
	    {

		    var hmesh = myTarget.hmesh;
		    var splitHMesh = hmesh.Split(true, 89);
		    
		    Debug.Log("str1:" + splitHMesh.CreateDebugData());
		    Debug.Log("Mesh valid "+splitHMesh.IsValid());
		    var fileObj = splitHMesh.ExportObj();
		    File.WriteAllText("/Users/mnob/Desktop/export-obj.obj", fileObj);
		    //Debug.Assert(splitHMesh.IsValid());
		    //var meshes = splitHMesh.ExportSplit(new Vector3i(1, 1, 1)); 
		    /*foreach (var mesh in meshes)
		    {
			    GameObject go = new GameObject("SplitMesh");
			    var mr = go.AddComponent<MeshRenderer>();
			    var mat = new Material(Shader.Find("Standard"));
			    mat.color = Color.white;
			    mr.material = mat;
			    var mf = go.AddComponent<MeshFilter>();
			    mf.mesh = mesh;
		    }*/
	    }
		
		if (GUILayout.Button("Export OBJ"))
	    {

	        var fileObj = myTarget.hmesh.ExportObj();
	        File.WriteAllText("/Users/mnob/Desktop/export-obj.obj", fileObj);
	    }
		
		
	}
}
