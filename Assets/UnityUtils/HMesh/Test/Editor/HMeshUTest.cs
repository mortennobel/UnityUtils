using System;
using System.Runtime.InteropServices;
using UnityEngine;
using NUnit.Framework;
using Random = UnityEngine.Random;

[TestFixture]
public class HMeshUTest
{

    [Test]
    public void HMeshCollapseBug()
    {
        var obj = "# HMesh export - neighbourhood of face 23776\n" +
                  "v -290.961 44.2496 -380.715\n" +
                  "v -290.961 44.2496 -380.716\n" +
                  "v -340.644 44.2496 -380.738\n" +
                  "v -280.572 44.2496 -380.711\n" +
                  "v -280.571 44.2496 -380.712\n" +
                  "# face id 23776 edges 72214 72212 72213 \n" +
                  "f 1 2 3 \n" +
                  "# face id 28275 edges 86385 86382 86383 86384 \n" +
                  "f 2 1 4 5 ";
        //for (int i=0;i<3;i++){
        int i = 1;
        var hmesh = new HMesh();
        hmesh.BuildFromObj(obj, false);
        
        Debug.Log(hmesh.CreateDebugData());
        
        Assert.AreEqual(0, hmesh.SplitNonManifoldVertices());
        Assert.AreEqual(7, hmesh.GetHalfedgesRaw().Count);
        Assert.AreEqual(2, hmesh.GetFacesRaw().Count);
        Assert.AreEqual(5, hmesh.GetVerticesRaw().Count);
        
        hmesh.GetFacesRaw()[0].Circulate()[i].Collapse(true);

        Assert.AreEqual(3, hmesh.GetHalfedgesRaw().Count);
        Assert.AreEqual(1, hmesh.GetFacesRaw().Count);
        Assert.AreEqual(3, hmesh.GetVerticesRaw().Count);

        Assert.AreEqual(true, hmesh.IsValid());
    }

    [Test]
    public void HMeshSimplify()
    {
        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n" +
                         "# www.blender.org\n" +
                         "mtllib untitled.mtl\n" +
                         "o Plane\n" +
                         "v -1.000000 0.000000 1.000000\n" +
                         "v 1.000000 0.000000 1.000000\n" +
                         "v -1.000000 0.000000 -1.000000\n" +
                         "v 1.000000 0.000000 -1.000000\n" +
                         "v 0.000000 0.000000 0.000000\n" +
                         "vn 0.0000 1.0000 0.0000\n" +
                         "usemtl None\n" +
                         "s off\n" +
                         "f 1//1 2//1 5//1\n" +
                         "f 4//1 3//1 5//1\n" +
                         "f 5//1 2//1 4//1\n" +
                         "f 5//1 3//1 1//1\n" +
                         "";
        var hmesh = new HMesh();
        hmesh.BuildFromObj(objTest);
        hmesh.GetFacesRaw()[0].label = 1;
        hmesh.GetFacesRaw()[2].label = 1;
        hmesh.IsValid();
        var collapsed = HMeshSimplification.SimplifyByCollapse(hmesh);
        Assert.AreEqual(1, collapsed);
        Assert.AreEqual(2, hmesh.GetFacesRaw().Count);
    }

    [Test]
    public void HMeshSimplifyFailed()
    {
        // Simplify should fail due to minimum angle
        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n" +
                         "# www.blender.org\n" +
                         "mtllib untitled.mtl\n" +
                         "o Plane\n" +
                         "v -1.000000 0.000000 1.000000\n" +
                         "v 1.000000 0.000000 1.000000\n" +
                         "v -1.000000 0.000000 -1.000000\n" +
                         "v 1.000000 0.000000 -1.000000\n" +
                         "v 0.000000 0.500000 0.000000\n" +
                         "vn 0.0000 1.0000 0.0000\n" +
                         "usemtl None\n" +
                         "s off\n" +
                         "f 1//1 2//1 5//1\n" +
                         "f 4//1 3//1 5//1\n" +
                         "f 5//1 2//1 4//1\n" +
                         "f 5//1 3//1 1//1\n" +
                         "";
        var hmesh = new HMesh();
        hmesh.BuildFromObj(objTest);
        hmesh.GetFacesRaw()[0].label = 1;
        hmesh.GetFacesRaw()[2].label = 1;
        hmesh.IsValid();
        var collapsed = HMeshSimplification.SimplifyByCollapse(hmesh);
        Assert.AreEqual(0, collapsed);
        Assert.AreEqual(4, hmesh.GetFacesRaw().Count);
    }

    [Test]
    public void HMeshSimplifyOnEdge()
    {
        // Simplify should fail due to minimum angle
        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n" +
                         "# www.blender.org\n" +
                         "mtllib untitled.mtl\n" +
                         "o Plane\n" +
                         "v -1.000000 0.000000 1.000000\n" + //
                         "v 1.000000 1.000000 1.000000\n" + // vertex moved
                         "v -1.000000 0.000000 -1.000000\n" + //
                         "v 1.000000 0.000000 -1.000000\n" + //
                         "v 0.000000 0.000000 0.000000\n" + //
                         "vn 0.0000 1.0000 0.0000\n" +
                         "usemtl None\n" +
                         "s off\n" +
                         "f 1//1 2//1 5//1\n" +
                         "f 4//1 3//1 5//1\n" +
                         "f 5//1 2//1 4//1\n" +
                         "f 5//1 3//1 1//1\n" +
                         "";
        //Debug.Log(objTest);
        var hmesh = new HMesh();
        hmesh.BuildFromObj(objTest);
        hmesh.GetFacesRaw()[0].label = 1;
        hmesh.GetFacesRaw()[2].label = 1;
        hmesh.IsValid();
        var collapsed = HMeshSimplification.SimplifyByCollapse(hmesh);
        Assert.AreEqual(1, collapsed);
        Assert.AreEqual(2, hmesh.GetFacesRaw().Count);
    }

    [Test]
    public void HMeshSplitTest()
    {
        for (int i = 0; i < 10; i++)
        {
            // Simplify should fail due to minimum angle
            string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n" +
                             "# www.blender.org\n" +
                             "mtllib test.mtl\n" +
                             "o Plane_Plane.001\n" +
                             "v -1.000000 0.000000 1.000000\n" +
                             "v 1.000000 0.000000 1.000000\n" +
                             "v -1.000000 0.000000 -1.000000\n" +
                             "v 1.000000 0.000000 -1.000000\n" +
                             "v -1.000000 0.000000 0.000000\n" +
                             "v 0.000000 0.000000 1.000000\n" +
                             "v 1.000000 0.000000 0.000000\n" +
                             "v 0.000000 0.000000 -1.000000\n" +
                             "v 0.000000 0.000000 0.000000\n" +
                             "v -1.000000 0.000000 0.500000\n" +
                             "v 0.500000 0.000000 1.000000\n" +
                             "v 1.000000 0.000000 -0.500000\n" +
                             "v -0.500000 0.000000 -1.000000\n" +
                             "v -1.000000 0.000000 -0.500000\n" +
                             "v -0.500000 0.000000 1.000000\n" +
                             "v 1.000000 0.000000 0.500000\n" +
                             "v 0.500000 0.000000 -1.000000\n" +
                             "v 0.000000 0.000000 -0.500000\n" +
                             "v 0.000000 0.000000 0.500000\n" +
                             "v -0.500000 0.000000 0.000000\n" +
                             "v 0.500000 0.000000 0.000000\n" +
                             "v 0.500000 0.000000 0.500000\n" +
                             "v -0.500000 0.000000 0.500000\n" +
                             "v -0.500000 0.000000 -0.500000\n" +
                             "v 0.500000 0.000000 -0.500000\n" +
                             "vn 0.0000 1.0000 0.0000\n" +
                             "usemtl None\n" +
                             "s off\n" +
                             "f 12//1 17//1 25//1\n" +
                             "f 18//1 13//1 24//1\n" +
                             "f 19//1 20//1 23//1\n" +
                             "f 16//1 21//1 22//1\n" +
                             "f 22//1 9//1 19//1\n" +
                             "f 11//1 19//1 6//1\n" +
                             "f 2//1 22//1 11//1\n" +
                             "f 23//1 5//1 10//1\n" +
                             "f 15//1 10//1 1//1\n" +
                             "f 6//1 23//1 15//1\n" +
                             "f 24//1 3//1 14//1\n" +
                             "f 20//1 14//1 5//1\n" +
                             "f 9//1 24//1 20//1\n" +
                             "f 25//1 8//1 18//1\n" +
                             "f 21//1 18//1 9//1\n" +
                             "f 7//1 25//1 21//1\n" +
                             "f 12//1 4//1 17//1\n" +
                             "f 18//1 8//1 13//1\n" +
                             "f 19//1 9//1 20//1\n" +
                             "f 16//1 7//1 21//1\n" +
                             "f 22//1 21//1 9//1\n" +
                             "f 11//1 22//1 19//1\n" +
                             "f 2//1 16//1 22//1\n" +
                             "f 23//1 20//1 5//1\n" +
                             "f 15//1 23//1 10//1\n" +
                             "f 6//1 19//1 23//1\n" +
                             "f 24//1 13//1 3//1\n" +
                             "f 20//1 24//1 14//1\n" +
                             "f 9//1 18//1 24//1\n" +
                             "f 25//1 17//1 8//1\n" +
                             "f 21//1 25//1 18//1\n" +
                             "f 7//1 12//1 25//1\n";
            ;
            // Debug.Log(objTest);
            var hmesh = new HMesh();
            hmesh.BuildFromObj(objTest);
            int vertices = hmesh.GetVerticesRaw().Count;
            int edges = hmesh.GetHalfedgesRaw().Count;
            int faces = hmesh.GetFacesRaw().Count;
            var face = hmesh.GetFacesRaw()[Random.Range(0, hmesh.GetFacesRaw().Count)];
            var newVertex = face.Split();
            foreach (var he in newVertex.Circulate())
            {
                Assert.AreEqual(true, he.IsValid());
                Assert.AreEqual(true, he.vert.IsValid());
            }
            Assert.AreEqual(true, face.IsValid());
            Assert.AreEqual(true, hmesh.IsValid());
            Assert.AreEqual(vertices + 1, hmesh.GetVerticesRaw().Count);
            Assert.AreEqual(edges + 6, hmesh.GetHalfedgesRaw().Count);
            Assert.AreEqual(faces + 2, hmesh.GetFacesRaw().Count);
        }
    }

