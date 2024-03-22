using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField] protected Image icomImage = null;

    public int SlotIndex {  get; private set; }

    public abstract Collectible SlotCollectible { get; set; }

    private void OnEnable()
    {
        UpdateSlotUI();
    }

    protected virtual void Start()
    {
        SlotIndex = transform.GetSiblingIndex();
        UpdateSlotUI();
    }

    public abstract void OnDrop(PointerEventData eventData);

    public abstract void UpdateSlotUI();

    protected virtual void EnableSlotUI(bool enable)
    {
        icomImage.enabled = enable;
    }
}
