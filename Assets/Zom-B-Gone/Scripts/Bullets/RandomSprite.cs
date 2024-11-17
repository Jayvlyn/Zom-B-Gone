using UnityEngine;

public class RandomSprite : MonoBehaviour
{
	[SerializeField] private Sprite[] sprites;
	[SerializeField] private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		int roll = Random.Range(0, sprites.Length);
		spriteRenderer.sprite = sprites[roll];
	}
}
