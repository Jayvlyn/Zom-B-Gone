using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectible : ScriptableObject
{
    [Header("Collectible Attributes")]
    [SerializeField] private new string name = "Unnamed";
    [SerializeField] private string description = "";
    [SerializeField] private Sprite icon = null;

    public string Name => name;
    public string Description => description;
    public abstract string ColoredName { get; }
    public Sprite Icon => icon;
    public abstract string GetInfoDisplayText();
}