    [Test]
    public void HMeshCollapseFace()
    {
        // Simplify should fail due to minimum angle
        string triangulatedPlane = "# Blender v2.78 (sub 0) OBJ File: ''\n" +
                                   "# www.blender.org\n" +
                                   "mtllib untitled.mtl\n" +
                                   "o Plane\n" +
                                   "v -1.000000 0.000000 1.000000\n" +
                                   "v 1.000000 0.000000 1.000000\n" +
                                   "v -1.000000 0.000000 -1.000000\n" +
                                   "v 1.000000 0.000000 -1.000000\n" +
                                   "v -1.000000 0.000000 0.000000\n" +
                                   "v 0.000000 0.000000 1.000000\n" +
                                   "v 1.000000 0.000000 0.000000\n" +
                                   "v 0.000000 0.000000 -1.000000\n" +
                                   "v 0.000000 0.000000 0.000000\n" +
                                   "v -1.000000 0.000000 0.500000\n" +
                                   "v 0.500000 0.000000 1.000000\n" +
                                   "v 1.000000 0.000000 -0.500000\n" +
                                   "v -0.500000 0.000000 -1.000000\n" +
                                   "v -1.000000 0.000000 -0.500000\n" +
                                   "v -0.500000 0.000000 1.000000\n" +
                                   "v 1.000000 0.000000 0.500000\n" +
                                   "v 0.500000 0.000000 -1.000000\n" +
                                   "v 0.000000 0.000000 -0.500000\n" +
                                   "v 0.000000 0.000000 0.500000\n" +
                                   "v -0.500000 0.000000 0.000000\n" +
                                   "v 0.500000 0.000000 0.000000\n" +
                                   "v 0.500000 0.000000 0.500000\n" +
                                   "v -0.500000 0.000000 0.500000\n" +
                                   "v -0.500000 0.000000 -0.500000\n" +
                                   "v 0.500000 0.000000 -0.500000\n" +
                                   "vn 0.0000 1.0000 0.0000\n" +
                                   "usemtl None\n" +
                                   "s off\n" +
                                   "f 12//1 17//1 25//1\n" +
                                   "f 18//1 13//1 24//1\n" +
                                   "f 19//1 20//1 23//1\n" +
                                   "f 16//1 21//1 22//1\n" +
                                   "f 22//1 9//1 19//1\n" +
                                   "f 11//1 19//1 6//1\n" +
                                   "f 2//1 22//1 11//1\n" +
                                   "f 23//1 5//1 10//1\n" +
                                   "f 15//1 10//1 1//1\n" +
                                   "f 6//1 23//1 15//1\n" +
                                   "f 24//1 3//1 14//1\n" +
                                   "f 20//1 14//1 5//1\n" +
                                   "f 9//1 24//1 20//1\n" +
                                   "f 25//1 8//1 18//1\n" +
                                   "f 21//1 18//1 9//1\n" +
                                   "f 7//1 25//1 21//1\n" +
                                   "f 12//1 4//1 17//1\n" +
                                   "f 18//1 8//1 13//1\n" +
                                   "f 19//1 9//1 20//1\n" +
                                   "f 16//1 7//1 21//1\n" +
                                   "f 22//1 21//1 9//1\n" +
                                   "f 11//1 22//1 19//1\n" +
                                   "f 2//1 16//1 22//1\n" +
                                   "f 23//1 20//1 5//1\n" +
                                   "f 15//1 23//1 10//1\n" +
                                   "f 6//1 19//1 23//1\n" +
                                   "f 24//1 13//1 3//1\n" +
                                   "f 20//1 24//1 14//1\n" +
                                   "f 9//1 18//1 24//1\n" +
                                   "f 25//1 17//1 8//1\n" +
                                   "f 21//1 25//1 18//1\n" +
                                   "f 7//1 12//1 25//1";
        int maxFaces = 2;
        for (int i = 0; i < maxFaces; i++)
        {
            var hmesh = new HMesh();
            hmesh.BuildFromObj(triangulatedPlane);
            maxFaces = hmesh.GetFacesRaw().Count;
            bool isFaceConnectedToBoundary = false;
            foreach (var he in hmesh.GetFacesRaw()[i].Circulate())
            {
                if (he.vert.IsBoundary())
                {
                    isFaceConnectedToBoundary = true;
                }
            }
            if (isFaceConnectedToBoundary)
            {
                continue;
            }
            //Debug.Log("Iteration "+i+"/"+maxFaces+" destroying face "+hmesh.GetFacesRaw()[i]);
            hmesh.GetFacesRaw()[i].Collapse();
            Assert.True(hmesh.IsValid());
            Assert.True(maxFaces - 1 >= hmesh.GetFacesRaw().Count);
            Assert.AreEqual(16, hmesh.GetBoundaryHalfedges().Count);
            //Debug.Log(hmesh.ExportObj());
        }
    }

    [Test]
    public void HMeshCollapseFaceHard()
    {
        // Simplify should fail due to minimum angle
        string triangulatedPlane = "# Blender v2.78 (sub 0) OBJ File: ''\n" +
                                   "# www.blender.org\n" +
                                   "mtllib untitled.mtl\n" +
                                   "o label0.001\n" +
                                   "v 1.000000 0.000000 -1.000000\n" +
                                   "v -1.000000 0.000000 -1.000000\n" +
                                   "v 1.000000 0.000000 1.000000\n" +
                                   "v -1.000000 0.000000 1.000000\n" +
                                   "v 1.000000 0.000000 0.333333\n" +
                                   "v 1.000000 -0.000000 -0.333333\n" +
                                   "v 0.333333 0.000000 -1.000000\n" +
                                   "v -0.333333 0.000000 -1.000000\n" +
                                   "v -1.000000 -0.000000 -0.333333\n" +
                                   "v -1.000000 0.000000 0.333333\n" +
                                   "v -0.333333 0.000000 1.000000\n" +
                                   "v 0.333333 0.000000 1.000000\n" +
                                   "v 0.083413 0.000000 0.535490\n" +
                                   "v -0.070333 -0.000000 0.050065\n" +
                                   "v 0.333333 0.000000 0.033104\n" +
                                   "v -0.126849 -0.000000 -0.333333\n" +
                                   "vn 0.0000 1.0000 0.0000\n" +
                                   "usemtl None\n" +
                                   "s off\n" +
                                   "f 6//1 7//1 16//1\n" +
                                   "f 14//1 2//1 9//1\n" +
                                   "f 16//1 8//1 14//1\n" +
                                   "f 11//1 10//1 4//1\n" +
                                   "f 13//1 9//1 10//1\n" +
                                   "f 12//1 13//1 11//1\n" +
                                   "f 15//1 14//1 13//1\n" +
                                   "f 3//1 15//1 12//1\n" +
                                   "f 5//1 16//1 15//1\n" +
                                   "f 6//1 1//1 7//1\n" +
                                   "f 14//1 8//1 2//1\n" +
                                   "f 16//1 7//1 8//1\n" +
                                   "f 11//1 13//1 10//1\n" +
                                   "f 13//1 14//1 9//1\n" +
                                   "f 12//1 15//1 13//1\n" +
                                   "f 15//1 16//1 14//1\n" +
                                   "f 3//1 5//1 15//1\n" +
                                   "f 5//1 6//1 16//1";
        //Debug.Log(triangulatedPlane);
        int maxFaces = 2; // will be updated with actual max below
        for (int i = 0; i < maxFaces; i++)
        {
            var hmesh = new HMesh();
            hmesh.BuildFromObj(triangulatedPlane);
            maxFaces = hmesh.GetFacesRaw().Count;
            bool isFaceConnectedToBoundary = false;
            foreach (var he in hmesh.GetFacesRaw()[i].Circulate())
            {
                if (he.vert.IsBoundary())
                {
                    isFaceConnectedToBoundary = true;
                }
            }
            if (isFaceConnectedToBoundary)
            {
                continue;
            }
            //Debug.Log("Iteration "+i+"/"+maxFaces+" destroying face "+hmesh.GetFacesRaw()[i]);
            bool res = hmesh.GetFacesRaw()[i].Collapse();
            //Debug.Log("Collapse fase returned "+res );
            Assert.True(hmesh.IsValid());
            foreach (var f in hmesh.GetFaces())
            {
                Assert.True(f.GetNormal()[1] > 0.5f, "Normals should not be flipped");
            }
            //Debug.Log(hmesh.ExportObj());
        }
    }


