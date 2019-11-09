using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildTile : ScriptableObject
{
    public Mesh buildMesh;
    public GameObject buildPrefab;

    private void OnValidate()
    {
        if (buildPrefab != null)
        {
            buildMesh = buildPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
        }
    }
}
