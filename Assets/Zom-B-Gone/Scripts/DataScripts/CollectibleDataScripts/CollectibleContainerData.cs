using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Container", menuName = "New Container")]
public class CollectibleContainerData : ScriptableObject
{
    public ContainerType containerType;
    public VoidEvent onContainerCollectibleUpdated = null;
    public VoidEvent onContainerCollectibleSwapped = null;
    public VoidEvent onContainerSwapped = null;
    [SerializeField] private CollectibleSlot testCollectibleSlot = new CollectibleSlot();
    public int size = 42;

    public CollectibleContainer container;
    public CollectibleContainer Container
    {
        get { return container; }
        set
        {
            container = value;
            Container.OnCollectibleUpdated += onContainerCollectibleUpdated.Raise;
            Container.OnCollectibleSwapped += onContainerCollectibleSwapped.Raise;
        }
    }

    private void Awake()
    {
		//Container = new CollectibleContainer(size);
	}

    public void OnEnable()
    {
        Container.OnCollectibleUpdated += onContainerCollectibleUpdated.Raise;
        Container.OnCollectibleSwapped += onContainerCollectibleSwapped.Raise;
    }

    public void OnDisable()
    {
        Container.OnCollectibleUpdated -= onContainerCollectibleUpdated.Raise;
        Container.OnCollectibleSwapped -= onContainerCollectibleSwapped.Raise;
    }

    [ContextMenu("Update UI")]
    public void UpdateUI()
    {
        Container.OnCollectibleUpdated.Invoke();
    }

    [ContextMenu("Test Add")]
    public void TestAdd()
    {
        Container.AddCollectible(ref testCollectibleSlot);
    }

    [ContextMenu("Subscribe to events")]
    public void SubscribeToEvents()
    {
        Container.OnCollectibleUpdated += onContainerCollectibleUpdated.Raise;
        Container.OnCollectibleSwapped += onContainerCollectibleSwapped.Raise;
    }

    public void AddToContainer(CollectibleData data, int amount)
    {
        CollectibleSlot incomingCollectible = new CollectibleSlot(data, data.name, amount);
        Container.AddCollectible(ref incomingCollectible);
    }

    public void AddToContainer(ref CollectibleSlot slot)
    {
        Container.AddCollectible(ref slot);
    }


    public int AddToContainerNSIA(CollectibleData data, int amount)
    {
        CollectibleSlot incomingCollectible = new CollectibleSlot(data, data.name, amount);
        return Container.AddCollectibleNoStackIgnoreAllows(incomingCollectible);
    }

    public void AddSpace(int addedSpace)
    {
        CollectibleSlot[] cachedSlots = Container.collectibleSlots;

        size += addedSpace;
        Container = new CollectibleContainer(size);

        for (int i = 0; i < cachedSlots.Length; i++)
        {
            Container.collectibleSlots[i] = cachedSlots[i];
        }
    }
}

public enum ContainerType
{
    HANDS, HEAD, LOCKER, BACKPACK, LOOTABLE, WORKBENCH, FLOOR
}