    [Test]
    public void HMeshCollapseFaceQuad()
    {
        // Simplify should fail due to minimum angle
        string triangulatedPlane = "# Blender v2.78 (sub 0) OBJ File: ''\n" +
                                   "# www.blender.org\n" +
                                   "mtllib untitled.mtl\n" +
                                   "o Plane\n" +
                                   "v -1.000000 0.000000 1.000000\n" +
                                   "v -1.000000 0.000000 0.000000\n" +
                                   "v 0.000000 0.000000 1.000000\n" +
                                   "v 0.000000 0.000000 0.000000\n" +
                                   "v -1.000000 0.000000 0.500000\n" +
                                   "v 0.500000 0.000000 1.000000\n" +
                                   "v -1.000000 0.000000 -0.500000\n" +
                                   "v -0.500000 0.000000 1.000000\n" +
                                   "v 0.000000 0.000000 -0.500000\n" +
                                   "v 0.000000 0.000000 0.500000\n" +
                                   "v -0.500000 0.000000 0.000000\n" +
                                   "v 0.500000 0.000000 0.000000\n" +
                                   "v 0.500000 0.000000 0.500000\n" +
                                   "v -0.500000 0.000000 0.500000\n" +
                                   "v -0.500000 0.000000 -0.500000\n" +
                                   "v 0.500000 0.000000 -0.500000\n" +
                                   "vn 0.0000 1.0000 0.0000\n" +
                                   "usemtl None\n" +
                                   "s off\n" +
                                   "f 14//1 10//1 4//1 11//1\n" +
                                   "f 10//1 13//1 12//1 4//1\n" +
                                   "f 3//1 6//1 13//1 10//1\n" +
                                   "f 5//1 14//1 11//1 2//1\n" +
                                   "f 1//1 8//1 14//1 5//1\n" +
                                   "f 8//1 3//1 10//1 14//1\n" +
                                   "f 2//1 11//1 15//1 7//1\n" +
                                   "f 11//1 4//1 9//1 15//1\n" +
                                   "f 4//1 12//1 16//1 9//1";
        int maxFaces = 2;
        for (int i = 0; i < maxFaces; i++)
        {
            var hmesh = new HMesh();
            hmesh.BuildFromObj(triangulatedPlane);
            maxFaces = hmesh.GetFacesRaw().Count;
            bool isFaceConnectedToBoundary = false;
            foreach (var he in hmesh.GetFacesRaw()[i].Circulate())
            {
                if (he.vert.IsBoundary())
                {
                    isFaceConnectedToBoundary = true;
                }
            }
            if (isFaceConnectedToBoundary)
            {
                continue;
            }
            //Debug.Log("Iteration "+i+"/"+maxFaces+" destroying "+hmesh.GetFacesRaw()[i]);
            hmesh.GetFacesRaw()[i].Collapse();
            Assert.True(hmesh.IsValid());
            Assert.AreEqual(maxFaces - 1, hmesh.GetFacesRaw().Count);
            Assert.AreEqual(12, hmesh.GetBoundaryHalfedges().Count);
        }

    }

    [Test]
    public void HMeshSplitPlane()
    {

        // Simplify should fail due to minimum angle
        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n" +
                         "# www.blender.org\n" +
                         "mtllib test.mtl\n" +
                         "o Plane\n" +
                         "v -1.000000 0.000000 1.000000\n" +
                         "v 1.000000 0.000000 1.000000\n" +
                         "v -1.000000 0.000000 -1.000000\n" +
                         "v 1.000000 0.000000 -1.000000\n" +
                         "v -1.000000 0.000000 0.000000\n" +
                         "v 0.000000 0.000000 1.000000\n" +
                         "v 1.000000 0.000000 0.000000\n" +
                         "v 0.000000 0.000000 -1.000000\n" +
                         "v 0.000000 0.000000 0.000000\n" +
                         "vn 0.0000 1.0000 0.0000\n" +
                         "usemtl None\n" +
                         "s off\n" +
                         "f 7//1 8//1 9//1\n" +
                         "f 9//1 3//1 5//1\n" +
                         "f 6//1 5//1 1//1\n" +
                         "f 2//1 9//1 6//1\n" +
                         "f 7//1 4//1 8//1\n" +
                         "f 9//1 8//1 3//1\n" +
                         "f 6//1 9//1 5//1\n" +
                         "f 2//1 7//1 9//1";
        //Debug.Log(objTest);
        var hmesh = new HMesh();
        hmesh.BuildFromObj(objTest);
        foreach (var f in hmesh.GetFaces())
        {
            if (f.GetCenter().x > 0)
            {
                f.label = 1;
            }
        }

        int vertices = hmesh.GetVerticesRaw().Count;
        int edges = hmesh.GetHalfedgesRaw().Count;
        int faces = hmesh.GetFacesRaw().Count;
        Assert.AreEqual(9, vertices);
        Assert.AreEqual(8 + 8 * 2, edges);
        Assert.AreEqual(8, faces);
        var collapsed = HMeshSimplification.SimplifyByCollapse(hmesh);
        //Debug.Log("Before");
        //Debug.Log(hmesh.ExportObj());
        //var collapsedAdjecent = HMeshSimplification.RemoveAdjecentEdges(hmesh);
        //Debug.Log(hmesh.ExportObj());
        var collapsedUn = HMeshSimplification.DissolveUnneededBoundaryVertices(hmesh);

        //Debug.Log("collapsed "+collapsed);
        //Debug.Log("collapsedAdjecent "+collapsedAdjecent);
        //Debug.Log("collapsedUn "+collapsedUn);
        //Debug.Log(hmesh.ExportObj());
        Assert.AreEqual(true, hmesh.IsValid());
        Assert.AreEqual(6, hmesh.GetVerticesRaw().Count);
        Assert.AreEqual(6 + 3 * 2, hmesh.GetHalfedgesRaw().Count);
        Assert.AreEqual(4, hmesh.GetFacesRaw().Count);
    }

    [Test]
    public void TestSplitNonmanifoldVertices()
    {
        var objTest = "o Plane\n"+
        "v 1.000000 0.000000 1.000000\n"+
        "v -1.000000 0.000000 -1.000000\n"+
        "v 0.000000 0.000000 1.000000\n"+
        "v 0.000000 0.000000 -1.000000\n"+
        "v 0.000000 0.000000 0.000000\n"+
        "vn 0.0000 1.0000 0.0000\n"+
        "f 1//1 5//1 3//1\n"+
        "f 5//1 4//1 2//1\n";
        
        var hmesh = new HMesh();
        hmesh.BuildFromObj(objTest, false);
        
        var res = hmesh.SplitNonManifoldVertices();
        Assert.AreEqual(1, res);
        Assert.AreEqual(true, hmesh.IsValid());
    }

    [Test]
    public void HMeshCollapse() {

        // Quad made of two triangles (splitting edge (-1,1) -> (1,-1)
        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
            "# www.blender.org\n"+
            "mtllib quad.mtl\n"+
            "    o Plane\n"+
            "v -1.000000 0.000000 1.000000\n"+
            "v 1.000000 0.000000 1.000000\n"+
            "v -1.000000 0.000000 -1.000000\n"+
            "v 1.000000 0.000000 -1.000000\n"+
            "vn 0.0000 1.0000 0.0000\n"+
            "usemtl None\n"+
            "s off\n"+
            "f 2//1 3//1 1//1\n"+
            "f 2//1 4//1 3//1\n";

        // collapse (1,1) to (-1,1)
        {
            var hmesh = new HMesh();
            hmesh.BuildFromObj(objTest);
            Debug.Log(hmesh.CreateDebugData());
            foreach (var he in hmesh.GetHalfedges())
            {
                if (he.vert.position.z > 0.9f && he.prev.vert.position.z > 0.9f)
                {
                    Assert.AreEqual( Halfedge.CollapsePreconditionReason.Ok, he.CollapsePrecondition(false, Halfedge.CollapsePreconditionReason.NormalFlipped));
                    he.Collapse(new Vector3D(-1, 0, 1));
                    break;
                }
            }
            Debug.Log( hmesh.CreateDebugData());
            Assert.AreEqual(true, hmesh.IsValid());
            Assert.AreEqual(3, hmesh.GetVerticesRaw().Count);
            Assert.AreEqual(3, hmesh.GetHalfedgesRaw().Count);
            Assert.AreEqual(1, hmesh.GetFacesRaw().Count);
        }
        // collapse (-1,-1) to (1,-1)
        {
            var hmesh = new HMesh();
            hmesh.BuildFromObj(objTest);
            foreach (var he in hmesh.GetHalfedges())
            {
                if (he.vert.position.z < 0.9f && he.prev.vert.position.z < 0.9f)
                {
                    Assert.AreEqual(Halfedge.CollapsePreconditionReason.Ok, he.CollapsePrecondition(false, Halfedge.CollapsePreconditionReason.NormalFlipped));
                    he.Collapse(new Vector3D(1, 0, -1));
                    break;
                }
            }
            Assert.AreEqual(true, hmesh.IsValid());
            Assert.AreEqual(3, hmesh.GetVerticesRaw().Count);
            Assert.AreEqual(3, hmesh.GetHalfedgesRaw().Count);
            Assert.AreEqual(1, hmesh.GetFacesRaw().Count);
        }
        // collapse (-1,1) to (-1,1) (collapse diagonal)
        {
            var hmesh = new HMesh();
            hmesh.BuildFromObj(objTest);
#if HMDebug
            Debug.Log(hmesh.CreateDebugData());
#endif
            foreach (var he in hmesh.GetHalfedges())
            {
                if (he.vert.position.x != he.prev.vert.position.x && he.vert.position.z != he.prev.vert.position.z)
                {
                    Assert.AreNotEqual(Halfedge.CollapsePreconditionReason.Ok, he.CollapsePrecondition());
                    //he.Collapse(new Vector3D(-1, 0, -1));
                    break;
                }
            }
        }
        // collapse (-1,1) to (-1,1) (collapse diagonal)
        {
            var hmesh = new HMesh();
            hmesh.BuildFromObj(objTest);
            foreach (var he in hmesh.GetHalfedges())
            {
                if (he.vert.position.x != he.prev.vert.position.x && he.vert.position.z != he.prev.vert.position.z && he.vert.id < he.prev.vert.id)
                {

                    Assert.AreNotEqual(Halfedge.CollapsePreconditionReason.Ok, he.CollapsePrecondition());
                    //he.Collapse(new Vector3D(1, 0, -1));
                    break;
                }
            }
        }
    }

