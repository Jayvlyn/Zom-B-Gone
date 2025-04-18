using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootrunnerDataRefs : MonoBehaviour
{
	public PlayerData playerData;

    public CollectibleContainerData handsData;
    public CollectibleContainerData headData;
    public CollectibleContainerData backpackData;
    public CollectibleContainerData hatLockerData;
    public CollectibleContainerData itemLockerData;
    public CollectibleContainerData lootLockerData;

    public CollectibleContainerData worbenchInputData;
    public CollectibleContainerData worbenchOutputData;

    public FloorContainer vanFloor;
    public FloorContainer unitFloor;

    public MarketData marketData;

    public Button[] zoneButtons;

    public Image schematicZBG;

    public TMP_Text infoText;
}
