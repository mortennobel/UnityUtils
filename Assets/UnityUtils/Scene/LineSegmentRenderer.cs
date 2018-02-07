/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class LineSegmentRenderer : MonoBehaviour {

    public List<Vector3> positions;

    public float lineWidth = 0.1f;

    public void UpdateMesh()
    {
        if (positions.Count % 2 != 0)
        {
            Debug.LogError("Invalid position count - should come in pair (for each line segment)");
            return;
        }

        List<Vector3> meshPositions = new List<Vector3>();
        List<int> meshIndices = new List<int>();

        float l = lineWidth *0.5f;

        for (int j=0;j<positions.Count;j=j+2)
        {
            var from = positions[j];
            var to = positions[j+1];
            if (from == to)
            {
                Debug.LogWarning("from == to");
                continue;
            }
            var normal = (to - from).normalized;
            Vector3 t;
            Vector3 b;
            normal.BranchlessONB(out t, out b);

            for (int i = 0; i < 7; i++)
            {
                if (meshPositions.Count > 64000)
                {
                    break;
                }
                int vertCount = meshPositions.Count;
                meshIndices.Add(vertCount);
                meshIndices.Add(vertCount+1);
                meshIndices.Add(vertCount+2);
                meshIndices.Add(vertCount);
                meshIndices.Add(vertCount+2);
                meshIndices.Add(vertCount+3);

                meshPositions.Add((from - t*l + b*l).RotateAround(from, normal, i*120));
                meshPositions.Add((to   - t*l + b*l).RotateAround(from, normal, i*120));
                meshPositions.Add((to   + t*l + b*l).RotateAround(from, normal, i*120));
                meshPositions.Add((from + t*l + b*l).RotateAround(from, normal, i*120));
            }
        }
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter == null");
            return;
        }
        var mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        mesh.SetTriangles(new List<int>(), 0);
        mesh.SetVertices(meshPositions);
        mesh.SetTriangles(meshIndices, 0);
        meshFilter.sharedMesh = mesh;
    }

}
