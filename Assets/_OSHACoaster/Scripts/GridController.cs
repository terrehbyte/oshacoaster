using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

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
    private GridCell[][] gridCells;

    [SerializeField]
    private LineRenderer gridLines;

    void Start()
    {
        gridCells = new GridCell[gridDimensions.x][];
        for (int i = 0; i < gridDimensions.x; ++i)
        {
            gridCells[i] = new GridCell[gridDimensions.z];
        }

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

    public void WriteGridCell(Vector3Int tileLocation, Object tileObj)
    {
        var cellData = gridCells[tileLocation.x][tileLocation.z];
        cellData.cellObject = tileObj;
        gridCells[tileLocation.x][tileLocation.z] = cellData;
    }

    public bool CheckOccupied(Vector3Int tileLocation)
    {
        try
        {
            return gridCells[tileLocation.x][tileLocation.z].isOccupied;
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

}

public struct GridCell
{
    public Object cellObject;

    public bool isOccupied
    {
        get
        {
            return cellObject != null;
        }
    }
}
