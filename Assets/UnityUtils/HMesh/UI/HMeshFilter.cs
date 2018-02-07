using UnityEngine;

/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */


public class HMeshFilter : MonoBehaviour
{

    public HMesh hMesh;


    public void SetHMesh(HMesh m)
    {
        hMesh = m;

        foreach (var c in GetComponents<HMeshARenderer>())
        {
            c.UpdateMesh();
        }
    }


}