    [Test]
    public void HMeshCollapseHexagon()
    {
        string v = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
       "# www.blender.org\n"+
       "mtllib hex.mtl\n"+
       "o Circle\n"+
       "v 0.000000 0.000000 -1.000000\n"+
       "v -0.866025 0.000000 -0.500000\n"+
       "v -0.866025 0.000000 0.500000\n"+
       "v 0.000000 0.000000 1.000000\n"+
       "v 0.866025 0.000000 0.500000\n"+
       "v 0.866025 0.000000 -0.500000\n"+
       "v 0.000000 0.000000 0.000000\n"+
       "vn 0.0000 -1.0000 0.0000\n"+
       "usemtl None\n"+
       "s off\n"+
       "f 6//1 5//1 7//1\n"+
       "f 4//1 3//1 7//1\n"+
       "f 2//1 1//1 7//1\n"+
       "f 1//1 6//1 7//1\n"+
       "f 5//1 4//1 7//1\n"+
       "f 3//1 2//1 7//1\n"+
       "";
        var hmesh = new HMesh();
        hmesh.BuildFromObj(v);
        //Debug.Log(hmesh.CreateDebugData());
        Assert.AreEqual(true, hmesh.IsValid());
        for (int i = 0; i < hmesh.GetHalfedgesRaw().Count; i++)
        {
            var hm = new HMesh();
            hm.BuildFromObj(v);
            //Debug.Log("Collapse he "+hm.GetHalfedgesRaw()[i].id+" from "+hm.GetHalfedgesRaw()[i].prev.vert.position+" to "+
            //          hm.GetHalfedgesRaw()[i].vert.position);
            if (hm.GetHalfedgesRaw()[i].CollapsePrecondition() == Halfedge.CollapsePreconditionReason.Ok){
                hm.GetHalfedgesRaw()[i].Collapse(true);
                Assert.AreEqual(true, hm.IsValid());
            }
        }
    }

    [Test]
    public void TestTriangulateBug()
    {
        var mesh = new HMesh();
        var objFile = "# HMesh export - neighbourhood of face 27825\n"+
        "v 322.0073 44.24959 -323.8074\n"+
        "v 327.3816 44.24959 -332.5917\n"+
        "v 327.4597 44.24959 -333.0756\n"+
        "v 327.4791 44.22952 -332.7493\n"+
        "v 327.3811 44.24959 -332.5887\n"+
        "v 321.5249 44.24959 -322.9873\n"+
        "v 321.5201 44.24959 -323.0111\n"+
        "v 258.3253 44.2496 -389.9377\n"+
        "v 324.0491 44.24959 -311.9405\n"+
        "v 322.6757 44.24958 -303.43\n"+
        "v 258.3236 44.2496 -386.8879\n"+
        "v 258.3241 44.2496 -387.8611\n"+
        "v 258.3242 44.2496 -387.9865\n"+
        "v 258.3245 44.2496 -388.6248\n"+
        "v 258.3246 44.2496 -388.752\n"+
        "v 258.3248 44.2496 -389.1623\n"+
        "v 258.3249 44.2496 -389.241\n"+
        "v 258.3249 44.2496 -389.3425\n"+
        "v 258.3253 44.2496 -389.9315\n"+
        "v 325.317 43.64647 -306.3917\n"+
        "v 328.8097 42.84892 -310.3083\n"+
        "f 1 2 3 \n"+
        "f 2 4 3 \n"+
        "f 4 2 5 \n"+
        "f 2 1 6 5 \n"+
        "f 1 7 6 \n"+
        "f 7 1 3 \n"+
        "f 8 6 7 \n"+
        "f 6 9 5 \n"+
        "f 9 6 10 \n"+
        "f 11 10 6 \n"+
        "f 12 11 6 \n"+
        "f 13 12 6 \n"+
        "f 14 13 6 \n"+
        "f 15 14 6 \n"+
        "f 16 15 6 \n"+
        "f 17 16 6 \n"+
        "f 18 17 6 \n"+
        "f 19 18 6 \n"+
        "f 6 8 19 \n"+
        "f 9 20 21 4 5 ";
        mesh.BuildFromObj(objFile, false);
        var face = mesh.GetFacesRaw()[3];
        Assert.AreEqual(4,face.Circulate().Count);
        var res = face.Triangulate();
        Assert.AreEqual(1,res.Count);
        Assert.AreEqual(true, mesh.IsValid());
    }

    [Test]
    public void TestCutFace()
    {
        
        for (int i = 0; i < 4; i++)
        {
            // Forces cut to be consistent
            var quad = HMesh.CreateTestMeshQuad();
            var faces = quad.GetFacesRaw();
            var hes = faces[0].Circulate();
            var middleVertex = hes[(i + 1) % 4].vert;
            var newFace = faces[0].Cut(hes[i].vert, hes[(i + 2) % 4].vert);
            Assert.AreEqual(3, newFace.Circulate().Count);
            Assert.AreEqual(3, faces[0].Circulate().Count);
            bool foundVertex = false;
            foreach (var he in newFace.Circulate())
            {
                if (he.vert == middleVertex)
                {
                    foundVertex = true;
                    break;
                }
            }
            Assert.True(foundVertex); 
        }
    }

    [Test]
    public void HMeshCollapseGrid()
    {
        string v = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
        "# www.blender.org\n"+
        "mtllib plane.mtl\n"+
        "    o Grid\n"+
        "v -1.000000 0.000000 1.000000\n"+
        "v 1.000000 0.000000 1.000000\n"+
        "v -1.000000 0.000000 -1.000000\n"+
        "v 1.000000 0.000000 -1.000000\n"+
        "v -1.000000 0.000000 -0.333333\n"+
        "v -1.000000 0.000000 0.333333\n"+
        "v -0.333333 0.000000 1.000000\n"+
        "v 0.333333 0.000000 1.000000\n"+
        "v 1.000000 0.000000 0.333333\n"+
        "v 1.000000 0.000000 -0.333333\n"+
        "v 0.333333 0.000000 -1.000000\n"+
        "v -0.333333 0.000000 -1.000000\n"+
        "v -0.333333 0.000000 0.333333\n"+
        "v -0.333333 0.000000 -0.333333\n"+
        "v 0.333333 0.000000 0.333333\n"+
        "v 0.333333 0.000000 -0.333333\n"+
        "vn 0.0000 1.0000 0.0000\n"+
        "usemtl None\n"+
        "s off\n"+
        "f 10//1 11//1 16//1\n"+
        "f 14//1 3//1 5//1\n"+
        "f 16//1 12//1 14//1\n"+
        "f 7//1 6//1 1//1\n"+
        "f 13//1 5//1 6//1\n"+
        "f 8//1 13//1 7//1\n"+
        "f 15//1 14//1 13//1\n"+
        "f 2//1 15//1 8//1\n"+
        "f 9//1 16//1 15//1\n"+
        "f 10//1 4//1 11//1\n"+
        "f 14//1 12//1 3//1\n"+
        "f 16//1 11//1 12//1\n"+
        "f 7//1 13//1 6//1\n"+
        "f 13//1 14//1 5//1\n"+
        "f 8//1 15//1 13//1\n"+
        "f 15//1 16//1 14//1\n"+
        "f 2//1 9//1 15//1\n"+
        "f 9//1 10//1 16//1\n"+
        "";
        Debug.Log(v);
        var hmesh = new HMesh();
        hmesh.BuildFromObj(v);
        Assert.AreEqual(true, hmesh.IsValid());
        for (int i = 0; i < hmesh.GetHalfedgesRaw().Count; i++)
        {
            var hm = new HMesh();
            hm.BuildFromObj(v);
            if (hm.GetHalfedgesRaw()[i].CollapsePrecondition() == Halfedge.CollapsePreconditionReason.Ok){
                
                Debug.Log(hm.CreateDebugData());
                Debug.Log("Collapse "+hm.GetHalfedgesRaw()[i]);
                hm.GetHalfedgesRaw()[i].Collapse(true);
                
                Debug.Log(hm.CreateDebugData());
                
                Assert.AreEqual(true, hm.IsValid());
            }
        }
    }

