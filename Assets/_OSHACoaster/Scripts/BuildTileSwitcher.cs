using UnityEngine;

public class BuildTileSwitcher : MonoBehaviour
{
    public BuildTile assignedTile;
    public DesignController designController;
    public UnityEngine.UI.Button button;

    public void SetButtonState(bool enabled)
    {
        button.interactable = enabled;
    }

    public void Update()
    {
        SetButtonState(GamePlay.inventory[assignedTile.myName].qtyInStock > 0);
    }

    public void SwitchToAssignedTile()
    {
        designController.CurrentBuildCandidate = assignedTile;
    }
}