using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

[CreateAssetMenu(fileName = "New Backpack", menuName = "Loot/Backpack")]
public class Backpack : ScriptableObject
{
    [SerializeField] private VoidEvent onBackpackLootUpdated = null;
    [SerializeField] private LootSlot testLootSlot = new LootSlot();

    public LootContainer Container { get; } = new LootContainer(42);

    public void OnEnable()
    {
        Container.OnLootUpdated += onBackpackLootUpdated.Raise;
    }

    public void OnDisable()
    {
        Container.OnLootUpdated -= onBackpackLootUpdated.Raise;
    }

    [ContextMenu("Test Add")]
    public void TestAdd()
    {
        Container.AddLoot(testLootSlot);
    }
}