    [Test]
    public static void HMeshDegenerateFace()
    {
        var s = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
        "# www.blender.org\n"+
        "mtllib untitled.mtl\n"+
        "    o degenerateFace\n"+
        "v -1.000000 0.000000 1.000000\n"+
        "v 1.000000 0.000000 1.000000\n"+
        "v -1.000000 0.000000 -1.000000\n"+
        "v 1.000000 0.000000 -1.000000\n"+
        "v 0.000000 0.000000 0.000000\n"+
        "vn 0.0000 1.0000 0.0000\n"+
        "vn 0.0000 0.0000 1.0000\n"+
        "usemtl None\n"+
        "s off\n"+
        "f 1//1 2//1 3//1\n"+
        "f 5//1 4//1 3//1\n"+
        "f 3//2 2//2 5//2\n"+
        "f 2//1 4//1 5//1\n"+
        "";
        var hmesh = new HMesh();
        hmesh.BuildFromObj(s);
        int res = HMeshSimplification.FixDegenerateFaces(hmesh);
        Assert.AreEqual(1, res);
        Assert.AreEqual(4, hmesh.GetFacesRaw().Count);
    }

    [Test]
    public static void HMeshDissolveEdgesOfZero()
    {
        var s = Resources.Load<TextAsset>("hmeshbug").text;
        var hmesh = new HMesh();
        hmesh.BuildFromObj(s);
        int res = HMeshSimplification.FixDegenerateFaces(hmesh);
        Debug.Log("Collapsing "+res);
        Assert.AreEqual(true, hmesh.IsValid());
    }
    
    [Test]
    public static void HMeshFixDegenerateFacesBug()
    {
        var s = Resources.Load<TextAsset>("hmeshbug3").text;
        var hmesh = new HMesh();
        hmesh.BuildFromObj(s);
        int res;
        do
        {
            res = HMeshSimplification.FixDegenerateFaces(hmesh);
            Debug.Log("Fixes "+res);
        } 
        while (res > 0);
        
        Assert.AreEqual(true, hmesh.IsValid(HMeshValidationRules.All));
    }

    [Test]
    public static void HMeshTestDegenerate()
    {
        var s = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
        "# www.blender.org\n"+
        "mtllib test.mtl\n"+
        "o Plane\n"+
        "v -1.000000 0.000000 1.000000\n"+
        "v 1.000000 0.000000 1.000000\n"+
        "v -1.000000 0.000000 -1.000000\n"+
        "v 1.000000 0.000000 -1.000000\n"+
        "v -1.000000 0.000000 -0.333333\n"+
        "v -1.000000 0.000000 0.333333\n"+
        "v -0.333333 0.000000 1.000000\n"+
        "v 0.333333 0.000000 1.000000\n"+
        "v 1.000000 0.000000 0.333333\n"+
        "v 1.000000 0.000000 -0.333333\n"+
        "v 0.333333 0.000000 -1.000000\n"+
        "v -0.333333 0.000000 -1.000000\n"+
        "v -0.333333 0.000000 0.000000\n"+
        "v -0.333333 0.000000 0.000000\n"+
        "v 0.333333 0.000000 0.000000\n"+
        "v 0.333333 0.000000 0.000000\n"+
        "vn 0.0000 1.0000 0.0000\n"+
        "vn 0.0000 0.0000 1.0000\n"+
        "usemtl None\n"+
        "s off\n"+
        "f 16//1 10//1 4//1 11//1\n"+
        "f 5//1 14//1 12//1 3//1\n"+
        "f 14//1 16//1 11//1 12//1\n"+
        "f 1//1 7//1 13//1 6//1\n"+
        "f 6//1 13//1 14//1 5//1\n"+
        "f 7//1 8//1 15//1 13//1\n"+
        "f 13//2 15//2 16//2 14//2\n"+
        "f 8//1 2//1 9//1 15//1\n"+
        "f 15//1 9//1 10//1 16//1";
        
        Debug.Log(s);

        for (int i = 0; i < 9; i++)
        {
            var hmesh = new HMesh();
            hmesh.BuildFromObj(s);
            
            var face = hmesh.GetFacesRaw()[i];
            bool hasZeroLengthEdge = false;
            foreach (var he in face.Circulate())
            {
                if (he.GetDirection().sqrMagnitude == 0)
                {
                    hasZeroLengthEdge = true;
                }
            }
            
            if (hasZeroLengthEdge)
            {
                // Debug.Log("Collapsing "+face);
                face.Collapse();
                HMeshSimplification.FixDegenerateFaces(hmesh);
                Assert.AreEqual(true, hmesh.IsValid());
                // Debug.Log(hmesh.ExportObj());
            }
        }
    }

    [Test]
    public static void HMeshDissolveUnneededVertices()
    {
        var s = Resources.Load<TextAsset>("hmeshbug2").text;
        var hmesh = new HMesh();
        hmesh.BuildFromObj(s);
        
        //Assert.AreEqual(true, hmesh.IsValid());
        int res = HMeshSimplification.DissolveUnneededVertices(hmesh);
        //Assert.AreEqual(true, hmesh.IsValid());
        HMeshSimplification.FixDegenerateFaces(hmesh);
        Debug.Log("Dissolving "+res);
        Assert.AreEqual(true, hmesh.IsValid());
    }

    [Test]
    public static void TestLeftOfXZ()
    {
        Vector3D p1 = new Vector3D(0,0,0);
        Vector3D p2 = new Vector3D(1,0,1);
        Vector3D p3 = new Vector3D(0,0,1);
        Vector3D p4 = new Vector3D(1,0,0);
        Assert.True(HMeshMath.LeftOfXZ(p1,p2,p3));
        Assert.False(HMeshMath.LeftOfXZ(p1,p2,p4));
    }


    [Test]
    public static void TestLeftOfXY()
    {
        Vector3D p1 = new Vector3D(0,0,0);
        Vector3D p2 = new Vector3D(1,1,0);
        Vector3D p3 = new Vector3D(0,1,0);
        Vector3D p4 = new Vector3D(1,0,0);
        Assert.True(HMeshMath.LeftOfXY(p1,p2,p3));
        Assert.False(HMeshMath.LeftOfXY(p1,p2,p4));
    }

    [Test]
    public static void TestInCircleXY()
    {
        Vector3 point = new Vector3(1,0,0);//Random.insideUnitCircle;
        Vector3 point2 = Quaternion.Euler(0,0,-45) * point;
        Vector3 point3 = Quaternion.Euler(0,0,-90) * point;

        // Debug.Log(point+" "+point2+" "+point3);

        Assert.True(HMeshMath.InCircleXY(new Vector3D( point), new Vector3D( point2), new Vector3D( point3), new Vector3D( point*0.5f)),"Expected in circle");
        Assert.False(HMeshMath.InCircleXY(new Vector3D( point), new Vector3D( point2), new Vector3D( point3), new Vector3D( point*1.5f)),"Expected out of circle");
    }

    [Test]
    public static void TestTriangulate()
    {
        Vector3D[] vertices = new []
        {
            new Vector3D(-38.344144032700221, 40.49967335708476, -32.091330622572841),
            new Vector3D(-30.024999618530273, 40.499664306640625, -13.06254768371582),
            new Vector3D(-30.024999618530273, 40.499664306640625, -14.835947036743164),
            new Vector3D(-37.681685539510369, 40.499672636392425, -32.091297499972924),
            new Vector3D(-37.913331495731335, 40.499672888401371, -32.091309082157181)
        };

        for (int ii=0;ii<vertices.Length;ii++){
            var hMesh = HMesh.CreateTestMeshNGon(vertices.Length);
            for (int i = 0; i < hMesh.GetVerticesRaw().Count; i++)
            {
                hMesh.GetVerticesRaw()[i].positionD = vertices[i];
                hMesh.GetVerticesRaw()[i].label = i;
            }

            hMesh.GetFacesRaw()[0].halfedge = hMesh.GetHalfedgesRaw()[ii];
            Assert.True(hMesh.IsValid());
            var res = hMesh.GetFacesRaw()[0].Triangulate();

            foreach (var f in res)
            {
                Assert.True(f.IsValid(), "Fail at iteration "+ii);
            }

        }
    }

