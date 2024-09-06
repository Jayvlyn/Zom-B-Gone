using System;
using System.Collections.Generic;

[Serializable]
public struct Recipe
{
    public CollectibleData resultCollectible;
    public int resultAmount;
    public List<RecipeItem> recipeItems;

    public Recipe(int parameterless=0)
    {
        resultCollectible = null;
        resultAmount = 0;
        recipeItems = new List<RecipeItem>();
    }
}