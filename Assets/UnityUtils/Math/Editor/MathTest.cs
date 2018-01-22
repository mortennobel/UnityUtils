using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class MathTest {

    [Test]
    public void TestIsParallelDist()
    {
        Vector3D d1 = new Vector3D(0,0,1);
        Vector3D d2 = new Vector3D(0,0.002,1);
        double treshold = 0.002;
        Assert.True(Vector3D.IsParallelDist(d1,d2,treshold*treshold));
        Assert.False(Vector3D.IsParallelDist(d1,d2,treshold*treshold*treshold));

        d1 = -d1;
        Assert.True(Vector3D.IsParallelDist(d1,d2,treshold*treshold));
        Assert.False(Vector3D.IsParallelDist(d1,d2,treshold*treshold*treshold));

        d1 = new Vector3D(0,0,2);
        d2 = new Vector3D(0,0.002,2);
        Assert.True(Vector3D.IsParallelDist(d1,d2,treshold*treshold));
        Assert.False(Vector3D.IsParallelDist(d1,d2,treshold*treshold*treshold));

        d1 = new Vector3D(0,0,2);
        d2 = new Vector3D(0,0,2);
        Assert.True(Vector3D.IsParallelDist(d1,d2,treshold*treshold));
        
        d1 = new Vector3D(0,0,2);
        d2 = new Vector3D(0,2,0);
        Assert.False(Vector3D.IsParallelDist(d1,d2,treshold*treshold));
        

        
    }
}
