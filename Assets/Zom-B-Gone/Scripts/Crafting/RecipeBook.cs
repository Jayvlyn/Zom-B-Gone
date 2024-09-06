using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe Book", menuName = "New Recipe Book")]
public class RecipeBook : ScriptableObject
{
    public List<Recipe> recipes = new List<Recipe>();
}
