using GameEvents;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public class Workbench : MonoBehaviour, IInteractable
{
    [SerializeField] private CollectibleContainerData craftingTableOutput;
    [SerializeField] private CollectibleContainerData craftingTableInput;
    [SerializeField] private RecipeBook recipeBook;
    [SerializeField] private VoidEvent craftingTableOpened;
    [SerializeField] private CollectibleEvent craftAccepted;

    private bool activeCraftingTable = false;

    public Recipe foundRecipe;

    private int fullInputSlots;


    public void Interact(bool rightHand, PlayerController playerController)
    {
        OpenCraftingUI();
        gameObject.layer = LayerMask.NameToLayer("World");
        activeCraftingTable = true;
    }

    private void OpenCraftingUI()
    {
        craftingTableOpened.Raise();
    }

    public void OnCraftingClosed()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        activeCraftingTable = false;
    }

    public void OnCraftingInputUpdated()
    {
        if (!activeCraftingTable) return;
        int inputSlotCount = craftingTableInput.size;
        fullInputSlots = 0;

        // Find full input slots
        for(int i = 0; i < inputSlotCount; i++)
        {
            if (craftingTableInput.Container.collectibleSlots[i].Collectible != null)
            {
                fullInputSlots++;
            }
        }

        //if (foundRecipe.resultCollectible != null) { 
        //    foundRecipe = new Recipe(); 
        //    return; 
        //}

        // Find crafting recipe to match
        foundRecipe = new Recipe();

        foreach(Recipe r in recipeBook.recipes)
        {
            // Continue when items in recipe dont match how many collectibles are in crafting table
            if (r.recipeItems.Count != fullInputSlots) continue;

            int requirementsMetCount = 0;
            foreach(RecipeItem ri in r.recipeItems) // for each recipe item, see if one of the slots fulfills it's requirements
            {
                for(int i = 0; i < inputSlotCount; i++)
                {
                    if (ri.collectible != craftingTableInput.Container.collectibleSlots[i].Collectible) continue;
                    if (ri.requiredAmount > craftingTableInput.Container.collectibleSlots[i].quantity) continue;
                    requirementsMetCount++;
                    break;
                }
            }
            if (requirementsMetCount == r.recipeItems.Count)
            {
                foundRecipe = r;
                break;
            }
        }

        if(foundRecipe.resultCollectible != null) // recipe made! show crafted item!
        {
            //craftingTableOutput.Container.collectibleSlots[0].Collectible = foundRecipe.resultCollectible;
            craftingTableOutput.Container.collectibleSlots[0].CollectibleName = foundRecipe.resultCollectible.name;
            craftingTableOutput.Container.collectibleSlots[0].quantity = foundRecipe.resultAmount;
        }
        else // no recipe match, clear result slot
        {
            //craftingTableOutput.Container.collectibleSlots[0].Collectible = null;
            if(!craftJustAccepted)
            {
                craftingTableOutput.Container.collectibleSlots[0].CollectibleName = null;
                craftingTableOutput.Container.collectibleSlots[0].quantity = 0;
            }
            else
            {
                craftJustAccepted = false;
            }
        }
        craftingTableOutput.onContainerCollectibleUpdated.Raise();
    }

    bool craftJustAccepted = false;
    public void OnCraftAccepted()
    {
        if (!activeCraftingTable) return;
        foreach(RecipeItem ri in foundRecipe.recipeItems)
        {
            for (int i = 0; i < craftingTableInput.size; i++) // loop through each slot in crafting table input
            {
                if(ri.collectible == craftingTableInput.Container.collectibleSlots[i].Collectible) // see if this recipe item collectible matches this slot's
                {
                    // deduct amount from inputted collectible
                    craftingTableInput.Container.collectibleSlots[i].quantity -= ri.requiredAmount;

                    // if amount deducted reduces the inputted collectible to nothing, remove it from slot completely
                    if (craftingTableInput.Container.collectibleSlots[i].quantity == 0) craftingTableInput.Container.collectibleSlots[i].CollectibleName = null;
                    break;
                }
            }
        }
        craftJustAccepted = true;
        craftingTableInput.onContainerCollectibleUpdated.Raise();
        craftAccepted.Raise(craftingTableOutput.Container.collectibleSlots[0].Collectible);
        //foundRecipe.resultCollectible = null;
    }

   
}