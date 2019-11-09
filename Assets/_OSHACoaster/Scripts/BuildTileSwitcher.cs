using UnityEngine;

public class BuildTileSwitcher : MonoBehaviour
{
    public BuildTile assignedTile;
    public DesignController designController;

    public void SwitchToAssignedTile()
    {
        designController.CurrentBuildCandidate = assignedTile;
    }
}