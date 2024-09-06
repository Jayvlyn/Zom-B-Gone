using GameEvents;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField] private CollectibleContainerData craftingTableOutput;
    [SerializeField] private CollectibleContainerData craftingTableInput;
    [SerializeField] private RecipeBook recipeBook;
    [SerializeField] private VoidEvent craftingTableOpened;

    public Recipe foundRecipe;

    private int fullInputSlots;


    public void Interact(bool rightHand)
    {
        OpenCraftingUI();
        gameObject.layer = LayerMask.NameToLayer("World");
    }

    private void OpenCraftingUI()
    {
        craftingTableOpened.Raise();
    }

    public void OnCraftingClosed()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public void OnCraftingInputUpdated()
    {
        int inputSlotCount = craftingTableInput.size;
        fullInputSlots = 0;

        // Find full input slots
        for(int i = 0; i < inputSlotCount; i++)
        {
            if (craftingTableInput.Container.collectibleSlots[i].collectible != null)
            {
                fullInputSlots++;
            }
        }

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
                    if (ri.collectible != craftingTableInput.Container.collectibleSlots[i].collectible) continue;
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
            craftingTableOutput.Container.collectibleSlots[0].collectible = foundRecipe.resultCollectible;
            craftingTableOutput.Container.collectibleSlots[0].quantity = foundRecipe.resultAmount;
        }
        else // no recipe match, clear result slot
        {
            craftingTableOutput.Container.collectibleSlots[0].collectible = null;
            craftingTableOutput.Container.collectibleSlots[0].quantity = 0;
        }
        craftingTableOutput.onContainerCollectibleUpdated.Raise();
    }

    public void OnCraftAccepted()
    {

    }

    public void Interact(Head head)
    {
        throw new System.NotImplementedException();
    }
}