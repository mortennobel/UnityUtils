using System.Collections.Generic;
using UnityEngine;

/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

[RequireComponent(typeof(LineSegmentRenderer))]
[RequireComponent(typeof(HMeshFilter))]
public class HMeshLineRenderer : HMeshARenderer
{

    public bool showNormals = false;
    public float normalLength = 1;

    public override void UpdateMesh()
    {
        var lr = GetComponent<LineSegmentRenderer>();
        var mf = GetComponent<HMeshFilter>();
        if (lr == null || mf == null)
        {
            Debug.LogWarning("Was null");
            return;
        }
        if (mf.hMesh == null)
        {
            Debug.LogWarning("HMesh was null");
            return;
        }

        var pos = new List<Vector3>();
        foreach (var l in mf.hMesh.GetHalfedgesRaw())
        {
            pos.Add(l.prev.vert.position);
            pos.Add(l.vert.position);
        }

        if (showNormals)
        {
            foreach (var f in mf.hMesh.GetFacesRaw())
            {
                var c = f.GetCenter();
                var n = f.GetNormal();
                pos.Add(c.ToVector3());
                pos.Add((c+n*normalLength).ToVector3());
            }
        }

        lr.positions = pos;
        lr.UpdateMesh();
    }
}
