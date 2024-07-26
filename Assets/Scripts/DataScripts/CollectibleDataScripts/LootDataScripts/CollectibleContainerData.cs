using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Container", menuName = "New Container")]
public class CollectibleContainerData : ScriptableObject
{
    [SerializeField] private VoidEvent onContainerCollectibleUpdated = null;
    [SerializeField] private CollectibleSlot testCollectibleSlot = new CollectibleSlot();

    public CollectibleContainer Container { get; } = new CollectibleContainer(42);

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