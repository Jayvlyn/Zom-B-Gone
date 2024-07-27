using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Container", menuName = "New Container")]
public class CollectibleContainerData : ScriptableObject
{
    [SerializeField] public VoidEvent onContainerCollectibleUpdated = null;
    [SerializeField] private CollectibleSlot testCollectibleSlot = new CollectibleSlot();
    [SerializeField] private int size = 42;

    public CollectibleContainer Container;

    private void Awake()
    {
        Container = new CollectibleContainer(size);
    }

    public void OnEnable()
    {
        Container.OnCollectibleUpdated += onContainerCollectibleUpdated.Raise;
    }

    public void OnDisable()
    {
        Container.OnCollectibleUpdated -= onContainerCollectibleUpdated.Raise;
    }

    [ContextMenu("Test Add")]
    public void TestAdd()
    {
        Container.AddCollectible(testCollectibleSlot);
    }
}