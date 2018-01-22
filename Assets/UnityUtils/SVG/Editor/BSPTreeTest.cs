using NUnit.Framework;

[TestFixture]
public class BSPTreeTest {

    [Test]
    public void BuildBSPTree()
    {
        var triangles = new[]
        {
            new Vector3D(-1.000000, 0.000000, 0.000000),new Vector3D(1.000000, 0.000000, 0.000000),new Vector3D(1.000000, -1.000000, 0.000000),
            new Vector3D(0.750382, -0.453671, 1.597113),new Vector3D(1.044405, 0.284186, 0.820524),new Vector3D(1.177693, -0.022709, 0.454081),
            new Vector3D(-0.370747, -1.054776, 1.736725),new Vector3D(1.539376, -0.188849, 2.000000),new Vector3D(0.406594, -2.000000, 2.000000)
        };
        /*
		BSPTree bspTree = new BSPTree();
        Assert.Null(bspTree.node,"No root node");
        bspTree.Insert(new Triangle3D( triangles[0],triangles[1],triangles[2]));
        Assert.NotNull(bspTree.node, "First insert as root node");
        Assert.Null(bspTree.node.left, "First insert has empty left");
        Assert.Null(bspTree.node.right, "First insert has empty right");
        bspTree.Insert(new Triangle3D( triangles[3],triangles[4],triangles[5]));
        Assert.Null(bspTree.node.left,"Not inserted to the left");
        Assert.NotNull(bspTree.node.right,"Inserted to the right");
        Assert.Null(bspTree.node.right.left,"Newly inserted node has no children");
        Assert.Null(bspTree.node.right.right,"Newly inserted node has no children");
        bspTree.Insert(new Triangle3D( triangles[6],triangles[7],triangles[8]));
        Assert.Null(bspTree.node.left,"Not inserted to the left");
        Assert.NotNull(bspTree.node.right,"Inserted to the right");
        Assert.NotNull(bspTree.node.right.left,"Newly inserted node is split");
        Assert.NotNull(bspTree.node.right.right,"Newly inserted node is split");
        bspTree.ToList();*/
	}

    [Test]
    public void BuildBSPTreeExample()
    {
        /*
        var triangles = new[]
        {
            new Vector3D(3.6, -3.6, -6.4), new Vector3D(2.2, -1.2, -6.0), new Vector3D(2.2, -2.6, -7.4),
            new Vector3D(3.6, -3.6, -6.4), new Vector3D(3.6, -2.2, -5.0), new Vector3D(2.2, -1.2, -6.0),
            new Vector3D(3.6, -2.2, -5.0), new Vector3D(5.0, -2.6, -7.4), new Vector3D(5.0, -1.2, -6.0),
            new Vector3D(3.6, -2.2, -5.0), new Vector3D(3.6, -3.6, -6.4), new Vector3D(5.0, -2.6, -7.4),
            new Vector3D(3.4, 0.2, -7.5), new Vector3D(2.7, -1.1, -4.6), new Vector3D(4.5, 0.2, -5.9)
        };
        BSPTree bspTree = new BSPTree();
        for (int i = 0; i < triangles.Length; i = i + 3)
        {
            bspTree.Insert(new Triangle3D( triangles[i],triangles[i+1],triangles[i+2]));
        }
        bspTree.ToList();
*/
    }
}

/*
using System.Collections;
using System.Collections.Generic;
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
*/