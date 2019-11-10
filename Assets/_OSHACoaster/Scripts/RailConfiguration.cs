using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailConfiguration : MonoBehaviour
{
    public Dictionary<BuildTile.TileConnections, RailConfiguration> connections = new Dictionary<BuildTile.TileConnections, RailConfiguration>();

    void Start()
    {
        if(!GamePlay.grid.CheckWithinBounds(GetTilePosition()))
        {
            enabled = false; return;
        }
        var cons = GamePlay.grid.GetCellData(GetTilePosition()).connections;

        foreach(var con in cons)
        {
            try
            {
                connections.Add(con, null);
            }
            catch (System.ArgumentException ex)
            {

            }
        }
    }

    public Vector3Int GetTilePosition()
    {
        return GamePlay.grid.GetTileLocation(transform.position);
    }

    void OnDrawGizmos()
    {
        if (connections == null) { return; }
        Gizmos.color = Color.green;
        foreach(var pair in connections)
        {
            if(pair.Value == null) continue;
            Gizmos.DrawSphere(GamePlay.grid.GetInstantiationLocation(GetTilePosition()) + GamePlay.grid.GetConnectionOffset(pair.Key), 0.5f);
        }
    }
}
