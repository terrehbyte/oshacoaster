using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using System;
using System.Linq;

public class GridController : MonoBehaviour
{
    // Specifies the NxN dimensions of each tile
    //
    // Do NOT change this at run-time or else bad things :(
    [SerializeField]
    private int _tileSize = 2;
    public int tileSize
    {
        get { return _tileSize; }
    }
    public Vector3Int gridDimensions;

    // HACK: I don't care about the Y axis, so always use XZ to access this
    [SerializeField]
    private GridCell[] gridCells;

    [SerializeField]
    private LineRenderer gridLines;

    void Start()
    {
        gridCells = new GridCell[gridDimensions.x * gridDimensions.z];

        // TODO: LINES LINES LINES LINES
        // TODO: Y axis, which is coming never

        gridLines.positionCount = 4;
        Vector3[] positions = new Vector3[4];
        positions[0] = Vector3.zero;
        positions[1] = new Vector3(gridDimensions.x * tileSize, 0, 0);
        positions[2] = new Vector3(gridDimensions.x * tileSize, 0, gridDimensions.z * tileSize);
        positions[3] = new Vector3(0, 0, gridDimensions.z * tileSize);
        gridLines.SetPositions(positions);
    }

    public int GetCellIndexFromTileLocation(Vector3Int tileLocation)
    {
        return tileLocation.x * gridDimensions.x + tileLocation.z;
    }

    public Vector3Int GetTileLocationFromCellIndex(int tileIndex)
    {
        return new Vector3Int(tileIndex / gridDimensions.x, 0, tileIndex % gridDimensions.z);
    }

    public void WriteGridCell(Vector3Int tileLocation, int rotations, GameObject obj, BuildTile tileObj)
    {
        // TODO: magic only FOUR orientations!!!
        Debug.Assert(rotations >= 0 && rotations <= 4);

        var cellData = gridCells[GetCellIndexFromTileLocation(tileLocation)];
        cellData.tileType = tileObj;
        cellData.cellObject = obj;
        cellData.rotations = rotations;
        gridCells[GetCellIndexFromTileLocation(tileLocation)] = cellData;
    }

    public bool CheckOccupied(Vector3Int tileLocation)
    {
        try
        {
            return gridCells[GetCellIndexFromTileLocation(tileLocation)].isOccupied;
        }
        catch (System.IndexOutOfRangeException)
        {
            return false;
        }
    }

    public bool CheckWithinBounds(Vector3Int tileLocation)
    {
        return tileLocation.x < gridDimensions.x && tileLocation.z < gridDimensions.z &&
               tileLocation.x >= 0 && tileLocation.z >= 0;
    }

    public bool CheckBuildEligible(Vector3Int tileLocation)
    {
        return CheckWithinBounds(tileLocation) && !CheckOccupied(tileLocation);
    }

    // Returns the position of a tile in world space
    // This represents the bottom-left corner of the tile
    public Vector3 GetWorldLocation(Vector3Int worldTileLocation)
    {
        return Vector3.Scale((Vector3)worldTileLocation, new Vector3(tileSize, tileSize, tileSize));
    }

    // Returns the position of a tile in tile space
    public Vector3Int GetTileLocation(Vector3 worldLocation)
    {
        worldLocation.Scale(new Vector3(1.0f / tileSize, 1.0f / tileSize, 1.0f / tileSize));
        return Vector3Int.FloorToInt(worldLocation);
    }

    // Returns the center of a tile in world space, given a world space location
    public Vector3 GetInstantiationLocation(Vector3 worldLocation)
    {
        return GetInstantiationLocation(GetTileLocation(worldLocation));
    }

    // Returns the center of a tile in world space, given a tile space locatin
    public Vector3 GetInstantiationLocation(Vector3Int worldTileLocation)
    {
        Vector3 worldLocation = GetWorldLocation(worldTileLocation);
        Vector3 offset = new Vector3((float)tileSize / 2.0f, 0.0f, (float)tileSize / 2.0f);

        return worldLocation + offset;
    }

    public Vector3 GetConnectionPoint(Vector3Int worldTileLocation, BuildTile.TileConnections conn)
    {
        var worldPos = GetInstantiationLocation(worldTileLocation);

        Vector3 offset = Vector3.zero;
        switch (conn)
        {
            case BuildTile.TileConnections.NORTH:
                offset = new Vector3(0, 0, 1);
                break;
            case BuildTile.TileConnections.EAST:
                offset = new Vector3(1, 0, 0);
                break;
            case BuildTile.TileConnections.SOUTH:
                offset = new Vector3(0, 0, -1);
                break;
            case BuildTile.TileConnections.WEST:
                offset = new Vector3(-1, 0, 0);
                break;
        }

        return worldPos + offset * ((float)tileSize / 2.0f);
    }

    public Vector3 GetConnectionOffset(BuildTile.TileConnections conn)
    {
        Vector3 offset = Vector3.zero;
        switch (conn)
        {
            case BuildTile.TileConnections.NORTH:
                offset = new Vector3(0, 0, 1);
                break;
            case BuildTile.TileConnections.EAST:
                offset = new Vector3(1, 0, 0);
                break;
            case BuildTile.TileConnections.SOUTH:
                offset = new Vector3(0, 0, -1);
                break;
            case BuildTile.TileConnections.WEST:
                offset = new Vector3(-1, 0, 0);
                break;
        }

        return offset * ((float)tileSize / 2.0f);
    }

    public GridCell GetCellData(Vector3Int worldTileLocation)
    {
        return gridCells[GetCellIndexFromTileLocation(worldTileLocation)];
    }

    void OnDrawGizmos()
    {
        if (gridCells == null) { return; }

        Gizmos.color = Color.white;
        for (int i = 0; i < gridCells.Length; ++i)
        {
            var cell = gridCells[i];
            if (cell.isOccupied && cell.tileType.tileType == BuildTile.TileTypes.RAIL)
            {
                var gizCon = cell.connections;

                foreach (var con in gizCon)
                {
                    Gizmos.DrawSphere(GetInstantiationLocation(GetTileLocationFromCellIndex(i)) + GetConnectionOffset(con)  , 0.25f);
                }
            }
        }
    }
}

[System.Serializable]
public struct GridCell
{
    public BuildTile tileType;
    public GameObject cellObject;
    public int rotations;

    public bool isOccupied
    {
        get
        {
            return cellObject != null;
        }
    }

    public BuildTile.TileConnections[] connections
    {
        get
        {
            var finalConnections = new BuildTile.TileConnections[connectionCount];
            GetConnections(finalConnections);
            return finalConnections;
        }
    }

    public int GetConnections(BuildTile.TileConnections[] dest)
    {
        var connections = Mathf.Min(connectionCount, dest.Length);
        var baseConnections = tileType.baseConnections;
        for (int i = 0; i < connections; ++i)
        {
            dest[i] = (BuildTile.TileConnections)(((int)baseConnections[i] + rotations) % 4);
        }

        return connections;
    }

    public int connectionCount
    {
        get
        {
            return tileType.baseConnections.Length;
        }
    }

    // Returns the direction that traffic can flow in
    // Returns -1 if no connection can be made
    public int CanConnect(BuildTile.TileConnections[] sourceTypes)
    {
        // build acceptable connections
        var rotatedConnections = connections;
        for (int i = 0; i < rotatedConnections.Length; ++i)
        {
            rotatedConnections[i] = (BuildTile.TileConnections)((int)(rotatedConnections[i] + 2) % 4);
        }

        foreach (var conA in sourceTypes)
        {
            //Array.IndexOf
        }

        return -1;
    }
}