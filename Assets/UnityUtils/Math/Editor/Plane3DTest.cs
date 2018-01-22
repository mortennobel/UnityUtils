using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class Plane3DTest
{
    [Test]
    public void TestPlaneDistance()
    {
        Random.InitState(123);
        for (int i = 0; i < 1000; i++)
        {
            Vector3 direction = Random.onUnitSphere;
            float dist = Random.Range(-9999, 9999);
            Vector3D directionD = new Vector3D(direction);
            Plane3D planeD = new Plane3D(directionD, dist);
            Plane plane = new Plane(direction, dist);
            for (int j = 0; j < 10; j++)
            {
                Vector3 pos = Random.insideUnitSphere * 99999;
                Vector3D posD = new Vector3D(pos);
                double val1 = plane.GetDistanceToPoint(pos);
                double val2 = planeD.GetDistanceToPoint(posD);
                Assert.AreEqual(val1, val2, 0.02, "GetDistanceToPoint " + plane);
                Assert.AreEqual(plane.GetSide(pos), planeD.GetSide(posD), "GetSide " + plane);
            }
        }
    }

    [Test]
    public void TestPlaneCut()
    {
        /*
(2.7, 0.0, 0.0), (3.0, 1.0, 0.0), (2.5, 0.5, 0.0)) with plane Plane normal: (0, 1, 0) dist -0.50000005960464478
*/
        Triangle3D tri = new Triangle3D(new Vector3D(2.7, 0.0, 0.0),new Vector3D(3.0, 1.0, 0.0),new Vector3D(2.5, 0.5, 0.0));
        Plane3D planeD = new Plane3D(new Vector3D(0, 1, 0), -0.50000005960464478);
        var res = Triangle3D.SplitPlaneTriangleIntersection(planeD, tri);
        Assert.True(res.Count==2);
        Assert.True(res[0].ComputeArea()>0.001);
        Assert.True(res[1].ComputeArea()>0.001);
    }

    [Test]
    public void TestPlaneCut2()
    {
        /*
(1, 0, 0) dist -1.9021174907684326 ((2.013901948928833, 0.44548094272613525, -8.2427806854248047), (1.8874917030334473, 0.33960336446762085, -8.1310787200927734), (1.902117133140564, 0.18741984665393829, -8.2625789642333984))
*/
        Triangle3D tri = new Triangle3D(
            new Vector3D(2.013901948928833,  0.44548094272613525, -8.2427806854248047),
            new Vector3D(1.8874917030334473, 0.33960336446762085, -8.1310787200927734),
            new Vector3D(1.902117133140564,  0.18741984665393829, -8.2625789642333984));
        Plane3D planeD = new Plane3D(new Vector3D(1, 0, 0),  -1.9021174907684326);
        var res = Triangle3D.SplitPlaneTriangleIntersection(planeD, tri);
        double areaSum = 0;
        foreach (var t in res)
        {
            areaSum += t.ComputeArea();
        }
        Debug.Log(res.Count);
        Assert.AreEqual(tri.ComputeArea(), areaSum,0.0001, "Was "+areaSum+" expected "+tri.ComputeArea());
    }

    [Test]
    public void TestPlaneDistanceSimple()
    {
        Plane3D planeD = new Plane3D(new Vector3D(0,1,0), 10);
        Assert.AreEqual(0,planeD.GetDistanceToPoint(new Vector3D(0, -10, 0)));
        Assert.AreEqual(20,planeD.GetDistanceToPoint(new Vector3D(0, 10, 0)));
    }

    [Test]
    public void TestPlaneRayCast()
    {
        Random.InitState(123);
        for (int i = 0; i < 1000; i++)
        {
            Vector3 direction = Random.onUnitSphere;
            float dist = Random.Range(-999, 999);
            Vector3D directionD = new Vector3D(direction);
            Plane3D planeD = new Plane3D(directionD, dist);
            Plane plane = new Plane(direction, dist);
            for (int j = 0; j < 10; j++)
            {
                Vector3 pos = Random.insideUnitSphere * 999;
                Vector3 rayDir = Random.onUnitSphere;
                Vector3D posD = new Vector3D(pos);
                Vector3D rayDirD = new Vector3D(rayDir);
                Ray r = new Ray(pos, rayDir);
                RayD rd = new RayD(posD, rayDirD);

                float enter;
                double enterD;
                bool res = plane.Raycast(r, out enter);
                bool resD = planeD.Raycast(rd, out enterD);
                Assert.AreEqual(res, resD, "Raycast Res " + plane + " ray " + r);
                if (enterD < 1e7)
                {
                    Assert.AreEqual((double) enter, enterD, Mathf.Abs(enter) / 1000,
                        "GetDistanceToPoint " + plane + " ray " + r);
                }
            }
        }
    }

    [Test]
    public void TestPlaneSameSide()
    {
        Random.InitState(123);
        for (int i = 0; i < 1000; i++)
        {
            Vector3 direction = Random.onUnitSphere;
            float dist = Random.Range(-999, 999);
            Vector3D directionD = new Vector3D(direction);
            Plane3D planeD = new Plane3D(directionD, dist);
            Plane plane = new Plane(direction, dist);
            for (int j = 0; j < 10; j++)
            {
                Vector3 pos1 = Random.insideUnitSphere * 999;
                Vector3 pos2 = Random.insideUnitSphere * 999;
                Vector3D pos1D = new Vector3D(pos1);
                Vector3D pos2D = new Vector3D(pos2);
                bool ss = plane.SameSide(pos1, pos2);
                bool ssD = planeD.SameSide(pos1D, pos2D);
                Assert.AreEqual(ss, ss, "SameSide " + plane + " " + ss + " " + ssD);
            }
        }
    }

   /* [Test]
    public void TestSplitPlane()
    {
        var triangle = new[]
        {
            new Vector3D(3.0262959559856903,1.6081341366812849,-10.42053258495136),
            new Vector3D(1.7384121313586425,2.5141665181281589,-11.342813113926217),
            new Vector3D(1.7389390144763897,2.5131475914252639,-11.34405807474017)
        };
        var plane = new Plane3D(new Vector3D(0.47139809477123273, -0.62361210276954437, 0.62361188372688925) , 7.8208738177534132);

        var res1 = Triangle3D.SplitPlaneTriangleIntersection(plane, new Triangle3D( triangle[0], triangle[1], triangle[2]));
        for (int i = 0; i < res1.Count; i ++)
        {
            var res = Triangle3D.TestPlaneTriangleIntersection(plane, new Triangle3D( res1[i][0], res1[i][1], res1[i][2]));
            Debug.Log("Same side "+plane.SameSide(res1[i][0], res1[i ][1])+" "+plane.SameSide(res1[i][0], res1[i][2]));
            Debug.Log(string.Format("Intersect {0} {1} {2}", res.intersect, res.isInterior, res.numIntersections));
        }
    }*/
}


