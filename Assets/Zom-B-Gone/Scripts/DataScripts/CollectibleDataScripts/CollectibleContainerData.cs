using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Container", menuName = "New Container")]
public class CollectibleContainerData : ScriptableObject
{
    [SerializeField] public ContainerType containerType;
    [SerializeField] public VoidEvent onContainerCollectibleUpdated = null;
    [SerializeField] public VoidEvent onContainerCollectibleSwapped = null;
    [SerializeField] private CollectibleSlot testCollectibleSlot = new CollectibleSlot();
    [SerializeField] private int size = 42;

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
}

public enum ContainerType
{
    HANDS, HEAD, LOCKER, BACKPACK, LOOTABLE
}