    [Test]
    public static void TestFixDegenerateBug2()
    {
        Vector3D[] vertices = new []
        {
            new Vector3D(-33.330135345458984, 44.249599456787109, -396.83026123046875),
            new Vector3D(-43.652000427246094, 44.249599456787109, -396.83026123046875),
            new Vector3D(-57.384712219238281, 44.249599456787109, -396.83026123046875)
        };

        for (int ii=0;ii<vertices.Length;ii++){
            var hMesh = HMesh.CreateTestMeshNGon(vertices.Length);
            for (int i = 0; i < hMesh.GetVerticesRaw().Count; i++)
            {
                hMesh.GetVerticesRaw()[i].positionD = vertices[i];
                hMesh.GetVerticesRaw()[i].label = i;
            }
            // assign all halfedges to the face
            hMesh.GetFacesRaw()[0].halfedge = hMesh.GetHalfedgesRaw()[ii];

            Debug.Log(hMesh.CreateDebugData());
            
            // this will potentially dissolve the full mesh
            HMeshSimplification.FixDegenerateFaces(hMesh);
            Debug.Log(hMesh.CreateDebugData());
            Assert.True(hMesh.IsValid(), "Fail at iteration "+ii);
        }
    }

    [Test]
    public static void TestCollapseBug()
    {
        string obj = "v -69.280288696289063 40.499721527099609 -133.36997985839844\n"+
        "v -30.024999618530273 40.49969482421875 -76.7983169555664\n"+
        "v -94.633369445800781 40.499729156494141 -147.48594665527344\n"+
        "v -113.95390319824219 40.499652862548828 15.860174179077148\n"+
        "v -113.95390319824219 40.499759674072266 -214.11170959472656\n"+
        "v -94.633369445800781 40.499729156494141 -155.20808410644531\n"+
        "v -55.5473518371582 40.499729156494141 -147.48594665527344\n"+
        "v -6.5042881965637207 40.49969482421875 -76.808891296386719\n"+
        "v -6.5042881965637207 40.499652862548828 11.657781600952148\n"+
        "v -31.79998779296875 40.499672752628669 -32.09100341796875\n"+
        "v -31.79998779296875 40.499674467708722 -35.573272705078125\n"+
        "v -100.94741821289063 40.499675791666228 -35.570159912109375\n"+
        "v -94.05645751953125 40.499674023674281 -32.0941162109375\n"+
        "v -41.339879950075641 40.499673484924969 -32.091480407900235\n"+
        "v -44.480755386110431 40.4996729997474 -32.091637450132382\n"+
        "v -40.786950038852424 40.49967348491186 -32.091452761675711\n"+
        "v -40.63713075101775 40.499673357139287 -32.09144527078476\n"+
        "v -38.344144032700221 40.49967335708476 -32.091330622572841\n"+
        "v -37.681685539510369 40.499672636392425 -32.091297499972924\n"+
        "v -35.739476232426071 40.499672636345551 -32.091200390459619\n"+
        "v -92.613695465837026 40.499673993875753 -32.094044073542044\n"+
        "v -41.546365233083158 40.499673453028755 -32.091490732063164\n"+
        "v -41.115560233932342 40.499673484919647 -32.091469192024384\n"+
        "v -37.913331495731335 40.499672888401371 -32.091309082157181\n"+
        "v -36.109853397476186 40.499672636354489 -32.091218909136316\n"+
        "v -42.723284917390927 40.499675136304589 -35.572780973682207\n"+
        "v -43.375664028163236 40.499675136290655 -35.57275160569128\n"+
        "v -42.578623154067856 40.499675012930588 -35.572787485885435\n"+
        "v -39.866246539104495 40.499675012988661 -35.572909588280339\n"+
        "v -39.226587846344621 40.499674317100357 -35.572938383639318\n"+
        "v -36.892569168799213 40.499674317151083 -35.573043453587459\n"+
        "v -33.088649379123844 40.499674429610593 -35.573214693714576\n"+
        "v -46.4084177876856 40.499674667652144 -35.57261508095678\n"+
        "v -39.517490538287589 40.49967463357504 -35.572925288143956\n"+
        "v -36.723267677788051 40.499674322156331 -35.573051074991284\n"+
        "o label0\n"+
        "f 4 3 5\n"+
        "f 12 4 13\n"+
        "f 9 22 15\n"+
        "f 26 28 3\n"+
        "f 3 34 30\n"+
        "f 30 1 3\n"+
        "f 8 11 10\n"+
        "f 5 3 6\n"+
        "f 1 7 3\n"+
        "f 2 7 1\n"+
        "f 2 8 7\n"+
        "f 11 8 2\n"+
        "f 1 35 32\n"+
        "f 8 10 9\n"+
        "f 32 2 1\n"+
        "f 2 32 11\n"+
        "f 15 21 9\n"+
        "f 3 4 12\n"+
        "f 21 4 9\n"+
        "f 4 21 13\n"+
        "f 9 23 14\n"+
        "f 18 17 9\n"+
        "f 9 24 18\n"+
        "f 9 25 19\n"+
        "f 10 20 9\n"+
        "f 33 27 3\n"+
        "f 12 33 3\n"+
        "f 22 9 14\n"+
        "f 3 28 29\n"+
        "f 34 3 29\n"+
        "f 1 30 31\n"+
        "f 35 1 31\n"+
        "f 23 9 16\n"+
        "f 16 9 17\n"+
        "f 24 9 19\n"+
        "f 25 9 20\n"+
        "f 3 27 26\n"+
        "o label1\n"+
        "f 23 26 27\n"+
        "f 27 22 14\n"+
        "f 21 33 12\n"+
        "f 13 21 12\n"+
        "f 28 16 17\n"+
        "f 17 29 28\n"+
        "f 34 24 19\n"+
        "f 25 31 30\n"+
        "f 35 20 10\n"+
        "f 11 32 10\n"+
        "f 23 27 14\n"+
        "f 26 23 16\n"+
        "f 22 27 33\n"+
        "f 22 33 15\n"+
        "f 33 21 15\n"+
        "f 16 28 26\n"+
        "f 29 17 18\n"+
        "f 34 19 30\n"+
        "f 24 34 29\n"+
        "f 24 29 18\n"+
        "f 25 30 19\n"+
        "f 31 25 20\n"+
        "f 35 10 32\n"+
        "f 20 35 31";

        var hmesh = new HMesh();
        hmesh.BuildFromObj(obj);
        //Debug.Log(hmesh.GetVerticesRaw()[0]);
        //Debug.Log(hmesh.GetVerticesRaw()[31]);
        //Debug.Log(hmesh.IsValid());
        var edge = hmesh.GetVerticesRaw()[0].GetSharedEdge(hmesh.GetVerticesRaw()[31]);
        //Debug.Log(edge+" collapse to "+edge.vert.positionD);
        Assert.False(edge.CollapsePrecondition(edge.vert.positionD) == Halfedge.CollapsePreconditionReason.Ok,"Precondition "+edge+" collapse to "+edge.vert.positionD+" was "+edge.CollapsePrecondition(edge.vert.positionD));
        Assert.True(hmesh.IsValid(),"Valid after collapse");
    }

