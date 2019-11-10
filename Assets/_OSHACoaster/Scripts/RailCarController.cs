using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailCarController : MonoBehaviour
{
    public RailConfiguration currentRail;
    public float railSpeed = 0.5f;
    public float railProgress;

    public Vector3Int tilePosition;
    public Rigidbody rbody;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        tilePosition = GamePlay.grid.GetTileLocation(transform.position);

        var cellObj = GamePlay.grid.GetCellData(GamePlay.grid.GetTileLocation(transform.position)).cellObject;
        if (cellObj != null)
        {
            currentRail = cellObj.GetComponent<RailConfiguration>();
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
            return;
        }

        transform.position = GamePlay.grid.GetWorldLocation(tilePosition);
        railProgress += railSpeed * Time.deltaTime;

        if(railProgress >= 1.0f)
        {
            // update tile position
            currentRail = currentRail.nextRailTilePosition;

            if (currentRail == null) { return; }
            tilePosition = currentRail.GetTilePosition();

            railProgress = 0.0f;
        }
    }
}
