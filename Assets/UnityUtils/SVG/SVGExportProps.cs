/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SVGExportProps : MonoBehaviour
{
    public Vector3 baseDirection = new Vector3(0,1,0);

    public void OnDrawGizmosSelected()
    {
        var meshFilter = GetComponent<MeshFilter>();
        var meshRenderer = GetComponent<MeshRenderer>();

        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawWireCube(meshRenderer.bounds.center,meshRenderer.bounds.size);
        var direction = transform.TransformVector(baseDirection);
        Gizmos.DrawLine(meshRenderer.bounds.center,meshRenderer.bounds.center+direction*meshRenderer.bounds.size.magnitude);
    }
}
