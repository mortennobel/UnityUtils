using UnityEngine;

// When added the component can be modified using HMeshFilterOperationEditor
[RequireComponent(typeof(HMeshFilter))]
public class HMeshFilterOperation : MonoBehaviour
{
    public Material[] meshMaterials;

    public int edges = 4;
}
