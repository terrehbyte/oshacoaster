using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildTile : ScriptableObject
{
    public string myName;
    public Mesh buildMesh;
    public GameObject buildPrefab;

    private void OnValidate()
    {
        if (buildPrefab != null)
        {
            buildMesh = buildPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
        }
    }
    public void ForceValidate()
    {
    
            buildMesh = buildPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
  
    }
}
