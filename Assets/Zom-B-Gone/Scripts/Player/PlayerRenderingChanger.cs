using UnityEngine;

public class PlayerRenderingChanger : MonoBehaviour
{
    public PlayerController controller;

    private int defaultPlayerSortingLayerID;
    private int lowerPlayerSortingLayerID;

    private void Start()
    {
        defaultPlayerSortingLayerID = SortingLayer.NameToID("Player");
        lowerPlayerSortingLayerID = SortingLayer.NameToID("LowerPlayer");
    }

    public void DoLowerSorting()
    {
        controller.playerSprite.sortingLayerID = lowerPlayerSortingLayerID;
        controller.head.wornHat.ChangeSortingLayer(controller.head.wornHat.lowerSortingLayerID);
    }


    public void DoDefaultSorting()
    {
        controller.playerSprite.sortingLayerID = defaultPlayerSortingLayerID;
        controller.head.wornHat.ChangeSortingLayer(controller.head.wornHat.wornSortingLayerID);
    }
}
