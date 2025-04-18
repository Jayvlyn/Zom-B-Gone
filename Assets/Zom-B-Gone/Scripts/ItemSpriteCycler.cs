using UnityEngine;

public class ItemSpriteCycler : MonoBehaviour
{
    public Item item;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public float changeTime = 0.5f;
    private float changeTimer;
    private int spriteIndex = 0;

    private void Start()
    {
        spriteRenderer.sprite = sprites[0];
        spriteIndex = 0;
    }

    private void Update()
    {
        if(item.currentState == Item.ItemState.HELD)
        {
            if(changeTimer > 0)
            {
                changeTimer -= Time.deltaTime;
            }
            else
            {
                changeTimer = changeTime;
                ChangeSprite();

            }
        }
    }
    
    private void ChangeSprite()
    {
        if(spriteIndex+1 >= sprites.Length)
        {
            spriteIndex = 0;
        }
        else
        {
            spriteIndex++;
        }
        spriteRenderer.sprite = sprites[spriteIndex];
    }
}