    [Test]
    public static void TestFixDegenerateBug()
    {
        string obj = "v -18.185237884521484 40.499668121337891 -92.0524673461914\n"+
        "v -20.894014358520508 40.499668121337891 -92.0524673461914\n"+
        "v -20.894016265869141 40.499668121337891 -87.088592529296875\n"+
        "v -18.185239791870117 40.499668121337891 -87.088584899902344\n"+
        "v -101.00544738769531 40.499687194824219 -202.5389404296875\n"+
        "v -20.894016265869141 40.499656677246094 -21.997270584106445\n"+
        "v -101.00544738769531 40.499656677246094 -18.463706970214844\n"+
        "v -42.247764587402344 40.499656677246094 -18.433792114257813\n"+
        "v -32.24420166015625 40.499656677246094 -18.428699493408203\n"+
        "v -32.24420166015625 40.499649047851563 26.254707336425781\n"+
        "v -20.894016265869141 40.499649047851563 26.254707336425781\n"+
        "v -20.894016265869141 40.499656677246094 -18.422920227050781\n"+
        "v 25.29876708984375 40.499649047851563 26.254707336425781\n"+
        "v 20.834335327148438 40.499656677246094 -18.395105361938477\n"+
        "v 25.29876708984375 40.499656677246094 -18.392127990722656\n"+
        "v -42.247764587402344 40.499649047851563 26.254707336425781\n"+
        "v 20.834316253662109 40.629661560058594 -21.605915069580078\n"+
        "v -20.894012451171875 40.499691009521484 -206.95294189453125\n"+
        "v -100.87776947021484 40.499691009521484 -209.03173828125\n"+
        "v -33.280918121337891 40.499691009521484 -209.03173828125\n"+
        "v 32.910903930664063 40.499649047851563 26.254707336425781\n"+
        "v 37.805862426757813 40.499649047851563 26.254707336425781\n"+
        "v 33.1491813659668 40.499660491943359 -18.389570236206055\n"+
        "v 37.237030029296875 40.499656677246094 -18.388236999511719\n"+
        "v 196.92129516601563 40.629661560058594 -21.530467987060547\n"+
        "v 96.293502807617188 40.499656677246094 -18.282730102539063\n"+
        "v 109.17059326171875 40.499656677246094 -18.278514862060547\n"+
        "v 109.17059326171875 40.499649047851563 20.790802001953125\n"+
        "v 108.73865509033203 40.499649047851563 20.790802001953125\n"+
        "v 96.293502807617188 40.499649047851563 20.790802001953125\n"+
        "v 196.64863586425781 40.499649047851563 26.254707336425781\n"+
        "v 196.64863586425781 40.499656677246094 -18.251129150390625\n"+
        "v 212.77580261230469 40.499656677246094 -18.245038986206055\n"+
        "v 212.87248229980469 40.629661560058594 -21.530467987060547\n"+
        "v 196.92129516601563 44.249546051025391 -74.495086669921875\n"+
        "v 212.87248229980469 44.249546051025391 -74.495086669921875\n"+
        "v 213.61079406738281 41.017734527587891 -28.916828155517578\n"+
        "v 213.61079406738281 40.867378234863281 -21.482036590576172\n"+
        "v 220.81080627441406 41.4170036315918 -28.916828155517578\n"+
        "v 220.81080627441406 40.959980010986328 -21.482036590576172\n"+
        "v 224.41070556640625 41.662944793701172 -28.916826248168945\n"+
        "v 224.41070556640625 42.978801727294922 -36.351608276367188\n"+
        "v 220.81080627441406 42.763545989990234 -36.351608276367188\n"+
        "v 224.41070556640625 43.863971710205078 -43.786396026611328\n"+
        "v 220.81080627441406 43.413734436035156 -43.786399841308594\n"+
        "v 224.41070556640625 44.179615020751953 -51.221183776855469\n"+
        "v 220.81080627441406 43.914577484130859 -51.221183776855469\n"+
        "v 224.41070556640625 44.530097961425781 -58.655975341796875\n"+
        "v 220.81080627441406 44.361652374267578 -58.655975341796875\n"+
        "v 224.41070556640625 44.800930023193359 -66.09075927734375\n"+
        "v 220.81080627441406 44.7001838684082 -66.09075927734375\n"+
        "v 224.26344299316406 44.2288703918457 -71.365707397460938\n"+
        "v 220.81080627441406 44.685256958007813 -71.316619873046875\n"+
        "v 226.64849853515625 44.249546051025391 -74.495086669921875\n"+
        "v 226.64981079101563 44.249542236328125 -38.9974365234375\n"+
        "v 231.61070251464844 42.837867736816406 -36.351608276367188\n"+
        "v 231.61070251464844 42.022392272949219 -28.916824340820313\n"+
        "v 238.81059265136719 42.926486968994141 -36.351608276367188\n"+
        "v 238.81059265136719 42.035335540771484 -28.916824340820313\n"+
        "v 231.61070251464844 41.246738433837891 -21.482034683227539\n"+
        "v 238.81059265136719 41.326400756835938 -21.482034683227539\n"+
        "v 246.010498046875 42.113002777099609 -28.916824340820313\n"+
        "v 246.010498046875 41.509609222412109 -21.482034683227539\n"+
        "v 246.010498046875 43.102725982666016 -36.351604461669922\n"+
        "v 224.41070556640625 41.091411590576172 -21.482036590576172\n"+
        "v 196.920166015625 44.249553680419922 -105.38189697265625\n"+
        "v 196.43194580078125 44.249553680419922 -105.38189697265625\n"+
        "v 196.43307495117188 44.249546051025391 -74.495086669921875\n"+
        "v 102.65193939208984 44.249553680419922 -105.38189697265625\n"+
        "v 102.653076171875 44.249546051025391 -74.495086669921875\n"+
        "v 196.922607421875 44.249542236328125 -38.9974365234375\n"+
        "v 196.43438720703125 44.249542236328125 -38.9974365234375\n"+
        "v 196.92324829101563 44.249538421630859 -21.610836029052734\n"+
        "v 196.43502807617188 44.249538421630859 -21.610836029052734\n"+
        "v 102.65438842773438 44.249542236328125 -38.9974365234375\n"+
        "v 102.655029296875 44.249538421630859 -21.610836029052734\n"+
        "v 102.52647399902344 44.249546051025391 -74.495086669921875\n"+
        "v 102.52778625488281 44.249542236328125 -38.9974365234375\n"+
        "v 33.590412139892578 44.249553680419922 -105.38189697265625\n"+
        "v 102.52842712402344 44.249538421630859 -21.610836029052734\n"+
        "v 33.593509674072266 44.249538421630859 -21.610836029052734\n"+
        "v 102.52533721923828 44.249553680419922 -105.38189697265625\n"+
        "v 196.43502807617188 44.249538421630859 -21.610836029052734\n"+
        "v 196.92129516601563 40.629661560058594 -21.530467987060547\n"+
        "v 33.593509674072266 44.249538421630859 -21.610836029052734\n"+
        "v 33.1451301574707 40.629661560058594 -21.605915069580078\n"+
        "v 196.92324829101563 44.249538421630859 -21.610836029052734\n"+
        "v 21.166902542114258 44.249553680419922 -105.38189697265625\n"+
        "v 33.1451301574707 40.629661560058594 -21.605915069580078\n"+
        "v 33.1451301574707 44.249553680419922 -105.38189697265625\n"+
        "v 25.29876708984375 40.499656677246094 -18.392127990722656\n"+
        "v 33.1491813659668 40.499660491943359 -18.389570236206055\n"+
        "v 33.1451301574707 40.629661560058594 -21.605915069580078\n"+
        "v 20.834335327148438 40.499656677246094 -18.395105361938477\n"+
        "v 20.834316253662109 40.629661560058594 -21.605915069580078\n"+
        "v -18.154218673706055 44.249549865722656 -86.594429016113281\n"+
        "v -20.486660003662109 44.249549865722656 -86.594429016113281\n"+
        "v -20.484245300292969 44.249538421630859 -21.610836029052734\n"+
        "v -18.151803970336914 44.249538421630859 -21.610836029052734\n"+
        "v 21.027727127075195 44.249549865722656 -86.594429016113281\n"+
        "v 21.030143737792969 44.249538421630859 -21.610836029052734\n"+
        "v 21.027046203613281 44.249553680419922 -105.38189697265625\n"+
        "v -18.154897689819336 44.249553680419922 -106.18202972412109\n"+
        "v -20.599987030029297 44.249568939208984 -208.31097412109375\n"+
        "v -20.886323928833008 44.249553680419922 -106.18202972412109\n"+
        "v -18.158700942993164 44.249568939208984 -208.31097412109375\n"+
        "v -20.600955963134766 44.249568939208984 -209.16400146484375\n"+
        "v -18.159669876098633 44.249568939208984 -209.16400146484375\n"+
        "v -20.601131439208984 44.249568939208984 -212.32542419433594\n"+
        "v -18.159845352172852 44.249568939208984 -212.32542419433594\n"+
        "v 21.027727127075195 44.249549865722656 -86.594429016113281\n"+
        "v 21.166902542114258 44.249553680419922 -105.38189697265625\n"+
        "v -60.450511932373047 44.249568939208984 -212.32542419433594\n"+
        "v -57.406364440917969 44.249568939208984 -209.16400146484375\n"+
        "v -33.374298095703125 44.249568939208984 -209.16400146484375\n"+
        "v -33.374473571777344 44.249568939208984 -212.32542419433594\n"+
        "v 226.64736938476563 44.249553680419922 -105.38189697265625\n"+
        "v -20.894014358520508 40.499668121337891 -92.0524673461914\n"+
        "v -18.185237884521484 40.499668121337891 -92.0524673461914\n"+
        "v -18.154897689819336 44.249553680419922 -106.18202972412109\n"+
        "v -20.886323928833008 44.249553680419922 -106.18202972412109\n"+
        "v -20.599987030029297 44.249568939208984 -208.31097412109375\n"+
        "v -20.894012451171875 40.499691009521484 -206.95294189453125\n"+
        "v -20.600955963134766 44.249568939208984 -209.16400146484375\n"+
        "v -20.601131439208984 44.249568939208984 -212.32542419433594\n"+
        "v 213.1065673828125 40.499649047851563 26.254707336425781\n"+
        "v 43.54547119140625 40.499649047851563 26.254707336425781\n"+
        "v -100.87776947021484 40.499691009521484 -209.03173828125\n"+
        "v -33.280918121337891 40.499691009521484 -209.03173828125\n"+
        "v -33.374298095703125 44.249568939208984 -209.16400146484375\n"+
        "v -57.406364440917969 44.249568939208984 -209.16400146484375\n"+
        "v -80.0669174194336 44.249568939208984 -209.16400146484375\n"+
        "v -100.74127960205078 44.249568939208984 -209.16400146484375\n"+
        "v -18.185237884521484 40.499668121337891 -92.0524673461914\n"+
        "v -18.185239791870117 40.499668121337891 -87.088584899902344\n"+
        "v -18.154218673706055 44.249549865722656 -86.594429016113281\n"+
        "v -18.154897689819336 44.249553680419922 -106.18202972412109\n"+
        "v -20.894016265869141 40.499668121337891 -87.088592529296875\n"+
        "v -20.486660003662109 44.249549865722656 -86.594429016113281\n"+
        "v -18.154218673706055 44.249549865722656 -86.594429016113281\n"+
        "v -18.185239791870117 40.499668121337891 -87.088584899902344\n"+
        "v -20.484245300292969 44.249538421630859 -21.610836029052734\n"+
        "v -20.486660003662109 44.249549865722656 -86.594429016113281\n"+
        "v -20.894016265869141 40.499668121337891 -87.088592529296875\n"+
        "v -20.894016265869141 40.499656677246094 -21.997270584106445\n"+
        "v 20.834316253662109 40.629661560058594 -21.605915069580078\n"+
        "v 21.030143737792969 44.249538421630859 -21.610836029052734\n"+
        "v -18.151803970336914 44.249538421630859 -21.610836029052734\n"+
        "v -18.151803970336914 44.249538421630859 -21.610836029052734\n"+
        "v -20.894016265869141 40.499656677246094 -21.997270584106445\n"+
        "v 20.834316253662109 40.629661560058594 -21.605915069580078\n"+
        "v -20.484245300292969 44.249538421630859 -21.610836029052734\n"+
        "v 20.834316253662109 40.629661560058594 -21.605915069580078\n"+
        "v 21.166902542114258 44.249553680419922 -105.38189697265625\n"+
        "v 21.027727127075195 44.249549865722656 -86.594429016113281\n"+
        "v 21.030143737792969 44.249538421630859 -21.610836029052734\n"+
        "v 33.1451301574707 40.629661560058594 -21.605915069580078\n"+
        "v 33.593509674072266 44.249538421630859 -21.610836029052734\n"+
        "v 33.590412139892578 44.249553680419922 -105.38189697265625\n"+
        "v 33.1451301574707 44.249553680419922 -105.38189697265625\n"+
        "v 196.922607421875 44.249542236328125 -38.9974365234375\n"+
        "v 196.92324829101563 44.249538421630859 -21.610836029052734\n"+
        "v 196.92129516601563 40.629661560058594 -21.530467987060547\n"+
        "v 196.92129516601563 44.249546051025391 -74.495086669921875\n"+
        "v 20.834316253662109 40.629661560058594 -21.605915069580078\n"+
        "v 33.1451301574707 40.629661560058594 -21.605915069580078\n"+
        "v 21.027046203613281 44.249553680419922 -105.38189697265625\n"+
        "v -18.154897689819336 44.249553680419922 -106.18202972412109\n"+
        "o label0\n"+
        "f 1 2 3\n"+
        "f 1 3 4\n"+
        "f 3 2 5\n"+
        "f 6 3 5\n"+
        "f 6 5 7\n"+
        "f 6 7 8\n"+
        "f 6 8 9\n"+
        "f 8 10 9\n"+
        "f 9 10 11\n"+
        "f 9 11 12\n"+
        "f 9 12 6\n"+
        "f 12 11 13\n"+
        "f 13 14 12\n"+
        "f 14 6 12\n"+
        "f 13 15 14\n"+
        "f 8 16 10\n"+
        "f 14 165 6\n"+
        "f 2 18 5\n"+
        "f 18 19 5\n"+
        "f 18 20 19\n"+
        "f 15 13 21\n"+
        "f 21 22 15\n"+
        "f 22 23 15\n"+
        "f 22 24 23\n"+
        "f 25 23 24\n"+
        "f 25 24 26\n"+
        "f 25 26 27\n"+
        "f 28 27 26\n"+
        "f 29 28 26\n"+
        "f 26 30 29\n"+
        "f 30 26 24\n"+
        "f 27 28 31\n"+
        "f 27 31 32\n"+
        "f 27 32 25\n"+
        "f 25 32 33\n"+
        "f 25 33 34\n"+
        "f 35 25 34\n"+
        "f 35 34 36\n"+
        "f 37 36 34\n"+
        "f 34 38 37\n"+
        "f 39 37 38\n"+
        "f 37 39 36\n"+
        "f 39 38 40\n"+
        "f 41 39 40\n"+
        "f 42 39 41\n"+
        "f 42 43 39\n"+
        "f 39 43 36\n"+
        "f 44 43 42\n"+
        "f 44 45 43\n"+
        "f 43 45 36\n"+
        "f 46 45 44\n"+
        "f 46 47 45\n"+
        "f 45 47 36\n"+
        "f 48 47 46\n"+
        "f 48 49 47\n"+
        "f 47 49 36\n"+
        "f 50 49 48\n"+
        "f 50 51 49\n"+
        "f 49 51 36\n"+
        "f 52 51 50\n"+
        "f 52 53 51\n"+
        "f 53 52 54\n"+
        "f 50 54 52\n"+
        "f 48 54 50\n"+
        "f 53 54 36\n"+
        "f 51 53 36\n"+
        "f 48 55 54\n"+
        "f 55 44 42\n"+
        "f 44 55 46\n"+
        "f 55 48 46\n"+
        "f 56 42 41\n"+
        "f 56 41 57\n"+
        "f 58 56 57\n"+
        "f 58 55 56\n"+
        "f 58 57 59\n"+
        "f 59 57 60\n"+
        "f 59 60 61\n"+
        "f 62 59 61\n"+
        "f 62 61 63\n"+
        "f 64 58 59\n"+
        "f 64 59 62\n"+
        "f 58 64 55\n"+
        "f 57 41 65\n"+
        "f 57 65 60\n"+
        "f 41 40 65\n"+
        "f 36 54 66\n"+
        "f 66 35 36\n"+
        "f 67 35 66\n"+
        "f 67 68 35\n"+
        "f 69 68 67\n"+
        "f 69 70 68\n"+
        "f 68 71 35\n"+
        "f 68 72 71\n"+
        "f 70 72 68\n"+
        "f 72 73 71\n"+
        "f 72 74 73\n"+
        "f 75 74 72\n"+
        "f 70 75 72\n"+
        "f 75 76 74\n"+
        "f 77 75 70\n"+
        "f 77 78 75\n"+
        "f 78 76 75\n"+
        "f 78 77 79\n"+
        "f 80 78 79\n"+
        "f 78 80 76\n"+
        "f 79 81 80\n"+
        "f 77 82 79\n"+
        "f 82 77 70\n"+
        "f 82 70 69\n"+
        "f 80 83 76\n"+
        "f 80 84 83\n"+
        "f 85 84 80\n"+
        "f 85 86 84\n"+
        "f 83 84 87\n"+
        "f 88 17 166\n"+
        "f 88 166 90\n"+
        "f 25 89 23\n"+
        "f 91 92 93\n"+
        "f 93 94 91\n"+
        "f 93 95 94\n"+
        "f 96 97 98\n"+
        "f 96 98 99\n"+
        "f 100 96 99\n"+
        "f 100 99 101\n"+
        "f 167 96 100\n"+
        "f 167 168 96\n"+
        "f 104 105 103\n"+
        "f 104 103 106\n"+
        "f 107 104 106\n"+
        "f 107 106 108\n"+
        "f 109 107 108\n"+
        "f 109 108 110\n"+
        "f 111 112 102\n"+
        "f 113 114 115\n"+
        "f 113 115 116\n"+
        "f 54 117 66\n"+
        "f 118 119 120\n"+
        "f 118 120 121\n"+
        "f 118 121 122\n"+
        "f 118 122 123\n"+
        "f 122 124 123\n"+
        "f 125 123 124\n"+
        "f 32 31 126\n"+
        "f 32 126 33\n"+
        "f 24 22 127\n"+
        "f 127 30 24\n"+
        "f 42 56 55\n"+
        "f 128 129 130\n"+
        "f 130 131 128\n"+
        "f 131 132 128\n"+
        "f 132 133 128\n"+
        "f 134 135 136\n"+
        "f 134 136 137\n"+
        "f 138 139 140\n"+
        "f 138 140 141\n"+
        "f 142 143 144\n"+
        "f 142 144 145\n"+
        "f 146 147 148\n"+
        "f 149 150 151\n"+
        "f 149 152 150\n"+
        "f 153 154 155\n"+
        "f 153 155 156\n"+
        "f 157 158 159\n"+
        "f 157 159 160\n"+
        "f 161 162 163\n"+
        "f 161 163 164";

        var hmesh = new HMesh();
        hmesh.BuildFromObj(obj);
        HMeshSimplification.FixDegenerateFaces(hmesh);
        foreach (var v in hmesh.GetVerticesRaw()){
            Assert.True(v.IsValid(HMeshValidationRules.Standard));
        }
        foreach (var he in hmesh.GetHalfedgesRaw()){
            Assert.True(he.IsValid(HMeshValidationRules.Standard));
        }
        foreach (var f in hmesh.GetFacesRaw()){
            Assert.True(f.IsValid(HMeshValidationRules.Standard));
        }
    }

    static double AngleToDist(double angleDegrees) {
        double angleRadians = angleDegrees * Mathf.Deg2Rad;
        Vector2D v0 = new Vector2D(0, 1);
        Vector2D v1 = v0.Rotate(angleRadians);
        
        Debug.Log("angleDegrees "+angleDegrees+" "+v0+" "+v1);
        Debug.Log("Dot "+Vector2D.Dot(v0,v1));
        
        return (v0 - v1).magnitude;
    }
    
    [Test]
    public void Foo()
    {
        Debug.Log("Dist "+AngleToDist(90).ToString("R"));
        Debug.Log("Dist "+AngleToDist(45).ToString("R"));
        Debug.Log("Dist "+AngleToDist(2).ToString("R"));
        Debug.Log("Dist "+AngleToDist(1).ToString("R"));
        Debug.Log("Dist "+AngleToDist(0.5).ToString("R"));
    }
}

