using UnityEngine;

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
