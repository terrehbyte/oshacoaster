using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildTile : ScriptableObject
{
    public enum TileTypes
    {
        NONE = 0,
        WALKABLE = 1,
        SCENARY = 2,
        ATTRACTION = 3,
        RAIL = 4
    }

    public enum TileConnections
    {
        NONE = -1,
        NORTH = 0,
        EAST = 1,
        SOUTH = 2,
        WEST = 3
    }

    public string myName;
    public TileTypes tileType;
    public TileConnections[] baseConnections = new TileConnections[0];
    public Mesh buildMesh;
    public GameObject buildPrefab;
    public int dangerContribution = 10;

    public static TileConnections GetInvertedConnection(TileConnections con)
    {
        return (BuildTile.TileConnections)(((int)con + 2) % 4);
    }

    private void OnValidate()
    {
        if (buildPrefab != null)
        {
            buildMesh = buildPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        Debug.Assert(baseConnections != null);
        Debug.Assert(baseConnections?.Length < 4);
    }
    public void ForceValidate()
    {
        buildMesh = buildPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;  
    }
}
