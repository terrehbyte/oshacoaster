﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildTile : ScriptableObject
{
    public enum TileTypes
    {
        NONE,
        WALKABLE,
        SCENARY,
        ATTRACTION,
        RAIL
    }

    public enum TileConnections
    {
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
