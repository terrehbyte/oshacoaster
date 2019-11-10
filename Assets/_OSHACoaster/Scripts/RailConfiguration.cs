using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailConfiguration : MonoBehaviour
{
    public RailConfiguration nextRailTilePosition;

    //public Dictionary<BuildTile.TileConnections, RailConfiguration> connections;

    public Vector3Int GetTilePosition()
    {
        return GamePlay.grid.GetTileLocation(transform.position);
    }

    void OnDrawGizmos()
    {
        if (nextRailTilePosition == null) { return; }
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, nextRailTilePosition.transform.position);
    }
}
