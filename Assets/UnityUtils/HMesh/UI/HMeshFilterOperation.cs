using UnityEngine;

/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

// When added the component can be modified using HMeshFilterOperationEditor
[RequireComponent(typeof(HMeshFilter))]
public class HMeshFilterOperation : MonoBehaviour
{
    public Material[] meshMaterials;

    public int edges = 4;
}
