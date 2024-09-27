using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "New Container", menuName = "New Container")]
public class CollectibleContainerData : ScriptableObject
{
    [SerializeField] public ContainerType containerType;
    [SerializeField] public VoidEvent onContainerCollectibleUpdated = null;
    [SerializeField] public VoidEvent onContainerCollectibleSwapped = null;
    [SerializeField] private CollectibleSlot testCollectibleSlot = new CollectibleSlot();
    [SerializeField] public int size = 42;

    public CollectibleContainer Container;

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

    [ContextMenu("Test Add")]
    public void TestAdd()
    {
        Container.AddCollectible(testCollectibleSlot);
    }

    public void AddToContainer(CollectibleData data, int amount)
    {
        CollectibleSlot incomingCollectible = new CollectibleSlot(data, amount);
        Container.AddCollectible(incomingCollectible);
    }

    public int AddToContainerNSIA(CollectibleData data, int amount)
    {
        CollectibleSlot incomingCollectible = new CollectibleSlot(data, amount);
        return Container.AddCollectibleNoStackIgnoreAllows(incomingCollectible);
    }

    public void AddSpace(int addedSpace)
    {
        CollectibleSlot[] cachedSlots = Container.collectibleSlots;

        size += addedSpace;
        Container = new CollectibleContainer(size);
        Container.OnCollectibleUpdated += onContainerCollectibleUpdated.Raise;
        Container.OnCollectibleSwapped += onContainerCollectibleSwapped.Raise;

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