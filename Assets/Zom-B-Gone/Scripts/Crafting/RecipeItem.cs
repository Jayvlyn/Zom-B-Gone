using System;

[Serializable]
public struct RecipeItem
{
    public CollectibleData collectible;
    public int requiredAmount;

    public RecipeItem(CollectibleData collectible, int requiredAmount)
    {
        this.collectible = collectible;
        this.requiredAmount = requiredAmount;
    }
}
