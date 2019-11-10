using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailCarController : MonoBehaviour
{
    public RailConfiguration currentRail;
    public float railSpeed = 20f;
    public float railProgress;

    public Vector3Int prevTilePosition;
    public Vector3Int tilePosition;
    public Vector3 tileVelocity
    {
        get
        {
            return ((Vector3)tilePosition - (Vector3)prevTilePosition).normalized * railSpeed;
        }
    }
    public Rigidbody rbody;

    public float heightOffset = 0.5f;
    public BuildTile.TileConnections currentDirection;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        tilePosition = GamePlay.grid.GetTileLocation(transform.position);

        var cellObj = GamePlay.grid.GetCellData(GamePlay.grid.GetTileLocation(transform.position)).cellObject;
        if (cellObj != null)
        {
            currentRail = cellObj.GetComponent<RailConfiguration>();
            transform.forward = currentRail.transform.forward;
            currentDirection = GridController.GetDirectionFromRotationInt(GamePlay.grid.GetCellData(GamePlay.grid.GetTileLocation(transform.position)).rotations);
        }
    }

    GridCell GetCurrentCell()
    {
        return GamePlay.grid.GetCellData(tilePosition);
    }


    void Update()
    {
        if (currentRail == null)
        {
            enabled = false;
            rbody.isKinematic = false;
            rbody.AddForce(tileVelocity + Vector3.up * 5.0f, ForceMode.Impulse);
            return;
        }

        transform.position = GamePlay.grid.GetInstantiationLocation(tilePosition) + Vector3.up * heightOffset;
        railProgress += railSpeed * Time.deltaTime;

        while(railProgress >= 1.0f)
        {
            // get next tile
            var thing = currentDirection;
            foreach(var route in currentRail.connections)
            {
                if (GridController.GetReciprocalConnection(route.Key) == thing) { continue; }
                thing = route.Key;
                break;
            }
            currentRail = currentRail.connections[thing];

            if (currentRail == null) { return; }
            prevTilePosition = tilePosition;
            tilePosition = currentRail.GetTilePosition();
            transform.forward = currentRail.transform.forward;
            currentDirection = thing;

            railProgress -= 1.0f;
        }
    }
}