/*
public class HMeshUTest  {
    [Test]
    public void HMeshSimplify () {
        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
                         "# www.blender.org\n"+
                         "mtllib untitled.mtl\n"+
                         "o Plane\n"+
                         "v -1.000000 0.000000 1.000000\n"+
                         "v 1.000000 0.000000 1.000000\n"+
                         "v -1.000000 0.000000 -1.000000\n"+
                         "v 1.000000 0.000000 -1.000000\n"+
                         "v 0.000000 0.000000 0.000000\n"+
                         "vn 0.0000 1.0000 0.0000\n"+
                         "usemtl None\n"+
                         "s off\n"+
                         "f 1//1 2//1 5//1\n"+
                         "f 4//1 3//1 5//1\n"+
                         "f 5//1 2//1 4//1\n"+
                         "f 5//1 3//1 1//1\n"+
                         "";
        var hmesh = new HMesh();
        hmesh.BuildFromObj(objTest);
        hmesh.GetFacesRaw()[0].label = 1;
        hmesh.GetFacesRaw()[2].label = 1;
        hmesh.IsValid();
        var collapsed = HMeshSimplification.Simplify(hmesh);
        Assert.AreEqual(1,collapsed);
        Assert.AreEqual(2,hmesh.GetFacesRaw().Count);
    }

    [Test]
    public void HMeshSimplifyFailed () {
        // Simplify should fail due to minimum angle
        string objTest = "# Blender v2.78 (sub 0) OBJ File: ''\n"+
                         "# www.blender.org\n"+
                         "mtllib untitled.mtl\n"+
                         "o Plane\n"+
                         "v -1.000000 0.000000 1.000000\n"+
                         "v 1.000000 0.000000 1.000000\n"+
                         "v -1.000000 0.000000 -1.000000\n"+
                         "v 1.000000 0.000000 -1.000000\n"+
                         "v 0.000000 0.500000 0.000000\n"+
                         "vn 0.0000 1.0000 0.0000\n"+
                         "usemtl None\n"+
                         "s off\n"+
                         "f 1//1 2//1 5//1\n"+
                         "f 4//1 3//1 5//1\n"+
                         "f 5//1 2//1 4//1\n"+
                         "f 5//1 3//1 1//1\n"+
                         "";
        var hmesh = new HMesh();
        hmesh.BuildFromObj(objTest);
        hmesh.GetFacesRaw()[0].label = 1;
        hmesh.GetFacesRaw()[2].label = 1;
        hmesh.IsValid();
        var collapsed = HMeshSimplification.Simplify(hmesh);
        Assert.AreEqual(0, collapsed);
        Assert.AreEqual(4, hmesh.GetFacesRaw().Count);
    }

    [Test]
    public void HMeshSimplifyOnEdge() {
        // Simplify should fail due to minimum angle
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
        Debug.Log(objTest);
        var hmesh = new HMesh();
        hmesh.BuildFromObj(objTest);
        hmesh.GetFacesRaw()[0].label = 1;
        hmesh.GetFacesRaw()[2].label = 1;
        hmesh.IsValid();
        var collapsed = HMeshSimplification.Simplify(hmesh);
        Assert.AreEqual(1, collapsed);
        Assert.AreEqual(2, hmesh.GetFacesRaw().Count);
    }

    [Test]
    public void HMeshSplitTest() {
        for (int i=0;i<10;i++){
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
            Debug.Log(objTest);
            var hmesh = new HMesh();
            hmesh.BuildFromObj(objTest);
            int vertices = hmesh.GetVerticesRaw().Count;
            int edges = hmesh.GetHalfedgesRaw().Count;
            int faces = hmesh.GetFacesRaw().Count;
            var face =hmesh.GetFacesRaw()[Random.Range(0,hmesh.GetFacesRaw().Count)];
            var newVertex = face.Split();
            foreach (var he in newVertex.Circulate())
            {
                Assert.AreEqual(true, he.IsValid());
                Assert.AreEqual(true, he.vert.IsValid());
            }
            Assert.AreEqual(true, face.IsValid());
            Assert.AreEqual(true, hmesh.IsValid());
            Assert.AreEqual(vertices+1, hmesh.GetVerticesRaw().Count);
            Assert.AreEqual(edges+6, hmesh.GetHalfedgesRaw().Count);
            Assert.AreEqual(faces+2, hmesh.GetFacesRaw().Count);
        }
    }

    [Test]
    public void HMeshSplitPlane() {

        // Simplify should fail due to minimum angle
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
        Assert.AreEqual(8+8*2, edges);
        Assert.AreEqual(8, faces);
        var collapsed = HMeshSimplification.Simplify(hmesh);
        Assert.AreEqual(true, hmesh.IsValid());
        Assert.AreEqual(6, hmesh.GetVerticesRaw().Count);
        Assert.AreEqual(6+3*2, hmesh.GetHalfedgesRaw().Count);
        Assert.AreEqual(4, hmesh.GetFacesRaw().Count);
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
                         "f 2//1 4//1 3//1\n"+
                         "";


        // collapse (1,1) to (-1,1)
        {
            var hmesh = new HMesh();
            hmesh.BuildFromObj(objTest);
            foreach (var he in hmesh.GetHalfedges())
            {
                if (he.vert.position.z > 0.9f && he.prev.vert.position.z > 0.9f)
                {

                    he.Collapse(new Vector3D(-1, 0, 1));
                    break;
                }
            }

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
                    he.Collapse(new Vector3D(-1, 0, -1));
                    break;
                }
            }

            Assert.AreEqual(true, hmesh.IsValid());
            Assert.AreEqual(0, hmesh.GetVerticesRaw().Count);
            Assert.AreEqual(0, hmesh.GetHalfedgesRaw().Count);
            Assert.AreEqual(0, hmesh.GetFacesRaw().Count);
        }
        // collapse (-1,1) to (-1,1) (collapse diagonal)
        {
            var hmesh = new HMesh();
            hmesh.BuildFromObj(objTest);
            foreach (var he in hmesh.GetHalfedges())
            {
                if (he.vert.position.x != he.prev.vert.position.x && he.vert.position.z != he.prev.vert.position.z && he.vert.id < he.prev.vert.id)
                {


                    he.Collapse(new Vector3D(1, 0, -1));
                    break;
                }
            }

            Assert.AreEqual(true, hmesh.IsValid());
            Assert.AreEqual(0, hmesh.GetVerticesRaw().Count);
            Assert.AreEqual(0, hmesh.GetHalfedgesRaw().Count);
            Assert.AreEqual(0, hmesh.GetFacesRaw().Count);
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
        Debug.Log(hmesh.CreateDebugData());
        Assert.AreEqual(true, hmesh.IsValid());
        for (int i = 0; i < hmesh.GetHalfedgesRaw().Count; i++)
        {
            var hm = new HMesh();
            hm.BuildFromObj(v);
            Debug.Log("Collapse he "+hm.GetHalfedgesRaw()[i].id+" from "+hm.GetHalfedgesRaw()[i].prev.vert.position+" to "+
                      hm.GetHalfedgesRaw()[i].vert.position);
            hm.GetHalfedgesRaw()[i].Collapse(true);
            Assert.AreEqual(true, hm.IsValid());
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
        var hmesh = new HMesh();
        hmesh.BuildFromObj(v);
        Assert.AreEqual(true, hmesh.IsValid());
        for (int i = 0; i < hmesh.GetHalfedgesRaw().Count; i++)
        {
            var hm = new HMesh();
            hm.BuildFromObj(v);
            Debug.Log("Collapse he "+hm.GetHalfedgesRaw()[i].id+" from "+hm.GetHalfedgesRaw()[i].prev.vert.position+" to "+
                      hm.GetHalfedgesRaw()[i].vert.position);
            hm.GetHalfedgesRaw()[i].Collapse(true);
            Assert.AreEqual(true, hm.IsValid());
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
}*/