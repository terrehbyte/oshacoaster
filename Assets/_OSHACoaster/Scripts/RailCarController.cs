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
        currentRail = GamePlay.grid.GetCellData(GamePlay.grid.GetTileLocation(transform.position)).cellObject.GetComponent<RailConfiguration>();
    }

    GridCell GetCurrentCell()
    {
        return GamePlay.grid.GetCellData(tilePosition);
    }

    void Update()
    {
        transform.position = GamePlay.grid.GetWorldLocation(tilePosition);
        railProgress += railSpeed * Time.deltaTime;

        if(railProgress >= 1.0f)
        {
            // update tile position
            currentRail = currentRail.nextRailTilePosition;
            tilePosition = currentRail.GetTilePosition();
        }
    }
}
